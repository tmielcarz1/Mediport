using WebApplication1.Application.Models.Tags.GetTags.Requests;
using WebApplication1.Application.Models.Tags.GetTags.Responses;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Application.Interfaces
{
    public interface ITagsRepository
    {
        public Task AddListTags(List<Tag> tags, CancellationToken ct);
        public Task<List<Tag>> GetAllTags(CancellationToken ct);
        public Task<GetTagsResponse> GetTags(GetTagsRequest getTagsRequest, CancellationToken ct);
    }
}
