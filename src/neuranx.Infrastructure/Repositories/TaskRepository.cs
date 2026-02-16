using Microsoft.EntityFrameworkCore;
using neuranx.Application.Common.Models;
using neuranx.Application.Interfaces;
using neuranx.Domain.Entities;
using neuranx.Infrastructure.Persistence;

namespace neuranx.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly ApplicationDbContext _context;

    public TaskRepository(ApplicationDbContext context) { _context = context; }

    public async Task<TaskItem> AddTaskAsync(TaskItem task)
    {
        try
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task DeleteTaskAsync(TaskItem task)
    {
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
    }

    public async Task<TaskList?> GetTaskByIdAsync(int id, string userId)
    {
        return await _context.Tasks.Where(x => x.Id == id)
            .Select(p => new TaskList
            {
                Name = p.AssignedUser.UserName,
                Tasks = p
            }).FirstOrDefaultAsync();
    }

    public async Task<PaginatedResult<TaskList>> GetTasksByUserIdAsync(string userId, int pageNumber, int pageSize)
    {
        var query = _context.Tasks.Where(t => t.AssignedToUserId == Guid.Parse(userId));
        var count = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(p => p.AssignedUser)
            .Select(p => new TaskList
            {
                Name = p.AssignedUser.UserName,
                Tasks = p,
            }).ToListAsync();

        return PaginatedResult<TaskList>.Create(items, pageNumber, pageSize, count);
    }

    public async Task<PaginatedResult<TaskList>> GetTasksByAsync(int pageNumber, int pageSize)
    {
        var query = _context.Tasks;
        var count = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).Include(p => p.AssignedUser)
            .Select(p => new TaskList
            {
                Name = p.AssignedUser.UserName,
                Tasks = p,
            }).ToListAsync();
        return PaginatedResult<TaskList>.Create(items, pageNumber, pageSize, count);
    }

    public async Task UpdateTaskAsync(TaskItem task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
    }
}

