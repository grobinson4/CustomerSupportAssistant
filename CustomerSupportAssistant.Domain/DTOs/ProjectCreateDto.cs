namespace CustomerSupportAssistant.Domain.Dtos
{
    public class ProjectCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; } // Just link to user by ID
    }
}
