namespace ExamProject.Entities
{
    public class AdminLatestTaskDto
    {
        public int UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;

        public int TaskId { get; set; }
        public string Title { get; set; } = string.Empty;

        public bool IsComplete { get; set; }

        public DateTime LastActivity { get; set; }
    }
}