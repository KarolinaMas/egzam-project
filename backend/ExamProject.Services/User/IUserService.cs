using ExamProject.Entities;

namespace ExamProject.Services
{
    public interface IUserService
    {
        Task<int> AddAsync(string userName, string email, string password, string role);
        Task<User?> GetAsync(int id);
        Task<User?> LoginAsync(string email, string password);
        Task<User?> GetByEmailAsync(string email);
        Task<bool> DeleteAsync(int id);
    }
}
