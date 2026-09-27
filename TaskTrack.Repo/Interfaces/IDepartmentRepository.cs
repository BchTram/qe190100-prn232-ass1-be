using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Interfaces;

public interface IDepartmentRepository
{
    Task<IEnumerable<Department>> GetAllAsync();
    Task<Department?> GetByIdAsync(int id);
    Task<IEnumerable<Department>> SearchAsync(string? keyword);
    Task<Department> AddAsync(Department department);
    Task<Department> UpdateAsync(Department department);
    Task<bool> DeleteAsync(int id);
}
