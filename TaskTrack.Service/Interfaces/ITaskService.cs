using TaskTrack.Service.DTOs;
using TaskEntity = TaskTrack.Repo.Models.Task;

namespace TaskTrack.Service.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskResponse>> GetAllAsync();
    Task<TaskResponse?> GetByIdAsync(int id);
    Task<IEnumerable<TaskResponse>> GetByProjectIdAsync(int projectId);
    Task<IEnumerable<TaskResponse>> SearchAsync(string? keyword, short? status, short? priority, int? projectId);
    Task<TaskResponse> CreateAsync(TaskCreateRequest request);
    Task<TaskResponse?> UpdateAsync(int id, TaskUpdateRequest request);
    Task<bool> DeleteAsync(int id);
}
