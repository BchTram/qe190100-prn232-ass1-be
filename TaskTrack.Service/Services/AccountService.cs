using TaskTrack.Repo.Interfaces;
using TaskTrack.Repo.Models;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Exceptions;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.Service.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<IEnumerable<AccountResponse>> GetAllAsync() =>
        (await _accountRepository.GetAllAsync()).Select(AuthService.MapToResponse);

    public async Task<AccountResponse?> GetByIdAsync(int id)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        return account is null ? null : AuthService.MapToResponse(account);
    }

    public async Task<AccountResponse?> UpdateAsync(int id, UpdateAccountRequest request)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        if (account is null)
        {
            return null;
        }

        var email = AuthService.NormalizeEmail(request.Email);
        if (await _accountRepository.EmailExistsAsync(email, id))
        {
            throw new DuplicateEmailException();
        }

        account.FullName = request.FullName.Trim();
        account.Email = email;
        account.Role = request.Role;
        await _accountRepository.UpdateAsync(account);
        return AuthService.MapToResponse(account);
    }

    public async System.Threading.Tasks.Task DeleteAsync(int id)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        if (account is null)
        {
            throw new KeyNotFoundException("Account was not found.");
        }

        if (await _accountRepository.HasCreatedTasksAsync(id))
        {
            throw new AccountHasCreatedTasksException();
        }

        await _accountRepository.DeleteAsync(account);
    }

    public async System.Threading.Tasks.Task EnsureSeedAdminAsync(string email, string password)
    {
        var normalizedEmail = AuthService.NormalizeEmail(email);
        if (await _accountRepository.GetByEmailAsync(normalizedEmail) is not null)
        {
            return;
        }

        await _accountRepository.AddAsync(new SystemAccount
        {
            FullName = "System Administrator",
            Email = normalizedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = 1,
            CreatedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        });
    }
}