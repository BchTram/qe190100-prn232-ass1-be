using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Interfaces;
using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly Prn232PostgresContext _context;

    public DepartmentRepository(Prn232PostgresContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Department>> GetAllAsync()
    {
        return await _context.Departments
            .AsNoTracking()
            .OrderBy(d => d.DepartmentName)
            .ToListAsync();
    }

    public async Task<Department?> GetByIdAsync(int id)
    {
        return await _context.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.DepartmentId == id);
    }

    public async Task<IEnumerable<Department>> SearchAsync(string? keyword)
    {
        var normalizedKeyword = keyword?.Trim();

        var query = _context.Departments
            .AsNoTracking()
            .Where(d => d.IsActive);

        if (!string.IsNullOrWhiteSpace(normalizedKeyword))
        {
            var lowerKeyword = normalizedKeyword.ToLower();
            query = query.Where(d =>
                d.DepartmentName.ToLower().Contains(lowerKeyword) ||
                (d.DepartmentDescription != null && d.DepartmentDescription.ToLower().Contains(lowerKeyword)));
        }

        return await query
            .OrderBy(d => d.DepartmentName)
            .ToListAsync();
    }

    public async Task<Department> AddAsync(Department department)
    {
        _context.Departments.Add(department);
        await _context.SaveChangesAsync();
        return department;
    }

    public async Task<Department> UpdateAsync(Department department)
    {
        _context.Entry(department).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return department;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var department = await _context.Departments.FirstOrDefaultAsync(d => d.DepartmentId == id);
        if (department is null)
        {
            return false;
        }

        if (await _context.Projects.AnyAsync(p => p.DepartmentId == id))
        {
            throw new InvalidOperationException(
                "Department cannot be deleted because it contains projects.");
        }

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();
        return true;
    }
}
