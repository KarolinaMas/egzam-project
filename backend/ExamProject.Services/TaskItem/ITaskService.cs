using ExamProject.Entities;

namespace ExamProject.Services
{
    public interface ITaskItemService
    {
        Task<int> CreateAsync(int userId, CreateTaskItemDto taskItem);

        Task<TaskItem?> GetAsync(int userId, int id);

        Task<List<TaskItem>> GetAllAsync(int userId, int page, int itemsPerPage);

        Task<TaskItem?> UpdateAsync(int userId, int id, UpdateTaskItemDto dto);

        Task<bool> DeleteAsync(int userId, int id);
        Task<List<AdminTaskItemDto>> GetAllAdminAsync();
        Task<List<AdminTaskItemDto>> GetByUserIdAsync(int userId);
        Task<AdminLatestTaskDto?> GetLatestTaskByUserIdAsync(int userId);
        Task<bool> DeleteLatestTaskByUserIdAsync(int userId);
    }
}