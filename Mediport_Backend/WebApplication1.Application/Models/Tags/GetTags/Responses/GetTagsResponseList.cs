using System.Text.Json.Serialization;

namespace WebApplication1.Application.Models.Tags.GetTags.Responses
{
    public class GetTagsResponseList
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("percentage")]
        public decimal Percentage { get; set; }

    }
}
