using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Interfaces;

public interface IProjectRepository
{
    Task<IEnumerable<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(int id);
    Task<IEnumerable<Project>> GetByDepartmentIdAsync(int departmentId);
    Task<IEnumerable<Project>> SearchAsync(string? keyword);
    Task<Project> AddAsync(Project project);
    Task<Project> UpdateAsync(Project project);
    Task<bool> DeleteAsync(int id);
}
