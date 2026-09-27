using System.ComponentModel.DataAnnotations;

namespace TaskTrack.Service.DTOs;

public class TaskCreateRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Range(0, short.MaxValue)]
    public short Status { get; set; }

    [Range(0, short.MaxValue)]
    public short Priority { get; set; }

    public DateOnly? DueDate { get; set; }

    [Range(1, int.MaxValue)]
    public int ProjectId { get; set; }
}

public class TaskUpdateRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Range(0, short.MaxValue)]
    public short Status { get; set; }

    [Range(0, short.MaxValue)]
    public short Priority { get; set; }

    public DateOnly? DueDate { get; set; }

    [Range(1, int.MaxValue)]
    public int ProjectId { get; set; }

    public bool IsActive { get; set; }
}

public class TaskResponse
{
    public int TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public short Status { get; set; }
    public short Priority { get; set; }
    public DateOnly? DueDate { get; set; }
    public int ProjectId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
}
