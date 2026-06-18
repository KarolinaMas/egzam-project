using ExamProject.Data;
using ExamProject.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExamProject.Services
{
    public class TaskItemService : ITaskItemService
    {
        private readonly ExamProjectDbContext _context;
        private const int DefaultItemsPerPage = 10;

        public TaskItemService(ExamProjectDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(int userId, CreateTaskItemDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Title is required.");

            if (string.IsNullOrWhiteSpace(dto.Description))
                throw new ArgumentException("Description is required.");

            var taskItem = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                UserId = userId 
            };

            await _context.Tasks.AddAsync(taskItem);
            await _context.SaveChangesAsync();

            return taskItem.Id;
        }

        public async Task<TaskItem?> GetAsync(int userId, int id)
        {
            return await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId); 
        }

        public async Task<List<TaskItem>> GetAllAsync(int userId, int page, int itemsPerPage)
        {
            if (page <= 0)
                page = 1;

            if (itemsPerPage <= 0)
                itemsPerPage = DefaultItemsPerPage;

            return await _context.Tasks
                .Where(t => t.UserId == userId) 
                .OrderBy(t => t.Id)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync();
        }

        public async Task<TaskItem?> UpdateAsync(int userId, int id, UpdateTaskItemDto dto)
        {
            var taskItem = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (taskItem == null)
                return null;

            taskItem.Title = dto.Title;
            taskItem.Description = dto.Description;
            taskItem.IsComplete = dto.IsCompleted;
            taskItem.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return taskItem;
        }

        public async Task<bool> DeleteAsync(int userId, int id)
        {
            var deletedRows = await _context.Tasks
                .Where(t => t.Id == id && t.UserId == userId) 
                .ExecuteDeleteAsync();

            return deletedRows > 0;
        }

        public async Task<List<AdminTaskItemDto>> GetAllAdminAsync()
        {
            return await _context.Tasks
                .Include(t => t.User)
                .Select(t => new AdminTaskItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    IsComplete = t.IsComplete,
                    UserId = t.UserId,
                    UserEmail = t.User.Email
                })
                .ToListAsync();
        }

        public async Task<List<AdminTaskItemDto>> GetByUserIdAsync(int userId)
        {
            return await _context.Tasks
                .Include(t => t.User)
                .Where(t => t.UserId == userId)
                .Select(t => new AdminTaskItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    IsComplete = t.IsComplete,
                    UserId = t.UserId,
                    UserEmail = t.User.Email
                })
                .ToListAsync();
        }

        public async Task<AdminLatestTaskDto?> GetLatestTaskByUserIdAsync(int userId)
        {
            var task = await _context.Tasks
                .Include(t => t.User)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.UpdatedAt > t.CreatedAt ? t.UpdatedAt : t.CreatedAt)
                .FirstOrDefaultAsync();

            if (task == null)
                return null;

            return new AdminLatestTaskDto
            {
                UserId = task.UserId,
                UserEmail = task.User.Email,

                TaskId = task.Id,
                Title = task.Title,
                IsComplete = task.IsComplete,

                LastActivity = task.UpdatedAt > task.CreatedAt
                    ? task.UpdatedAt
                    : task.CreatedAt
            };
        }

        public async Task<bool> DeleteLatestTaskByUserIdAsync(int userId)
        {
            var latestTask = await _context.Tasks
                .Where(t => t.UserId == userId)
                .OrderByDescending(t =>
                    t.UpdatedAt > t.CreatedAt ? t.UpdatedAt : t.CreatedAt)
                .FirstOrDefaultAsync();

            if (latestTask == null)
            {
                return false;
            }

            _context.Tasks.Remove(latestTask);

            await _context.SaveChangesAsync();

            return true;
        }
        
    }
}