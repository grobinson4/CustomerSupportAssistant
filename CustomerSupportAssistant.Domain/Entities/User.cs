namespace CustomerSupportAssistant.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string HashedPassword { get; set; }
    public string Name { get; set; }

    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
