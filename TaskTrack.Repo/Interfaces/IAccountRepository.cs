using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Interfaces;

public interface IAccountRepository
{
    Task<IEnumerable<SystemAccount>> GetAllAsync();
    Task<SystemAccount?> GetByIdAsync(int id);
    Task<SystemAccount?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email, int? excludingAccountId = null);
    Task<bool> HasCreatedTasksAsync(int accountId);
    Task<SystemAccount> AddAsync(SystemAccount account);
    Task<SystemAccount> UpdateAsync(SystemAccount account);
    System.Threading.Tasks.Task DeleteAsync(SystemAccount account);
}