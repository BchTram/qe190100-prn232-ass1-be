using TaskTrack.Service.DTOs;

namespace TaskTrack.Service.Interfaces;

public interface ITagService
{
    Task<IEnumerable<TagResponse>> GetAllAsync();
    Task<TagResponse?> GetByIdAsync(int id);
    Task<TagResponse> CreateAsync(TagCreateRequest request);
    Task<TagResponse?> UpdateAsync(int id, TagUpdateRequest request);
    Task<bool> DeleteAsync(int id);
}
