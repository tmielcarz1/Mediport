using System.Text.Json.Serialization;

namespace BlazorApp2.Models.GetTags.Response
{
    public class GetTagsResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("tags")]
        public List<GetTagsResponseList>? Tags { get; set; }
    }
}
