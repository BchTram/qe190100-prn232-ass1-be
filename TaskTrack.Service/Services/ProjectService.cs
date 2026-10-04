using TaskTrack.Repo.Interfaces;
using TaskTrack.Repo.Models;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Interfaces;
using Microsoft.Extensions.Logging;

namespace TaskTrack.Service.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(
        IProjectRepository projectRepository,
        IDepartmentRepository departmentRepository,
        ILogger<ProjectService> logger)
    {
        _projectRepository = projectRepository;
        _departmentRepository = departmentRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<ProjectResponse>> GetAllAsync()
    {
        var projects = await _projectRepository.GetAllAsync();
        return projects.Select(MapToResponse);
    }

    public async Task<ProjectResponse?> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Project ID must be greater than zero.");
        }

        var project = await _projectRepository.GetByIdAsync(id);
        return project is null ? null : MapToResponse(project);
    }

    public async Task<IEnumerable<ProjectResponse>> GetByDepartmentIdAsync(int departmentId)
    {
        if (departmentId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(departmentId), "Department ID must be greater than zero.");
        }

        var projects = await _projectRepository.GetByDepartmentIdAsync(departmentId);
        return projects.Select(MapToResponse);
    }

    public async Task<IEnumerable<ProjectResponse>> SearchAsync(string? keyword)
    {
        var projects = await _projectRepository.SearchAsync(keyword);
        return projects.Select(MapToResponse);
    }

    public async Task<ProjectResponse> CreateAsync(ProjectCreateRequest request)
    {
        try
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (await _departmentRepository.GetByIdAsync(request.DepartmentId) is null)
            {
                throw new ArgumentException("Department does not exist.", nameof(request.DepartmentId));
            }

            var project = new Project
            {
                ProjectName = request.ProjectName.Trim(),
                Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = request.Status,
                DepartmentId = request.DepartmentId,
                IsActive = true,
                CreatedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
            };

            _logger.LogInformation(
                "Mapped Project entity. ProjectName={ProjectName}, DepartmentId={DepartmentId}, StartDate={StartDate}, EndDate={EndDate}, Status={Status}, CreatedDate={CreatedDate}, CreatedDateKind={CreatedDateKind}",
                project.ProjectName,
                project.DepartmentId,
                project.StartDate,
                project.EndDate,
                project.Status,
                project.CreatedDate,
                project.CreatedDate.Kind);

            var createdProject = await _projectRepository.AddAsync(project);
            return MapToResponse(createdProject);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Project creation failed. ExceptionType={ExceptionType}, Message={Message}, InnerException={InnerException}, StackTrace={StackTrace}",
                ex.GetType().FullName,
                ex.Message,
                ex.InnerException?.ToString(),
                ex.StackTrace);

            throw;
        }
    }

    public async Task<ProjectResponse?> UpdateAsync(int id, ProjectUpdateRequest request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Project ID must be greater than zero.");
        }

        var existingProject = await _projectRepository.GetByIdAsync(id);
        if (existingProject is null)
        {
            return null;
        }

        existingProject.ProjectName = request.ProjectName.Trim();
        existingProject.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        existingProject.StartDate = request.StartDate;
        existingProject.EndDate = request.EndDate;
        existingProject.Status = request.Status;
        existingProject.DepartmentId = request.DepartmentId;
        existingProject.IsActive = request.IsActive;

        var updatedProject = await _projectRepository.UpdateAsync(existingProject);
        return MapToResponse(updatedProject);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Project ID must be greater than zero.");
        }

        return await _projectRepository.DeleteAsync(id);
    }

    private static ProjectResponse MapToResponse(Project project)
    {
        return new ProjectResponse
        {
            ProjectId = project.ProjectId,
            ProjectName = project.ProjectName,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Status = project.Status,
            DepartmentId = project.DepartmentId,
            IsActive = project.IsActive,
            CreatedDate = project.CreatedDate
        };
    }
}
