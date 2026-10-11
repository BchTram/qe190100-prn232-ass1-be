using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Interfaces;
using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly Prn232PostgresContext _context;

    public AccountRepository(Prn232PostgresContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SystemAccount>> GetAllAsync() =>
        await _context.SystemAccounts.AsNoTracking().OrderBy(account => account.AccountId).ToListAsync();

    public Task<SystemAccount?> GetByIdAsync(int id) =>
        _context.SystemAccounts.FirstOrDefaultAsync(account => account.AccountId == id);

    public Task<SystemAccount?> GetByEmailAsync(string email) =>
        _context.SystemAccounts.FirstOrDefaultAsync(account => account.Email == email);

    public Task<bool> EmailExistsAsync(string email, int? excludingAccountId = null) =>
        _context.SystemAccounts.AnyAsync(account =>
            account.Email == email && (!excludingAccountId.HasValue || account.AccountId != excludingAccountId.Value));

    public Task<bool> HasCreatedTasksAsync(int accountId) =>
        _context.Tasks.AnyAsync(task => task.CreatedByAccountId == accountId);

    public async Task<SystemAccount> AddAsync(SystemAccount account)
    {
        _context.SystemAccounts.Add(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task<SystemAccount> UpdateAsync(SystemAccount account)
    {
        _context.SystemAccounts.Update(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async System.Threading.Tasks.Task DeleteAsync(SystemAccount account)
    {
        _context.SystemAccounts.Remove(account);
        await _context.SaveChangesAsync();
    }
}