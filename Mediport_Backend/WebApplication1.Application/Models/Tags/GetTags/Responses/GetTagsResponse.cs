using System.Text.Json.Serialization;

namespace WebApplication1.Application.Models.Tags.GetTags.Responses
{
    public class GetTagsResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("tags")]
        public List<GetTagsResponseList> Tags { get; set; } = new List<GetTagsResponseList>();
    }
}
