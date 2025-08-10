using BlogSpace.Shared.Models;

namespace BlogSpace.Server.Services
{
    public interface IAuthService
    {
        Task<User?> GetUserByIdAsync(string userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> CreateUserAsync(User user, string password);
        Task<bool> ValidateCredentialsAsync(string email, string password);
        Task<string> GenerateJwtTokenAsync(User user);
    }
} 