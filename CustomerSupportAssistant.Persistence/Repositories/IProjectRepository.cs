using CustomerSupportAssistant.Domain.Entities;

namespace CustomerSupportAssistant.Persistence.Repositories;

public interface IProjectRepository
{
    Task<IEnumerable<Project>> GetAllProjectsAsync();
    Task<Project?> GetProjectByIdAsync(int id);
    Task AddProjectAsync(Project project);
}
