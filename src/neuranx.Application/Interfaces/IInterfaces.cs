using neuranx.Application.Common.Models;
using neuranx.Domain;
using neuranx.Domain.Entities;

namespace neuranx.Application.Interfaces;

public interface ITaskRepository
{
    Task<PaginatedResult<TaskList>> GetTasksByUserIdAsync(string userId, int pageNumber, int pageSize);
    Task<PaginatedResult<TaskList>> GetTasksByAsync(int pageNumber, int pageSize);
    Task<TaskList?> GetTaskByIdAsync(int id, string userId);
    Task<TaskItem> AddTaskAsync(TaskItem task);
    Task UpdateTaskAsync(TaskItem task);
    Task DeleteTaskAsync(TaskItem task);
}

public interface IIdentityService
{
    Task<bool> RegisterUserAsync(string userName, string email, string password);
    Task<AuthResult> LoginUserAsync(string email, string password);
    Task<AuthResult> RefreshTokenAsync(string token, string refreshToken);

    Task<object> GetAllUsersAsync();
}
