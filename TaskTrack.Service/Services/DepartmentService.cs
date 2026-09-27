using TaskTrack.Repo.Interfaces;
using TaskTrack.Repo.Models;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.Service.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;

    public DepartmentService(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<IEnumerable<DepartmentResponse>> GetAllAsync()
    {
        var departments = await _departmentRepository.GetAllAsync();
        return departments.Select(MapToResponse);
    }

    public async Task<DepartmentResponse?> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Department ID must be greater than zero.");
        }

        var department = await _departmentRepository.GetByIdAsync(id);
        return department is null ? null : MapToResponse(department);
    }

    public async Task<IEnumerable<DepartmentResponse>> SearchAsync(string? keyword)
    {
        var departments = await _departmentRepository.SearchAsync(keyword);
        return departments.Select(MapToResponse);
    }

    public async Task<DepartmentResponse> CreateAsync(DepartmentCreateRequest request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var department = new Department
        {
            DepartmentName = request.DepartmentName.Trim(),
            DepartmentDescription = string.IsNullOrWhiteSpace(request.DepartmentDescription)
                ? string.Empty
                : request.DepartmentDescription.Trim(),
            IsActive = true
        };

        var createdDepartment = await _departmentRepository.AddAsync(department);
        return MapToResponse(createdDepartment);
    }

    public async Task<DepartmentResponse?> UpdateAsync(int id, DepartmentUpdateRequest request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Department ID must be greater than zero.");
        }

        var existingDepartment = await _departmentRepository.GetByIdAsync(id);
        if (existingDepartment is null)
        {
            return null;
        }

        existingDepartment.DepartmentName = request.DepartmentName.Trim();
        existingDepartment.DepartmentDescription = string.IsNullOrWhiteSpace(request.DepartmentDescription)
            ? string.Empty
            : request.DepartmentDescription.Trim();
        existingDepartment.IsActive = request.IsActive;

        var updatedDepartment = await _departmentRepository.UpdateAsync(existingDepartment);
        return MapToResponse(updatedDepartment);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Department ID must be greater than zero.");
        }

        return await _departmentRepository.DeleteAsync(id);
    }

    private static DepartmentResponse MapToResponse(Department department)
    {
        return new DepartmentResponse
        {
            DepartmentId = department.DepartmentId,
            DepartmentName = department.DepartmentName,
            DepartmentDescription = department.DepartmentDescription,
            IsActive = department.IsActive
        };
    }
}
