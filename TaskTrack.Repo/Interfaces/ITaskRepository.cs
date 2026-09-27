using TaskEntity = TaskTrack.Repo.Models.Task;

namespace TaskTrack.Repo.Interfaces;

public interface ITaskRepository
{
    Task<IEnumerable<TaskEntity>> GetAllAsync();
    Task<TaskEntity?> GetByIdAsync(int id);
    Task<IEnumerable<TaskEntity>> GetByProjectIdAsync(int projectId);
    Task<IEnumerable<TaskEntity>> SearchAsync(string? keyword, short? status, short? priority, int? projectId);
    Task<TaskEntity> AddAsync(TaskEntity task);
    Task<TaskEntity> UpdateAsync(TaskEntity task);
    Task<bool> SoftDeleteAsync(int id);
}
