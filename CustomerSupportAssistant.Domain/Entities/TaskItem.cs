namespace CustomerSupportAssistant.Domain.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public DateTime DueDate { get; set; }
}
