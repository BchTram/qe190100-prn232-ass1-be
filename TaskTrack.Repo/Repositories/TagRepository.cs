using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Interfaces;
using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories;

public class TagRepository : ITagRepository
{
    private readonly Prn232PostgresContext _context;

    public TagRepository(Prn232PostgresContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Tag>> GetAllAsync()
    {
        return await _context.Tags
            .AsNoTracking()
            .OrderBy(t => t.TagName)
            .ToListAsync();
    }

    public async Task<Tag?> GetByIdAsync(int id)
    {
        return await _context.Tags
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TagId == id);
    }

    public async Task<Tag> AddAsync(Tag tag)
    {
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();
        return tag;
    }

    public async Task<Tag> UpdateAsync(Tag tag)
    {
        _context.Entry(tag).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return tag;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var tag = await _context.Tags.FirstOrDefaultAsync(t => t.TagId == id);
        if (tag is null)
        {
            return false;
        }

        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsTagReferencedByAnyTaskAsync(int id)
    {
        return await _context.Tasks
            .AsNoTracking()
            .AnyAsync(t => t.Tags.Any(tag => tag.TagId == id));
    }
}
