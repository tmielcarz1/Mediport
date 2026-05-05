using System.Text.Json.Serialization;

namespace BlazorApp2.Models.GetTags.Response
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
