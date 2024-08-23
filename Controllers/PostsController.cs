using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Postable.Entities;
using Postable.Entities.Dtos;

namespace Postable.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly PostableDbContext _context;

        public PostsController(PostableDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>>
        GetPosts(
            [FromQuery] string? username,
            [FromQuery] string? orderBy = "createdAt",
            [FromQuery] string? order = "asc")
        {

            if (orderBy != "createdAt"
                && orderBy != "likesCount")
            {
            return NotFound("Invalid value for 'orderBy' parameter. Only 'createdAt' or 'likesCount' are valid values.");
            }

            if (order != "asc" && order != "desc")
            {
            return NotFound("Invalid value for 'order' parameter. Only 'asc' or 'desc' are valid values.");
            }

            IEnumerable<Post> allPosts = await _context.Posts
                .Include(p => p.User)
                .Include(P => P.Like)
                .ToListAsync();

            IEnumerable<Post> posts;
            if (username != null)
            {
                posts = allPosts
                    .Where(p => p.User.Username == username);
            }
            else
            {
                posts = allPosts;
            }

            if (orderBy == "createdAt")
            {
                posts = posts.OrderBy(p => p.CreatedAt);
            }

            if (orderBy == "likesCount")
            {
                posts = posts.OrderBy(p => p.Like.Count);
            }

            if (order == "desc")
            {
                posts = posts.Reverse();
            }

            List<PostDisplayDto> dtoPosts = [];
            
            foreach(Post post in posts)
            {
                PostDisplayDto dtoPost = new PostDisplayDto
                {
                    Id = post.Id,
                    Content = post.Content,
                    CreatedAt = post.CreatedAt,
                    Username = post.User.Username,
                    LikesCount = post.Like.Count
                };

                dtoPosts.Add(dtoPost);
            }

            return Ok(dtoPosts);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Post>>
        PostPost(PostPostDto postDto)
        {
            var username =  User
                .FindFirstValue(ClaimTypes.NameIdentifier);
            
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == username
                );

            var post = new Post
            {
                UserId = user!.Id,
                User = user,
                Content = postDto.Content,
                CreatedAt = DateTime.Now
            };

            _context.Posts.Add(post);
            
            try
            {
                await _context.SaveChangesAsync();

                PostDisplayDto response = new PostDisplayDto
                {
                    Id = post.Id,
                    Content = post.Content,
                    CreatedAt = post.CreatedAt,
                    Username = username,
                    LikesCount = 0
                };

                return Ok(response);
            }
            catch (DbUpdateException)
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult>
        PatchPost(	
            int id,
            [FromBody] PostPatchDto patch
        )
        {
            var post = await _context.Posts
                .Include(p => p.Like)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            var username =  User
                .FindFirstValue(ClaimTypes.NameIdentifier);
            
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == username
                );

            bool isUserAuthor = post.User.Username == username;

            if (!isUserAuthor)
            {
                return Unauthorized("User is not post's author");
            }

            post.Content = patch.Content;
            await _context.SaveChangesAsync();

            PostDisplayDto response = new PostDisplayDto
            {
                Id = post.Id,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                Username = username,
                LikesCount = post.Like.Count,
            };

            return Ok(response);
        }

        [HttpPost("{postId}/like")]
        [Authorize]
        public async Task<IActionResult> LikePost(int postId)
        {
            var username = User
                .FindFirstValue(ClaimTypes.NameIdentifier);
            
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == username
                );

            Post? post = await _context.Posts
                .Include(p => p.Like)
                .FirstOrDefaultAsync(p => p.Id == postId);
            
            if(post == null)
            {
                return NotFound("Post not found");
            }

            User? author = await _context.Users
                .FindAsync(post.UserId);

            if(user == null)
            {
                return BadRequest("Invalid user Id");
            }

            Like like = new Like
                {
                    UserId = user.Id,
                    User = user,

                    PostId = post.Id,
                    Post = post,

                    CreatedAt = DateTime.Now
                };

            _context.Likes.Add(like);

            try
            {
                await _context.SaveChangesAsync();
                
                PostDisplayDto response = new PostDisplayDto
                {
                    Id = post.Id,
                    Content = post.Content,
                    CreatedAt = post.CreatedAt,
                    Username = author!.Username,
                    LikesCount = post.Like.Count
                };

                return Ok(response);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Something is wrong.");
            }
        }

        [HttpDelete("{postId}/like")]
        [Authorize]
        public async Task<IActionResult>
        DeletePostLike(int postId)
        {
            var username =  User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == username
                );

            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Like)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if(post == null)
            {
                return NotFound("Post not found");
            }

            Like? like = await _context.Likes
                .FirstOrDefaultAsync(l =>
                    l.UserId == user!.Id &&
                    l.PostId == post.Id
                );

            if(like != null)
            {
                _context.Likes.Remove(like);

                try
                {
                    await _context.SaveChangesAsync();
                    
                    PostDisplayDto response = new PostDisplayDto
                    {
                        Id = post.Id,
                        Content = post.Content,
                        CreatedAt = post.CreatedAt,
                        Username = post.User.Username,
                        LikesCount = post.Like.Count
                    };

                    return Ok(response);
                }
                catch (DbUpdateException)
                {
                    return BadRequest();
                }
            }
            else
            {
                return NotFound("Like not found");
            }
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}