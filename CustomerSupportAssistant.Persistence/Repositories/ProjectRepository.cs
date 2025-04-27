using CustomerSupportAssistant.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerSupportAssistant.Persistence.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _dbContext;

    public ProjectRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Project>> GetAllProjectsAsync()
    {
        return await _dbContext.Projects
            .Include(p => p.Tasks) // include tasks if you want
            .ToListAsync();
    }

    public async Task<Project?> GetProjectByIdAsync(int id)
    {
        return await _dbContext.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddProjectAsync(Project project)
    {
        await _dbContext.Projects.AddAsync(project);
        await _dbContext.SaveChangesAsync();
    }
}
