using AutoMapper;
using WebApplication1.Application.Models.Tags.GetTagDistributionPercentage.Responses;
using WebApplication1.Application.Models.Tags.GetTags.Responses;
using WebApplication1.Application.Models.Tags.TagsSync.Responses;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Application.AutomapperProfiles
{
    public class TagProfile : Profile
    {
        public TagProfile()
        {
            CreateMap<TagsSyncApiResponseItem, Tag>();
            CreateMap<Tag, GetTagDistributionPercentageResponse>();
            CreateMap<Tag, GetTagsResponse>();

        }
    }
}
