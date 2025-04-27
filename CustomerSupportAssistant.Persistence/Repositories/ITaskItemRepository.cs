using CustomerSupportAssistant.Domain.Entities;

namespace CustomerSupportAssistant.Persistence.Repositories;

public interface ITaskItemRepository
{
    Task<IEnumerable<TaskItem>> GetAllTasksAsync();
    Task<TaskItem?> GetTaskByIdAsync(int id);
    Task AddTaskAsync(TaskItem taskItem);
}
