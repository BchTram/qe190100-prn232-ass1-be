using TaskTrack.Repo.Interfaces;
using TaskTrack.Repo.Models;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.Service.Services;

public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;

    public TagService(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<IEnumerable<TagResponse>> GetAllAsync()
    {
        var tags = await _tagRepository.GetAllAsync();
        return tags.Select(MapToResponse);
    }

    public async Task<TagResponse?> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Tag ID must be greater than zero.");
        }

        var tag = await _tagRepository.GetByIdAsync(id);
        return tag is null ? null : MapToResponse(tag);
    }

    public async Task<TagResponse> CreateAsync(TagCreateRequest request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var tag = new Tag
        {
            TagName = request.TagName.Trim(),
            Color = string.IsNullOrWhiteSpace(request.Color) ? "#000000" : request.Color.Trim()
        };

        var createdTag = await _tagRepository.AddAsync(tag);
        return MapToResponse(createdTag);
    }

    public async Task<TagResponse?> UpdateAsync(int id, TagUpdateRequest request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Tag ID must be greater than zero.");
        }

        var existingTag = await _tagRepository.GetByIdAsync(id);
        if (existingTag is null)
        {
            return null;
        }

        existingTag.TagName = request.TagName.Trim();
        existingTag.Color = string.IsNullOrWhiteSpace(request.Color) ? "#000000" : request.Color.Trim();

        var updatedTag = await _tagRepository.UpdateAsync(existingTag);
        return MapToResponse(updatedTag);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Tag ID must be greater than zero.");
        }

        var isReferenced = await _tagRepository.IsTagReferencedByAnyTaskAsync(id);
        if (isReferenced)
        {
            throw new InvalidOperationException(
                "Tag cannot be deleted because it is being used.");
        }

        return await _tagRepository.DeleteAsync(id);
    }

    private static TagResponse MapToResponse(Tag tag)
    {
        return new TagResponse
        {
            TagId = tag.TagId,
            TagName = tag.TagName,
            Color = tag.Color
        };
    }
}
