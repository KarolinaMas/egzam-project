using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using ExamProject.Data;
using ExamProject.Entities;

namespace ExamProject.Services.Tests
{
    public class TaskItemServiceTests
    {
        private readonly Mock<ExamProjectDbContext> contextMock = new();
        private readonly TaskItemService taskItemService;

        public TaskItemServiceTests()
        {
            taskItemService = new TaskItemService(contextMock.Object);
        }


        private static TaskItem MakeTask(int id, int userId, string title = "Task", string description = "Desc")
            => new() { Id = id, UserId = userId, Title = title, Description = description, CreatedAt = DateTime.UtcNow };

        private static CreateTaskItemDto MakeCreateDto(string title = "Task", string description = "Desc")
            => new() { Title = title, Description = description };

        private static UpdateTaskItemDto MakeUpdateDto(string title = "Updated", string description = "Updated Desc", bool isCompleted = false)
            => new() { Title = title, Description = description, IsCompleted = isCompleted };

        private void SetupTasksDbSet(IEnumerable<TaskItem> tasks)
        {
            var mock = tasks.AsQueryable().BuildMockDbSet();
            contextMock.Setup(c => c.Tasks).Returns(mock.Object);
        }


        [Fact]
        public async Task CreateAsync_ValidDto_AddsTaskAndSavesChanges()
        {
            var addedTasks = new List<TaskItem>();
            var mockSet = new List<TaskItem>().AsQueryable().BuildMockDbSet();
            mockSet
                .Setup(s => s.AddAsync(It.IsAny<TaskItem>(), default))
                .Callback<TaskItem, CancellationToken>((t, _) => addedTasks.Add(t))
                .Returns(ValueTask.FromResult((Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<TaskItem>)null!));

            contextMock.Setup(c => c.Tasks).Returns(mockSet.Object);
            contextMock.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            await taskItemService.CreateAsync(userId: 1, MakeCreateDto("Buy Groceries", "Milk and eggs"));

            contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once);
            Assert.Single(addedTasks);
            Assert.Equal("Buy Groceries", addedTasks[0].Title);
            Assert.Equal("Milk and eggs", addedTasks[0].Description);
            Assert.Equal(1, addedTasks[0].UserId);
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
        public async Task CreateAsync_InvalidDto_DoesNotCallSaveChanges()
        {
            try { await taskItemService.CreateAsync(1, MakeCreateDto(title: "")); } catch { }

            contextMock.Verify(c => c.SaveChangesAsync(default), Times.Never);
        }


