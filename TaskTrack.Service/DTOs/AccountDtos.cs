using System.ComponentModel.DataAnnotations;

namespace TaskTrack.Service.DTOs;

public class AccountResponse
{
    public int AccountId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Role { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class UpdateAccountRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(320)]
    public string Email { get; set; } = string.Empty;

    [Range(0, 1)]
    public int Role { get; set; }
}