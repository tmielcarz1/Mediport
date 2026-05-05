using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorApp2.Models.GetTags.Request
{
    public class GetTagsRequest
    {
        [Required]
        [JsonPropertyName("page_number")]
        [Range(1, int.MaxValue, ErrorMessage = "page_number must be >= 1")]
        public int PageNumber { get; set; }

        [Required]
        [JsonPropertyName("page_size")]
        [Range(1, int.MaxValue, ErrorMessage = "page_size must be >= 1")]
        public int PageSize { get; set; }

        [JsonPropertyName("search_value")]
        public string? SearchValue { get; set; }

        [JsonPropertyName("order_by")]
        public GetTagsRequestOrderBy? OrderBy { get; set; }

        [JsonPropertyName("sort_by")]
        public GetTagsRequestSortBy? SortBy { get; set; }

    }
}
