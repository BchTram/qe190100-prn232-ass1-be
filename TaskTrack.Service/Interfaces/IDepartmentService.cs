using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Interfaces;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentResponse>> GetAllAsync();
    Task<DepartmentResponse?> GetByIdAsync(int id);
    Task<IEnumerable<DepartmentResponse>> SearchAsync(string? keyword);
    Task<DepartmentResponse> CreateAsync(DepartmentCreateRequest request);
    Task<DepartmentResponse?> UpdateAsync(int id, DepartmentUpdateRequest request);
    Task<bool> DeleteAsync(int id);
}
