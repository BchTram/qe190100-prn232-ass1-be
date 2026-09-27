using System.ComponentModel.DataAnnotations;

namespace TaskTrack.Service.DTOs;

public class TagCreateRequest
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string TagName { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Color { get; set; }
}

public class TagUpdateRequest
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string TagName { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Color { get; set; }
}

public class TagResponse
{
    public int TagId { get; set; }
    public string TagName { get; set; } = string.Empty;
    public string? Color { get; set; }
}
