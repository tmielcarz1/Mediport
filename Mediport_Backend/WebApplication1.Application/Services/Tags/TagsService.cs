using AutoMapper;
using System.Net.Http.Json;
using WebApplication1.Application.Configuration;
using WebApplication1.Application.Interfaces;
using WebApplication1.Application.Models.Common;
using WebApplication1.Application.Models.Tags.GetTagDistributionPercentage.Responses;
using WebApplication1.Application.Models.Tags.GetTags.Requests;
using WebApplication1.Application.Models.Tags.GetTags.Responses;
using WebApplication1.Application.Models.Tags.TagsSync.Responses;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Application.Services.Tags
{
    public class TagsService : ITagsService
    {
        private readonly StackOverflowApiConfiguration _stackOverflowApiConfiguration;
        private readonly ITagsRepository _tagsRepository;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public TagsService(StackOverflowApiConfiguration stackOverflowApiConfiguration, ITagsRepository tagsRepository, HttpClient httpClient,
            IMapper mapper)
        {
            _stackOverflowApiConfiguration = stackOverflowApiConfiguration;
            _tagsRepository = tagsRepository;
            _httpClient = httpClient;
            _mapper = mapper;
        }

        public async Task<State> SyncTags(CancellationToken ct)
        {
            var state = new State();
            var tagsList = new List<Tag>();
            int pageSize = 100;
            var toFetch = _stackOverflowApiConfiguration.TagsToFetch;
            var totalPages = (int)Math.Ceiling((double)toFetch / pageSize);
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

            for (int page = 1; page <= totalPages; page++)
            {
                try
                {
                    var url = _stackOverflowApiConfiguration.GetTagsUrl!
                        .Replace("{pageSize}", pageSize.ToString())
                        .Replace("{page}", page.ToString());

                    var response = await _httpClient.GetFromJsonAsync<TagsSyncApiResponse>(url, ct);

                    if (response?.Items == null)
                        continue;

                    var mapped = _mapper.Map<List<Tag>>(response.Items);
                    var left = toFetch - tagsList.Count;

                    if (left <= 0)
                        break;

                    if (mapped.Count > left)
                        mapped = mapped.Take(left).ToList();

                    tagsList.AddRange(mapped);

                    if (tagsList.Count >= toFetch)
                        break;
                }
                catch (Exception ex)
                {
                    state.AddError(ex.Message);
                    Console.WriteLine(ex.Message);
                    return state;
                }
            }

            try
            {
                await _tagsRepository.AddListTags(tagsList, ct);
            }
            catch (Exception ex)
            {
                state.AddError(ex.Message);
                return state;
            }
            return state;
        }

        public async Task<State<List<GetTagDistributionPercentageResponse>>> GetTagDistributionPercentage(CancellationToken ct)
        {
            var state = new State<List<GetTagDistributionPercentageResponse>>();
            var tagsList = new List<Tag>();
            try
            {
                tagsList = await _tagsRepository.GetAllTags(ct);
                if (tagsList is null || tagsList.Count == 0)
                    return state;

                var result = tagsList.Select(a => new GetTagDistributionPercentageResponse
                {
                    Name = a.Name,
                    Percentage = tagsList.Sum(b => b.Count) > 0 ? Math.Round((double)a.Count * 100 / tagsList.Sum(b => b.Count), 2) : 0
                }).ToList();

                state.StateObject = result;
                return state;
            }
            catch (Exception ex)
            {
                state.AddError(ex.Message);
                return state;
            }
        }

        public async Task<State<GetTagsResponse>> GetTags(GetTagsRequest getTagsRequest, CancellationToken ct)
        {
            var state = new State<GetTagsResponse>();
            try
            {
                state.StateObject = await _tagsRepository.GetTags(getTagsRequest, ct);
            }
            catch (Exception ex)
            {
                state.AddError(ex.Message);
                return state;
            }
            return state;
        }


    }
}
