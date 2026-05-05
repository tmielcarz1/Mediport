using System.Text.Json.Serialization;

namespace WebApplication1.Application.Models.Tags.GetTagDistributionPercentage.Responses
{
    public class GetTagDistributionPercentageResponse
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("percentage")]
        public double Percentage { get; set; }
    }
}
