using Microsoft.AspNetCore.Mvc;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Interfaces;
using Microsoft.Extensions.Logging;

namespace TaskTrack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentController : ControllerBase
{
    private readonly IDepartmentService _departmentService;
    private readonly ILogger<DepartmentController> _logger;

    public DepartmentController(
        IDepartmentService departmentService,
        ILogger<DepartmentController> logger)
    {
        _departmentService = departmentService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentResponse>>> GetAll()
    {
        var departments = await _departmentService.GetAllAsync();
        return Ok(departments);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DepartmentResponse>> GetById(int id)
    {
        var department = await _departmentService.GetByIdAsync(id);

        if (department is null)
        {
            return NotFound();
        }

        return Ok(department);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<DepartmentResponse>>> Search([FromQuery] string? keyword)
    {
        var departments = await _departmentService.SearchAsync(keyword);
        return Ok(departments);
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentResponse>> Create([FromBody] DepartmentCreateRequest request)
    {
        try
        {
            var createdDepartment = await _departmentService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdDepartment.DepartmentId },
                createdDepartment);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<DepartmentResponse>> Update(
        int id,
        [FromBody] DepartmentUpdateRequest request)
    {
        try
        {
            var updatedDepartment = await _departmentService.UpdateAsync(id, request);

            if (updatedDepartment is null)
            {
                return NotFound();
            }

            return Ok(updatedDepartment);
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
            var isDeleted = await _departmentService.DeleteAsync(id);

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
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(
                ex,
                "Delete Department {DepartmentId} rejected. ExceptionType={ExceptionType}, Message={Message}, InnerException={InnerException}, StackTrace={StackTrace}",
                id,
                ex.GetType().FullName,
                ex.Message,
                ex.InnerException?.ToString(),
                ex.StackTrace);

            return Conflict(ex.Message);
        }
    }
}