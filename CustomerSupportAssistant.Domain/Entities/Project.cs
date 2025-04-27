namespace CustomerSupportAssistant.Domain.Entities;

public class Project
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
