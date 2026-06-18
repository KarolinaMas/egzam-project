namespace ExamProject.Entities
{
    public class AdminTaskItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsComplete { get; set; }
        public int UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
    }
}
