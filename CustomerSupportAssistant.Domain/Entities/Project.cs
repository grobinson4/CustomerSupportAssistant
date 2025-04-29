namespace CustomerSupportAssistant.Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int UserId { get; set; }
        public User? User { get; set; } // Navigation property made optional
        public ICollection<TaskItem>? Tasks { get; set; } // Navigation property made optional

        public string? AiPlan { get; set; }
    }
}