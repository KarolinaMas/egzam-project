using ExamProject.Entities;

namespace ExamProject.Services
{
    public interface IUserService
    {
        Task<int> AddAsync(string userName, string email, string password);
        Task<User?> GetAsync(int id);
        Task<User?> LoginAsync(string email, string password);
        Task<bool> DeleteAsync(int id);
    }
}
