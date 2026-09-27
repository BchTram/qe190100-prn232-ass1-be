using TaskTrack.Repo.Interfaces;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Interfaces;
using TaskEntity = TaskTrack.Repo.Models.Task;

namespace TaskTrack.Service.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;

    public TaskService(ITaskRepository taskRepository, IProjectRepository projectRepository)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
    }

    public async Task<IEnumerable<TaskResponse>> GetAllAsync()
    {
        var tasks = await _taskRepository.GetAllAsync();
        return tasks.Select(MapToResponse);
    }

    public async Task<TaskResponse?> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Task ID must be greater than zero.");
        }

        var task = await _taskRepository.GetByIdAsync(id);
        return task is null ? null : MapToResponse(task);
    }

    public async Task<IEnumerable<TaskResponse>> GetByProjectIdAsync(int projectId)
    {
        if (projectId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(projectId), "Project ID must be greater than zero.");
        }

        var tasks = await _taskRepository.GetByProjectIdAsync(projectId);
        return tasks.Select(MapToResponse);
    }

    public async Task<IEnumerable<TaskResponse>> SearchAsync(string? keyword, short? status, short? priority, int? projectId)
    {
        if (projectId.HasValue && projectId.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(projectId), "Project ID must be greater than zero.");
        }

        var tasks = await _taskRepository.SearchAsync(keyword, status, priority, projectId);
        return tasks.Select(MapToResponse);
    }

    public async Task<TaskResponse> CreateAsync(TaskCreateRequest request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var projectExists = await _projectRepository.GetByIdAsync(request.ProjectId);
        if (projectExists is null)
        {
            throw new InvalidOperationException("Task must belong to an existing project.");
        }

        var task = new TaskEntity
        {
            Title = request.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            Status = request.Status,
            Priority = request.Priority,
            DueDate = request.DueDate,
            ProjectId = request.ProjectId,
            IsActive = true,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = null
        };

        var createdTask = await _taskRepository.AddAsync(task);
        return MapToResponse(createdTask);
    }

    public async Task<TaskResponse?> UpdateAsync(int id, TaskUpdateRequest request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Task ID must be greater than zero.");
        }

        var existingTask = await _taskRepository.GetByIdAsync(id);
        if (existingTask is null)
        {
            return null;
        }

        var projectExists = await _projectRepository.GetByIdAsync(request.ProjectId);
        if (projectExists is null)
        {
            throw new InvalidOperationException("Task must belong to an existing project.");
        }

        existingTask.Title = request.Title.Trim();
        existingTask.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        existingTask.Status = request.Status;
        existingTask.Priority = request.Priority;
        existingTask.DueDate = request.DueDate;
        existingTask.ProjectId = request.ProjectId;
        existingTask.ModifiedDate = DateTime.UtcNow;
        existingTask.IsActive = request.IsActive;

        var updatedTask = await _taskRepository.UpdateAsync(existingTask);
        return MapToResponse(updatedTask);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Task ID must be greater than zero.");
        }

        return await _taskRepository.SoftDeleteAsync(id);
    }

    private static TaskResponse MapToResponse(TaskEntity task)
    {
        return new TaskResponse
        {
            TaskId = task.TaskId,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            DueDate = task.DueDate,
            ProjectId = task.ProjectId,
            IsActive = task.IsActive,
            CreatedDate = task.CreatedDate,
            ModifiedDate = task.ModifiedDate
        };
    }
}
