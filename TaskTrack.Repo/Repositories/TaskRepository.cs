using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Interfaces;
using TaskTrack.Repo.Models;
using TaskEntity = TaskTrack.Repo.Models.Task;

namespace TaskTrack.Repo.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly Prn232PostgresContext _context;

    public TaskRepository(Prn232PostgresContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskEntity>> GetAllAsync()
    {
        return await _context.Tasks
            .AsNoTracking()
            .Include(t => t.Project)
            .Include(t => t.Tags)
            .Where(t => t.IsActive)
            .OrderBy(t => t.CreatedDate)
            .ToListAsync();
    }

    public async Task<TaskEntity?> GetByIdAsync(int id)
    {
        return await _context.Tasks
            .AsNoTracking()
            .Include(t => t.Project)
            .Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.TaskId == id && t.IsActive);
    }

    public async Task<IEnumerable<TaskEntity>> GetByProjectIdAsync(int projectId)
    {
        return await _context.Tasks
            .AsNoTracking()
            .Include(t => t.Project)
            .Include(t => t.Tags)
            .Where(t => t.ProjectId == projectId && t.IsActive)
            .OrderBy(t => t.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskEntity>> SearchAsync(string? keyword, short? status, short? priority, int? projectId)
    {
        var normalizedKeyword = keyword?.Trim();

        var query = _context.Tasks
            .AsNoTracking()
            .Include(t => t.Project)
            .Include(t => t.Tags)
            .Where(t => t.IsActive);

        if (!string.IsNullOrWhiteSpace(normalizedKeyword))
        {
            var lowerKeyword = normalizedKeyword.ToLower();
            query = query.Where(t =>
                t.Title.ToLower().Contains(lowerKeyword) ||
                (t.Description != null && t.Description.ToLower().Contains(lowerKeyword)));
        }

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(t => t.Priority == priority.Value);
        }

        if (projectId.HasValue)
        {
            query = query.Where(t => t.ProjectId == projectId.Value);
        }

        return await query
            .OrderBy(t => t.CreatedDate)
            .ToListAsync();
    }

    public async Task<TaskEntity> AddAsync(TaskEntity task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<TaskEntity> UpdateAsync(TaskEntity task)
    {
        _context.Entry(task).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var taskEntity = await _context.Tasks.FirstOrDefaultAsync(t => t.TaskId == id && t.IsActive);
        if (taskEntity is null)
        {
            return false;
        }

        taskEntity.IsActive = false;
        taskEntity.ModifiedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}
