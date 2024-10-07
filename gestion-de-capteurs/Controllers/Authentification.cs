using gestion_de_capteurs.Dto;
using gestion_de_capteurs.Entity;
using gestion_de_capteurs.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace gestion_de_capteurs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthentificationController : ControllerBase
    {
        private readonly AuthentificationService _authentificationService;
        private readonly IConfiguration _configuration;

        public AuthentificationController(AuthentificationService authentificationService, IConfiguration configuration)
        {
            _authentificationService = authentificationService;
            _configuration = configuration;
        }

        // POST: api/Authentification/register
        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            var result = await _authentificationService.Register(registerDto);
            if (result == "User already exists")
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        // POST: api/Authentification/login
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginDto loginDto)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var (errorMessage, userId) = await _authentificationService.Login(loginDto, jwtKey);

            if (errorMessage != null)
            {
                return Unauthorized(errorMessage);
            }

            // Generate token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, loginDto.Username)
        }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new LoginResponse
            {
                Message = "Login successful! You can now access the SensorsController.",
                Token = tokenString
            });
        }

    }
}
