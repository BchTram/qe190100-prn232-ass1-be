using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Interfaces;

public interface ITagRepository
{
    Task<IEnumerable<Tag>> GetAllAsync();
    Task<Tag?> GetByIdAsync(int id);
    Task<Tag> AddAsync(Tag tag);
    Task<Tag> UpdateAsync(Tag tag);
    Task<bool> DeleteAsync(int id);
    Task<bool> IsTagReferencedByAnyTaskAsync(int id);
}
