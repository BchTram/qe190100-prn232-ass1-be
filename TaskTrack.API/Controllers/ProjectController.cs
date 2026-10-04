using Microsoft.AspNetCore.Mvc;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Interfaces;
using Microsoft.Extensions.Logging;


namespace TaskTrack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly ILogger<ProjectController> _logger;

    public ProjectController(IProjectService projectService, ILogger<ProjectController> logger)
    {
        _projectService = projectService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetAll()
    {
        var projects = await _projectService.GetAllAsync();
        return Ok(projects);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectResponse>> GetById(int id)
    {
        var project = await _projectService.GetByIdAsync(id);

        if (project is null)
        {
            return NotFound();
        }

        return Ok(project);
    }

    [HttpGet("department/{departmentId:int}")]
    public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetByDepartmentId(int departmentId)
    {
        try
        {
            var projects = await _projectService.GetByDepartmentIdAsync(departmentId);
            return Ok(projects);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProjectResponse>>> Search([FromQuery] string? keyword)
    {
        var projects = await _projectService.SearchAsync(keyword);
        return Ok(projects);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> Create([FromBody] ProjectCreateRequest request)
    {
        try
        {
            var createdProject = await _projectService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdProject.ProjectId },
                createdProject);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProjectResponse>> Update(int id, [FromBody] ProjectUpdateRequest request)
    {
        try
        {
            var updatedProject = await _projectService.UpdateAsync(id, request);

            if (updatedProject is null)
            {
                return NotFound();
            }

            return Ok(updatedProject);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var isDeleted = await _projectService.DeleteAsync(id);

            if (!isDeleted)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(
                ex,
                "Delete Project {ProjectId} rejected. ExceptionType={ExceptionType}, Message={Message}, InnerException={InnerException}, StackTrace={StackTrace}",
                id,
                ex.GetType().FullName,
                ex.Message,
                ex.InnerException?.ToString(),
                ex.StackTrace);

            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}