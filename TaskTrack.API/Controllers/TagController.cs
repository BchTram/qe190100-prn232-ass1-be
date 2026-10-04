using Microsoft.AspNetCore.Mvc;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Interfaces;
using Microsoft.Extensions.Logging;


namespace TaskTrack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagController : ControllerBase
{
    private readonly ITagService _tagService;
    private readonly ILogger<TagController> _logger;

    public TagController(ITagService tagService, ILogger<TagController> logger)
    {
        _tagService = tagService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagResponse>>> GetAll()
    {
        var tags = await _tagService.GetAllAsync();
        return Ok(tags);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TagResponse>> GetById(int id)
    {
        var tag = await _tagService.GetByIdAsync(id);

        if (tag is null)
        {
            return NotFound();
        }

        return Ok(tag);
    }

    [HttpPost]
    public async Task<ActionResult<TagResponse>> Create([FromBody] TagCreateRequest request)
    {
        try
        {
            var createdTag = await _tagService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdTag.TagId },
                createdTag);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TagResponse>> Update(int id, [FromBody] TagUpdateRequest request)
    {
        try
        {
            var updatedTag = await _tagService.UpdateAsync(id, request);

            if (updatedTag is null)
            {
                return NotFound();
            }

            return Ok(updatedTag);
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
            var isDeleted = await _tagService.DeleteAsync(id);

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
                "Delete Tag {TagId} rejected. ExceptionType={ExceptionType}, Message={Message}, InnerException={InnerException}, StackTrace={StackTrace}",
                id,
                ex.GetType().FullName,
                ex.Message,
                ex.InnerException?.ToString(),
                ex.StackTrace);

            return Conflict(ex.Message);
        }
    }
}