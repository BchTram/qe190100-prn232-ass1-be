using System.ComponentModel.DataAnnotations;

namespace TaskTrack.Service.DTOs;

public class ProjectCreateRequest : IValidatableObject
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string ProjectName { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required]
    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    [Range(1, int.MaxValue)]
    public int DepartmentId { get; set; }

    [Range(0, short.MaxValue)]
    public short Status { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate.HasValue && EndDate.Value < StartDate)
        {
            yield return new ValidationResult(
                "EndDate must be greater than or equal to StartDate.",
                new[] { nameof(EndDate) });
        }
    }
}

public class ProjectUpdateRequest : IValidatableObject
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string ProjectName { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required]
    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    [Range(1, int.MaxValue)]
    public int DepartmentId { get; set; }

    [Range(0, short.MaxValue)]
    public short Status { get; set; }

    public bool IsActive { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate.HasValue && EndDate.Value < StartDate)
        {
            yield return new ValidationResult(
                "EndDate must be greater than or equal to StartDate.",
                new[] { nameof(EndDate) });
        }
    }
}

public class ProjectResponse
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public short Status { get; set; }
    public int DepartmentId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
}
