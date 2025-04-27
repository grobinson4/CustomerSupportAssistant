using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CustomerSupportAssistant.Persistence.Repositories;
using CustomerSupportAssistant.Domain.Entities;

namespace CustomerSupportAssistant.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Require EntraID authenticated users
public class ProjectsController : ControllerBase
{
    private readonly IProjectRepository _projectRepository;

    public ProjectsController(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
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
    public async Task<IActionResult> CreateProject(Project project)
    {
        await _projectRepository.AddProjectAsync(project);
        return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, project);
    }
}
