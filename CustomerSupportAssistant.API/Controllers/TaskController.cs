using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CustomerSupportAssistant.Persistence.Repositories;
using CustomerSupportAssistant.Domain.Entities;

namespace CustomerSupportAssistant.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Require EntraID authenticated users
public class TasksController : ControllerBase
{
    private readonly ITaskItemRepository _taskItemRepository;

    public TasksController(ITaskItemRepository taskItemRepository)
    {
        _taskItemRepository = taskItemRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTasks()
    {
        var tasks = await _taskItemRepository.GetAllTasksAsync();
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskById(int id)
    {
        var task = await _taskItemRepository.GetTaskByIdAsync(id);
        if (task == null)
            return NotFound();

        return Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(TaskItem taskItem)
    {
        await _taskItemRepository.AddTaskAsync(taskItem);
        return CreatedAtAction(nameof(GetTaskById), new { id = taskItem.Id }, taskItem);
    }
}
