using WebApplication1.Application.Models.Common;
using WebApplication1.Application.Models.Tags.GetTagDistributionPercentage.Responses;
using WebApplication1.Application.Models.Tags.GetTags.Requests;
using WebApplication1.Application.Models.Tags.GetTags.Responses;

namespace WebApplication1.Application.Services.Tags
{
    public interface ITagsService
    {
        public Task<State> SyncTags(CancellationToken ct);
        public Task<State<List<GetTagDistributionPercentageResponse>>> GetTagDistributionPercentage(CancellationToken ct);
        public Task<State<GetTagsResponse>> GetTags(GetTagsRequest getTagsRequest, CancellationToken ct);
    }
}
