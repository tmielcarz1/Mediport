using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Net;
using System.Net.Http.Json;
using WebApplication1.Application.Models.Common;
using WebApplication1.Application.Models.Tags.GetTags.Requests;
using WebApplication1.Application.Models.Tags.GetTags.Responses;

namespace WebApplication1.Tests.IntegrationTests
{
    [TestFixture]
    public class TagsControllerIntegrationTests
    {
        private WebApplicationFactory<Program>? _factory;
        private HttpClient? _client;

        [SetUp]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }


        [Test]
        public async Task GetTagDistributionPercentage_ShouldReturn200()
        {
            var response = await _client!.GetAsync("/api/Tags/GetTagDistributionPercentage");

            var result = await response.Content.ReadFromJsonAsync<State>();

            ClassicAssert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsNotNull(result?.StateObject);
        }


        [Test]
        public async Task GetTags_ShouldReturn200()
        {
            var request = new GetTagsRequest
            {
                PageNumber = 1,
                PageSize = 10,
                OrderBy = GetTagsRequestOrderBy.Name,
                SearchValue = "java",
                SortBy = GetTagsRequestSortBy.Ascending
            };

            var response = await _client!.PostAsJsonAsync("/api/Tags/GetTags", request);

            var result = await response.Content.ReadFromJsonAsync<State<GetTagsResponse>>();

            ClassicAssert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsNotNull(result?.StateObject);
        }

        [Test]
        public async Task GetTags_ShouldReturn400_WrongRequest()
        {
            var request = new GetTagsRequest
            {
                PageNumber = 1,
                PageSize = -5,
                SearchValue = "java"
            };

            var response = await _client!.PostAsJsonAsync("/api/Tags/GetTags", request);

            var result = await response.Content.ReadFromJsonAsync<State<List<GetTagsResponse>>>();

            ClassicAssert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }


        [Test]
        public async Task SyncTags_ShouldReturn200()
        {
            var response = await _client!.PostAsync("/api/Tags/SyncTags", null);

            var result = await response.Content.ReadFromJsonAsync<State>();

            ClassicAssert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsNull(result?.StateObject);
        }

    }
}