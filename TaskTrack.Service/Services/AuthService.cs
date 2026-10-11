using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskTrack.Repo.Interfaces;
using TaskTrack.Repo.Models;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Exceptions;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.Service.Services;

public class AuthService : IAuthService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IAccountRepository accountRepository, IConfiguration configuration)
    {
        _accountRepository = accountRepository;
        _configuration = configuration;
    }

    public async Task<AccountResponse> RegisterAsync(RegisterRequest request)
    {
        var email = NormalizeEmail(request.Email);
        if (await _accountRepository.EmailExistsAsync(email))
        {
            throw new DuplicateEmailException();
        }

        var account = new SystemAccount
        {
            FullName = request.FullName.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = 0,
            CreatedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        };

        await _accountRepository.AddAsync(account);
        return MapToResponse(account);
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var account = await _accountRepository.GetByEmailAsync(NormalizeEmail(request.Email));
        if (account is null || !BCrypt.Net.BCrypt.Verify(request.Password, account.PasswordHash))
        {
            return null;
        }

        var expiresAt = DateTime.UtcNow.AddHours(24);
        var secret = _configuration["Jwt:Secret"]!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims:
            [
                new Claim("AccountID", account.AccountId.ToString()),
                new Claim("Email", account.Email),
                new Claim("Role", account.Role.ToString())
            ],
            expires: expiresAt,
            signingCredentials: credentials);

        return new LoginResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt,
            Account = MapToResponse(account)
        };
    }

    internal static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    internal static AccountResponse MapToResponse(SystemAccount account) => new()
    {
        AccountId = account.AccountId,
        FullName = account.FullName,
        Email = account.Email,
        Role = account.Role,
        CreatedDate = account.CreatedDate
    };
}