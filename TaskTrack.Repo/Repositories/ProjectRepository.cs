using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Interfaces;
using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly Prn232PostgresContext _context;

    public ProjectRepository(Prn232PostgresContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Project>> GetAllAsync()
    {
        return await _context.Projects
            .AsNoTracking()
            .Include(p => p.Department)
            .OrderBy(p => p.ProjectName)
            .ToListAsync();
    }

    public async Task<Project?> GetByIdAsync(int id)
    {
        return await _context.Projects
            .AsNoTracking()
            .Include(p => p.Department)
            .FirstOrDefaultAsync(p => p.ProjectId == id);
    }

    public async Task<IEnumerable<Project>> GetByDepartmentIdAsync(int departmentId)
    {
        return await _context.Projects
            .AsNoTracking()
            .Include(p => p.Department)
            .Where(p => p.DepartmentId == departmentId)
            .OrderBy(p => p.ProjectName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Project>> SearchAsync(string? keyword)
    {
        var normalizedKeyword = keyword?.Trim();

        var query = _context.Projects
            .AsNoTracking()
            .Include(p => p.Department)
            .Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(normalizedKeyword))
        {
            var lowerKeyword = normalizedKeyword.ToLower();
            query = query.Where(p =>
                p.ProjectName.ToLower().Contains(lowerKeyword) ||
                (p.Description != null && p.Description.ToLower().Contains(lowerKeyword)));
        }

        return await query
            .OrderBy(p => p.ProjectName)
            .ToListAsync();
    }

    public async Task<Project> AddAsync(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<Project> UpdateAsync(Project project)
    {
        _context.Entry(project).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == id);
        if (project is null)
        {
            return false;
        }

        if (await _context.Tasks.AnyAsync(t => t.ProjectId == id))
        {
            throw new InvalidOperationException(
                "Project cannot be deleted because it contains tasks.");
        }

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return true;
    }
}
