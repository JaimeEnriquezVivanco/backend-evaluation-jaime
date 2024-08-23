using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Postable;
using Postable.Entities;
using Postable.Entities.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductSalesApi.Controllers
{
    [Route("")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly PostableDbContext _context;

        public AuthController(IConfiguration configuration, PostableDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Register([FromBody] UserPostDto userDto)
        {
            var user = new User
            {
                Username = userDto.Username,
                Password = userDto.Password,
                Email = userDto.Email,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Role = userDto.Role,
                CreatedAt = DateTime.Now
            };

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                UserDisplayDto newUser = new UserDisplayDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreatedAt = user.CreatedAt
                };

                return Ok(newUser);
            }
            catch(DbUpdateException)
            {
                return BadRequest("Username and/or Email already taken");
            }
        }

        [HttpPost("login")]
        public IActionResult
        Login([FromBody] UserLogin userLogin)
        {
            var user = _context.Users
                .FirstOrDefault(u =>
                    u.Username == userLogin.Username &&
                    u.Password == userLogin.Password
                );
            
            if (user == null)
            {
                    return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(
                    JwtRegisteredClaimNames.Sub,
                    user.Username
                ),
                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString()
                ),
                new Claim(
                    ClaimTypes.Role,
                    user.Role
                )
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );
            
            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(300),
                signingCredentials: creds
            );

            return Ok(
                new 
                {
                    token = new JwtSecurityTokenHandler()
                        .WriteToken(token)
                }
            );
        }
    }

    public class UserLogin
    {
            public required string Username { get; set; }
            public required string Password { get; set; }
    }
}
