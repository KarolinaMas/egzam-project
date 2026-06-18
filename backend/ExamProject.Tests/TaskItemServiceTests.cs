using ExamProject.Data;
using ExamProject.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExamProject.Services.Tests
{
    public class TaskItemServiceTests : IDisposable
    {
        private readonly ExamProjectDbContext _context;
        private readonly TaskItemService taskItemService;

        public TaskItemServiceTests()
        {
            var options = new DbContextOptionsBuilder<ExamProjectDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ExamProjectDbContext(options);
            taskItemService = new TaskItemService(_context);
        }

        public void Dispose() => _context.Dispose();


        private static CreateTaskItemDto MakeCreateDto(string title = "Task", string description = "Desc")
            => new() { Title = title, Description = description };

        private static UpdateTaskItemDto MakeUpdateDto(string title = "Updated", string description = "Updated Desc", bool isCompleted = false)
            => new() { Title = title, Description = description, IsCompleted = isCompleted };

        private async Task<int> SeedTask(int userId, string title = "Task", string description = "Desc")
        {
            return await taskItemService.CreateAsync(userId, MakeCreateDto(title, description));
        }


        [Fact]
        public async Task CreateAsync_ValidDto_PersistsTaskInDatabase()
        {
            var id = await taskItemService.CreateAsync(userId: 1, MakeCreateDto("Buy Groceries", "Milk and eggs"));

            var saved = await _context.Tasks.FindAsync(id);
            Assert.NotNull(saved);
            Assert.Equal("Buy Groceries", saved.Title);
            Assert.Equal("Milk and eggs", saved.Description);
            Assert.Equal(1, saved.UserId);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task CreateAsync_EmptyOrWhitespaceTitle_ThrowsArgumentException(string? title)
        {
            await Assert.ThrowsAsync<ArgumentException>(
                () => taskItemService.CreateAsync(1, MakeCreateDto(title: title!))
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task CreateAsync_EmptyOrWhitespaceDescription_ThrowsArgumentException(string? description)
        {
            await Assert.ThrowsAsync<ArgumentException>(
                () => taskItemService.CreateAsync(1, MakeCreateDto(description: description!))
            );
        }

        [Fact]
        public async Task CreateAsync_InvalidDto_DoesNotPersistAnything()
        {
            try { await taskItemService.CreateAsync(1, MakeCreateDto(title: "")); } catch { }

            Assert.Empty(_context.Tasks);
        }


        [Fact]
        public async Task GetAsync_ExistingTaskForCorrectUser_ReturnsTask()
        {
            var id = await SeedTask(userId: 1, title: "My Task");

            var result = await taskItemService.GetAsync(userId: 1, id);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("My Task", result.Title);
        }

        [Fact]
        public async Task GetAsync_NonExistentId_ReturnsNull()
        {
            var result = await taskItemService.GetAsync(userId: 1, id: 999);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAsync_TaskBelongsToOtherUser_ReturnsNull()
        {
            var id = await SeedTask(userId: 2);

            var result = await taskItemService.GetAsync(userId: 1, id);

            Assert.Null(result);
        }

    
        [Fact]
        public async Task GetAllAsync_ReturnsOnlyTasksForGivenUser()
        {
            await SeedTask(userId: 1, title: "U1-A");
            await SeedTask(userId: 1, title: "U1-B");
            await SeedTask(userId: 2, title: "U2-A");

            var result = await taskItemService.GetAllAsync(userId: 1, page: 1, itemsPerPage: 10);

            Assert.Equal(2, result.Count);
            Assert.All(result, t => Assert.Equal(1, t.UserId));
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList_WhenUserHasNoTasks()
        {
            var result = await taskItemService.GetAllAsync(userId: 99, page: 1, itemsPerPage: 10);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_PaginatesCorrectly_FirstPage()
        {
            for (var i = 0; i < 5; i++)
                await SeedTask(userId: 1, title: $"Task {i}");

            var result = await taskItemService.GetAllAsync(userId: 1, page: 1, itemsPerPage: 3);

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task GetAllAsync_PaginatesCorrectly_SecondPage()
        {
            for (var i = 0; i < 5; i++)
                await SeedTask(userId: 1, title: $"Task {i}");

            var result = await taskItemService.GetAllAsync(userId: 1, page: 2, itemsPerPage: 3);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetAllAsync_InvalidPage_DefaultsToPageOne()
        {
            for (var i = 0; i < 3; i++)
                await SeedTask(userId: 1);

            var result = await taskItemService.GetAllAsync(userId: 1, page: 0, itemsPerPage: 10);

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task GetAllAsync_InvalidItemsPerPage_DefaultsToTen()
        {
            for (var i = 0; i < 12; i++)
                await SeedTask(userId: 1);

            var result = await taskItemService.GetAllAsync(userId: 1, page: 1, itemsPerPage: 0);

            Assert.Equal(10, result.Count);
        }

        [Fact]
        public async Task GetAllAsync_ResultsAreOrderedById()
        {
            for (var i = 0; i < 5; i++)
                await SeedTask(userId: 1);

            var result = await taskItemService.GetAllAsync(userId: 1, page: 1, itemsPerPage: 10);

            Assert.Equal(result.OrderBy(t => t.Id).Select(t => t.Id), result.Select(t => t.Id));
        }


        [Fact]
        public async Task UpdateAsync_ExistingTask_ReturnsUpdatedTask()
        {
            var id = await SeedTask(userId: 1, title: "Old Title", description: "Old Desc");

            var result = await taskItemService.UpdateAsync(userId: 1, id, MakeUpdateDto("New Title", "New Desc", true));

            Assert.NotNull(result);
            Assert.Equal("New Title", result.Title);
            Assert.Equal("New Desc", result.Description);
            Assert.True(result.IsComplete);
        }

        [Fact]
        public async Task UpdateAsync_ExistingTask_PersistsChanges()
        {
            var id = await SeedTask(userId: 1);

            await taskItemService.UpdateAsync(userId: 1, id, MakeUpdateDto("Persisted", "Persisted Desc", true));

            var saved = await _context.Tasks.FindAsync(id);
            Assert.Equal("Persisted", saved!.Title);
            Assert.True(saved.IsComplete);
        }

        [Fact]
        public async Task UpdateAsync_NonExistentId_ReturnsNull()
        {
            var result = await taskItemService.UpdateAsync(userId: 1, id: 999, MakeUpdateDto());

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_TaskBelongsToOtherUser_ReturnsNull()
        {
            var id = await SeedTask(userId: 2);

            var result = await taskItemService.UpdateAsync(userId: 1, id, MakeUpdateDto());

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_TaskBelongsToOtherUser_DoesNotModifyTask()
        {
            var id = await SeedTask(userId: 2, title: "Original", description: "Original Desc");

            await taskItemService.UpdateAsync(userId: 1, id, MakeUpdateDto("Hacked", "Hacked"));

            var saved = await _context.Tasks.FindAsync(id);
            Assert.Equal("Original", saved!.Title);
        }

    }
}