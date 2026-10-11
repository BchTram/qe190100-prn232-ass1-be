using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Interfaces;

public interface IAccountService
{
    Task<IEnumerable<AccountResponse>> GetAllAsync();
    Task<AccountResponse?> GetByIdAsync(int id);
    Task<AccountResponse?> UpdateAsync(int id, UpdateAccountRequest request);
    Task DeleteAsync(int id);
    Task EnsureSeedAdminAsync(string email, string password);
}