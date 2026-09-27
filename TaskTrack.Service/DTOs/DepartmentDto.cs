using System.ComponentModel.DataAnnotations;

namespace TaskTrack.Service.DTOs;

public class DepartmentCreateRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string DepartmentName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? DepartmentDescription { get; set; }
}

public class DepartmentUpdateRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string DepartmentName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? DepartmentDescription { get; set; }

    public bool IsActive { get; set; }
}

public class DepartmentResponse
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string? DepartmentDescription { get; set; }
    public bool IsActive { get; set; }
}
