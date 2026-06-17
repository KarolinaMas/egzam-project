using ExamProject.Data;
using ExamProject.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExamProject.Services
{
    public class UserService : IUserService
    {
        private readonly ExamProjectDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(ExamProjectDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<int> AddAsync(string userName, string email, string password)
        {
            if (await _context.Users.AnyAsync(u => u.UserName == userName))
                throw new ArgumentException("Username is already taken.");

            if (await _context.Users.AnyAsync(u => u.Email == email))
                throw new ArgumentException("Email is already in use.");

            var user = new User
            {
                UserName = userName,
                Email = email,
                Role = "user",
                CreatedAt = DateTime.UtcNow,
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user.Id;
        }

        public async Task<User?> GetAsync(int id) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return result == PasswordVerificationResult.Success ? user : null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
