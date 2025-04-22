using MediatR;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Mind_Map.Models;
using System.Text.RegularExpressions;

namespace Mind_Map.Application.Users.Handlers
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, int>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RegisterUserHandler> _logger;

        public RegisterUserHandler(AppDbContext context, ILogger<RegisterUserHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Normalize and validate inputs
                request.Email = request.Email?.Trim().ToLower() ?? throw new ArgumentException("Email is required");
                request.PasswordHash = request.PasswordHash?.Trim() ?? throw new ArgumentException("Password is required");

                // 2. Basic email validation
                if (!IsValidEmail(request.Email))
                    throw new ArgumentException("Invalid email format");

                // 3. Check for existing user
                if (await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
                    throw new Exception("Email already registered");

                // 4. Create user with all required fields
                var user = new User
                {
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.PasswordHash),
                    Name = request.Name?.Trim() ?? string.Empty,
                    Age = request.Age,
                    Gender = request.Gender?.Trim() ?? string.Empty,
                    CreatedAt = DateTime.UtcNow
                };

                // 5. Save to database
                _context.Users.Add(user);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("New user registered: {Email}", request.Email);
                return user.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for {Email}", request.Email);
                throw;
            }
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                // Simple but effective validation
                return !string.IsNullOrWhiteSpace(email) &&
                       email.Contains('@') &&
                       email.IndexOf('@') > 0 &&
                       email.LastIndexOf('.') > email.IndexOf('@') + 1;
            }
            catch
            {
                return false;
            }
        }
    }
}