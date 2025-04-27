using CustomerSupportAssistant.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerSupportAssistant.Persistence.Repositories;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly AppDbContext _dbContext;

    public TaskItemRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
    {
        return await _dbContext.TaskItems.ToListAsync();
    }

    public async Task<TaskItem?> GetTaskByIdAsync(int id)
    {
        return await _dbContext.TaskItems.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task AddTaskAsync(TaskItem taskItem)
    {
        await _dbContext.TaskItems.AddAsync(taskItem);
        await _dbContext.SaveChangesAsync();
    }
}
