using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CustomerSupportAssistant.Persistence.Repositories;
using CustomerSupportAssistant.Domain.Entities;
using CustomerSupportAssistant.Persistence;
using CustomerSupportAssistant.Infrastructure.Services;

namespace CustomerSupportAssistant.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Require EntraID authenticated users
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly OpenAIService _openAIService;
    private readonly IProjectRepository _projectRepository;

    public ProjectsController(IProjectRepository projectRepository, AppDbContext dbContext, OpenAIService openAIService)
    {
        _projectRepository = projectRepository;
        _dbContext = dbContext;
        _openAIService = openAIService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProjects()
    {
        var projects = await _projectRepository.GetAllProjectsAsync();
        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProjectById(int id)
    {
        var project = await _projectRepository.GetProjectByIdAsync(id);
        if (project == null)
            return NotFound();

        return Ok(project);
    }

   [HttpPost]
public async Task<ActionResult<Project>> CreateProject([FromBody] Project project)
{
    if (string.IsNullOrEmpty(project.Title) || string.IsNullOrEmpty(project.Description))
    {
        return BadRequest("Project title and description are required.");
    }

    // 🔥 Call OpenAI to generate a project plan
    var plan = await _openAIService.SummarizeProjectAsync(project.Title, project.Description);

    // Create a new project entity
    var newProject = new Project
    {
        Title = project.Title,
        Description = project.Description,
        UserId = project.UserId,
        AiPlan = plan
    };

    _dbContext.Projects.Add(newProject);
    await _dbContext.SaveChangesAsync();

    return CreatedAtAction(nameof(GetProjectById), new { id = newProject.Id }, newProject);
}
}
