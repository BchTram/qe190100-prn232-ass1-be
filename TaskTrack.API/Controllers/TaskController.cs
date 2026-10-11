using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskResponse>>> GetAll()
    {
        var tasks = await _taskService.GetAllAsync();
        return Ok(tasks);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskResponse>> GetById(int id)
    {
        var task = await _taskService.GetByIdAsync(id);
        if (task is null)
        {
            return NotFound();
        }

        return Ok(task);
    }

    [HttpGet("project/{projectId:int}")]
    public async Task<ActionResult<IEnumerable<TaskResponse>>> GetByProjectId(int projectId)
    {
        try
        {
            var tasks = await _taskService.GetByProjectIdAsync(projectId);
            return Ok(tasks);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<TaskResponse>>> Search(
        [FromQuery] string? keyword,
        [FromQuery] short? status,
        [FromQuery] short? priority,
        [FromQuery] int? projectId)
    {
        try
        {
            var tasks = await _taskService.SearchAsync(keyword, status, priority, projectId);
            return Ok(tasks);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Authorize(Policy = "AuthenticatedUser")]
    public async Task<ActionResult<TaskResponse>> Create([FromBody] TaskCreateRequest request)
    {
        try
        {
            if (!int.TryParse(User.FindFirstValue("AccountID"), out var creatorAccountId))
            {
                return Unauthorized();
            }

            var createdTask = await _taskService.CreateAsync(request, creatorAccountId);
            return CreatedAtAction(nameof(GetById), new { id = createdTask.TaskId }, createdTask);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "AuthenticatedUser")]
    public async Task<ActionResult<TaskResponse>> Update(int id, [FromBody] TaskUpdateRequest request)
    {
        try
        {
            var updatedTask = await _taskService.UpdateAsync(id, request);
            if (updatedTask is null)
            {
                return NotFound();
            }

            return Ok(updatedTask);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AuthenticatedUser")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var isDeleted = await _taskService.DeleteAsync(id);
            if (!isDeleted)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