        [Fact]
        public async Task GetAsync_ExistingTaskForCorrectUser_ReturnsTask()
        {
            var task = MakeTask(id: 1, userId: 1, title: "My Task");
            SetupTasksDbSet([task]);

            var result = await taskItemService.GetAsync(userId: 1, id: 1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("My Task", result.Title);
        }

        [Fact]
        public async Task GetAsync_NonExistentId_ReturnsNull()
        {
            SetupTasksDbSet([]);

            var result = await taskItemService.GetAsync(userId: 1, id: 999);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAsync_TaskBelongsToOtherUser_ReturnsNull()
        {
            var task = MakeTask(id: 1, userId: 2);
            SetupTasksDbSet([task]);

            var result = await taskItemService.GetAsync(userId: 1, id: 1);

            Assert.Null(result);
        }


        [Fact]
        public async Task GetAllAsync_ReturnsOnlyTasksForGivenUser()
        {
            var tasks = new List<TaskItem>
            {
                MakeTask(id: 1, userId: 1, title: "U1-A"),
                MakeTask(id: 2, userId: 1, title: "U1-B"),
                MakeTask(id: 3, userId: 2, title: "U2-A"),
            };
            SetupTasksDbSet(tasks);

            var result = await taskItemService.GetAllAsync(userId: 1, page: 1, itemsPerPage: 10);

            Assert.Equal(2, result.Count);
            Assert.All(result, t => Assert.Equal(1, t.UserId));
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList_WhenUserHasNoTasks()
        {
            SetupTasksDbSet([]);

            var result = await taskItemService.GetAllAsync(userId: 99, page: 1, itemsPerPage: 10);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_PaginatesCorrectly_FirstPage()
        {
            var tasks = Enumerable.Range(1, 5)
                .Select(i => MakeTask(id: i, userId: 1, title: $"Task {i}"))
                .ToList();
            SetupTasksDbSet(tasks);

            var result = await taskItemService.GetAllAsync(userId: 1, page: 1, itemsPerPage: 3);

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task GetAllAsync_PaginatesCorrectly_SecondPage()
        {
            var tasks = Enumerable.Range(1, 5)
                .Select(i => MakeTask(id: i, userId: 1, title: $"Task {i}"))
                .ToList();
            SetupTasksDbSet(tasks);

            var result = await taskItemService.GetAllAsync(userId: 1, page: 2, itemsPerPage: 3);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetAllAsync_InvalidPage_DefaultsToPageOne()
        {
            var tasks = Enumerable.Range(1, 3)
                .Select(i => MakeTask(id: i, userId: 1))
                .ToList();
            SetupTasksDbSet(tasks);

            var result = await taskItemService.GetAllAsync(userId: 1, page: 0, itemsPerPage: 10);

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task GetAllAsync_InvalidItemsPerPage_DefaultsToTen()
        {
            var tasks = Enumerable.Range(1, 12)
                .Select(i => MakeTask(id: i, userId: 1))
                .ToList();
            SetupTasksDbSet(tasks);

            var result = await taskItemService.GetAllAsync(userId: 1, page: 1, itemsPerPage: 0);

            Assert.Equal(10, result.Count);
        }

        [Fact]
        public async Task GetAllAsync_ResultsAreOrderedById()
        {
            var tasks = new List<TaskItem>
            {
                MakeTask(id: 3, userId: 1),
                MakeTask(id: 1, userId: 1),
                MakeTask(id: 2, userId: 1),
            };
            SetupTasksDbSet(tasks);

            var result = await taskItemService.GetAllAsync(userId: 1, page: 1, itemsPerPage: 10);

            Assert.Equal([1, 2, 3], result.Select(t => t.Id));
        }

    
        [Fact]
        public async Task UpdateAsync_ExistingTask_ReturnsUpdatedTask()
        {
            var task = MakeTask(id: 1, userId: 1, title: "Old Title", description: "Old Desc");
            SetupTasksDbSet([task]);
            contextMock.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            var result = await taskItemService.UpdateAsync(userId: 1, id: 1, MakeUpdateDto("New Title", "New Desc", true));

            Assert.NotNull(result);
            Assert.Equal("New Title", result.Title);
            Assert.Equal("New Desc", result.Description);
            Assert.True(result.IsComplete);
        }

        [Fact]
        public async Task UpdateAsync_ExistingTask_CallsSaveChanges()
        {
            var task = MakeTask(id: 1, userId: 1);
            SetupTasksDbSet([task]);
            contextMock.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            await taskItemService.UpdateAsync(userId: 1, id: 1, MakeUpdateDto());

            contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_NonExistentId_ReturnsNull()
        {
            SetupTasksDbSet([]);

            var result = await taskItemService.UpdateAsync(userId: 1, id: 999, MakeUpdateDto());

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_NonExistentId_DoesNotCallSaveChanges()
        {
            SetupTasksDbSet([]);

            await taskItemService.UpdateAsync(userId: 1, id: 999, MakeUpdateDto());

            contextMock.Verify(c => c.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_TaskBelongsToOtherUser_ReturnsNull()
        {
            var task = MakeTask(id: 1, userId: 2);
            SetupTasksDbSet([task]);

            var result = await taskItemService.UpdateAsync(userId: 1, id: 1, MakeUpdateDto());

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_TaskBelongsToOtherUser_DoesNotCallSaveChanges()
        {
            var task = MakeTask(id: 1, userId: 2);
            SetupTasksDbSet([task]);

            await taskItemService.UpdateAsync(userId: 1, id: 1, MakeUpdateDto());

            contextMock.Verify(c => c.SaveChangesAsync(default), Times.Never);
        }


        [Fact]
        public async Task DeleteAsync_ExistingTask_ReturnsTrue()
        {
            var task = MakeTask(id: 1, userId: 1);
            SetupTasksDbSet([task]);

            var result = await taskItemService.DeleteAsync(userId: 1, id: 1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_NonExistentId_ReturnsFalse()
        {
            SetupTasksDbSet([]);

            var result = await taskItemService.DeleteAsync(userId: 1, id: 999);

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_TaskBelongsToOtherUser_ReturnsFalse()
        {
            var task = MakeTask(id: 1, userId: 2);
            SetupTasksDbSet([task]);

            var result = await taskItemService.DeleteAsync(userId: 1, id: 1);

            Assert.False(result);
        }
    }
}