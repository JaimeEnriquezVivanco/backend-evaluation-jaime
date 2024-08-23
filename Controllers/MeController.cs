using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Postable;
using Postable.Entities;
using Postable.Entities.Dtos;

namespace postable_evaluation_JaimeEnriquezVivanco.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MeController : Controller
    {
        private readonly PostableDbContext _context;

        public MeController(PostableDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<User>> GetMe()
        {
            var username =  User
                .FindFirstValue(ClaimTypes.NameIdentifier);
            
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u =>
                        u.Username == username
                    );

                if (user == null)
                {
                    return NotFound();
                }

                UserDisplayDto response = new UserDisplayDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreatedAt =user.CreatedAt
                };

                return Ok(response);
            }
            catch
            {
                return BadRequest(username);
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteMe()
        {
            var username =  User
                .FindFirstValue(ClaimTypes.NameIdentifier);
            
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == username
                );
            
            var hangingLikes = await _context.Likes
                .Include(l => l.Post)
                .Where(l =>
                    l.Post!.UserId != user!.Id &&
                    l.UserId == user.Id
                )
                .ToListAsync();

            if (user != null)
            {

                foreach(var like in hangingLikes)
                {
                    _context.Likes.Remove(like);
                }

                _context.Users.Remove(user);

                await _context.SaveChangesAsync();
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPatch]
        [Authorize]
        public async Task<IActionResult>
        PatchMe([FromBody] UserPatchDto patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var username =  User
                .FindFirstValue(ClaimTypes.NameIdentifier);
            
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == username
                );
            
            if (user != null)
            {
                user.Email = patch.Email;
                user.FirstName = patch.FirstName;
                user.LastName = patch.LastName;

                if (!TryValidateModel(user))
                {
                    return BadRequest(ModelState);
                }

                bool isEmailProvided = patch.Email != null;

                bool isEmailTaken = await _context.Users
                    .FirstOrDefaultAsync(u =>
                        u.Email == patch.Email
                    )
                    != null;

                if (isEmailProvided && isEmailTaken)
                {
                    return BadRequest("Email is already in use");
                }

                await _context.SaveChangesAsync();
                
                UserDisplayDto response = new UserDisplayDto {
                    Id = user.Id,
                    Username = user.Username,
                    Email = patch.Email,
                    FirstName = patch.FirstName,
                    LastName = patch.LastName,
                    CreatedAt = user.CreatedAt
                };

                return Ok(response);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}