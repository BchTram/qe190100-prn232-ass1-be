using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectResponse>> GetAllAsync();
    Task<ProjectResponse?> GetByIdAsync(int id);
    Task<IEnumerable<ProjectResponse>> GetByDepartmentIdAsync(int departmentId);
    Task<IEnumerable<ProjectResponse>> SearchAsync(string? keyword);
    Task<ProjectResponse> CreateAsync(ProjectCreateRequest request);
    Task<ProjectResponse?> UpdateAsync(int id, ProjectUpdateRequest request);
    Task<bool> DeleteAsync(int id);
}
