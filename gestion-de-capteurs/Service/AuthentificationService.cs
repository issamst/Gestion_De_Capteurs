using gestion_de_capteurs.Context;
using gestion_de_capteurs.Dto;
using gestion_de_capteurs.Entity;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System.Threading.Tasks;

namespace gestion_de_capteurs.Service
{
    public class AuthentificationService
    {
        private readonly AppDbContext _context;

        public AuthentificationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> Register(RegisterDto registerDto)
        {
            if (await _context.Authentifications.AnyAsync(u => u.User == registerDto.Username))
            {
                return "User already exists";
            }

            var auth = new Authentification
            {
                Fullname = registerDto.Fullname,
                User = registerDto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
            };

            _context.Authentifications.Add(auth);
            await _context.SaveChangesAsync();

            return "User registered successfully";
        }

        public async Task<(string Message, string Token)> Login(LoginDto loginDto, string jwtKey)
        {
            var user = await _context.Authentifications.SingleOrDefaultAsync(u => u.User == loginDto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                return ("Invalid username or password", null);
            }

            // Token generation logic will be handled in the controller
            return (null, user.Id.ToString());
        }
    }
}
