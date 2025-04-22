using BCrypt.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Mind_Map.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Mind_Map.Application.Users.Commands.LoginUser
{
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, string?>
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private static bool IsValidEmail(string email)
        {
            return !string.IsNullOrWhiteSpace(email) &&
                   email.Contains('@') &&
                   email.Contains('.') &&
                   email.IndexOf('@') < email.LastIndexOf('.');
        }
        public LoginUserHandler(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string?> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email.Trim().ToLower(), cancellationToken);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password.Trim(), user.PasswordHash))
                return null;

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]));

            // Use ExpirationMinutes from appsettings.json
            var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationMinutes"]);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: new[]
                {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // Subject - User ID
            new Claim(JwtRegisteredClaimNames.Email, user.Email), // Optional - User Email
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) // NameIdentifier for Authorization
                },
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}