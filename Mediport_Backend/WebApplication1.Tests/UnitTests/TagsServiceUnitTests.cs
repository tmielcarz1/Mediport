using AutoMapper;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Net;
using System.Net.Http.Json;
using WebApplication1.Application.Configuration;
using WebApplication1.Application.Interfaces;
using WebApplication1.Application.Models.Tags.GetTags.Requests;
using WebApplication1.Application.Models.Tags.GetTags.Responses;
using WebApplication1.Application.Models.Tags.TagsSync.Responses;
using WebApplication1.Application.Services.Tags;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Tests.UnitTests
{
    [TestFixture]
    public class TagsServiceUnitTests
    {
        private Mock<ITagsRepository>? _repoMock;
        private Mock<IMapper>? _mapperMock;
        private TagsService? _service;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<ITagsRepository>();
            _mapperMock = new Mock<IMapper>();

            var config = new StackOverflowApiConfiguration
            {
                TagsToFetch = 100,
                GetTagsUrl = "http://wp.pl?page={page}&pagesize={pageSize}"
            };

            var httpClient = new HttpClient();

            _service = new TagsService(
                config,
                _repoMock.Object,
                httpClient,
                _mapperMock.Object
            );
        }

        /// <summary>
        /// zwraca tagi
        /// </summary>
        /// <returns></returns>

        [Test]
        public async Task GetTags_ShouldGetTags()
        {
            var request = new GetTagsRequest();

            var expected = new GetTagsResponse
            {
                Count = 2,
                Tags = new List<GetTagsResponseList>()
                {
                    new GetTagsResponseList()
                    {
                        Count = 5, Name = "java", Percentage = 0.3m
                    },
                    new GetTagsResponseList()
                    {
                        Count = 53, Name = ".net", Percentage = 0.44m
                    }
                }
            };

            _repoMock!.Setup(x => x.GetTags(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await _service!.GetTags(request, CancellationToken.None);

            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsNotNull(result.StateObject);
            ClassicAssert.AreEqual(2, result.StateObject.Count);
            ClassicAssert.AreEqual("java", result.StateObject.Tags[0].Name);
            ClassicAssert.AreEqual(".net", result.StateObject.Tags[1].Name);

        }


        /// <summary>
        /// błąd bazy podczas pobierania
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetTags_ShouldntGetTags_DbError()
        {
            var request = new GetTagsRequest();

            _repoMock!.Setup(x => x.GetTags(request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            var result = await _service!.GetTags(request, CancellationToken.None);

            ClassicAssert.IsTrue(result.IsNotValid);
        }

        /// <summary>
        /// zwraca procenty
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetTagDistributionPercentage_ShouldGetPercentages()
        {
            var tags = new List<Tag>
            {
                new Tag { Name = "c#", Count = 50 },
                new Tag { Name = "java", Count = 50 }
            };

            _repoMock!.Setup(x => x.GetAllTags(It.IsAny<CancellationToken>()))
                .ReturnsAsync(tags);

            var result = await _service!.GetTagDistributionPercentage(CancellationToken.None);

            ClassicAssert.IsNotNull(result.StateObject);
            ClassicAssert.AreEqual(2, result.StateObject.Count);

            ClassicAssert.AreEqual(50, result.StateObject.First(x => x.Name == "c#").Percentage);
            ClassicAssert.AreEqual(50, result.StateObject.First(x => x.Name == "java").Percentage);
        }

        /// <summary>
        /// ok, ale nie ma wyników
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetTagDistributionPercentage_ShouldGetPercentages_Empty()
        {
            _repoMock!.Setup(x => x.GetAllTags(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Tag>());

            var result = await _service!.GetTagDistributionPercentage(CancellationToken.None);

            ClassicAssert.IsNull(result.StateObject);
        }


        /// <summary>
        /// błąd bazy
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetTagDistributionPercentage_ShouldntGetPercentages_DbError()
        {
            _repoMock!.Setup(x => x.GetAllTags(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            var result = await _service!.GetTagDistributionPercentage(CancellationToken.None);

            ClassicAssert.IsTrue(result.IsNotValid);
        }

        /// <summary>
        /// Gdy tagi są aktualizowane
        /// </summary>
        /// <returns></returns>

        [Test]
        public async Task SyncTags_ShouldSync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(new TagsSyncApiResponse
                    {
                        Items = new List<TagsSyncApiResponseItem>
                        {
                            new TagsSyncApiResponseItem { Name = "c#", Count = 100 },
                            new TagsSyncApiResponseItem { Name = "java", Count = 1050 },
                            new TagsSyncApiResponseItem { Name = "javascript", Count = 125 },
                        }
                    })
                });

            var httpClient = new HttpClient(handlerMock.Object);

            _mapperMock!.Setup(x => x.Map<List<Tag>>(It.IsAny<object>()))
                .Returns(new List<Tag>
                {
                    new Tag { Name = "c#", Count = 100 }
                });

            var config = new StackOverflowApiConfiguration
            {
                TagsToFetch = 1,
                GetTagsUrl = "https://wp.pl?page={page}&pagesize={pageSize}"
            };

            var service = new TagsService(
                config,
                _repoMock!.Object,
                httpClient,
                _mapperMock.Object
            );

            var result = await service.SyncTags(CancellationToken.None);

            _repoMock.Verify(
                x => x.SyncListTags(It.IsAny<List<Tag>>(), It.IsAny<CancellationToken>()),
                Times.Once);

            ClassicAssert.IsFalse(result.IsNotValid);
        }

        /// <summary>
        /// Gdy nie połączy się z serwerem SO
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task SyncTags_ShouldntSync_HttpConnectionFail()
        {
            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new Exception("HTTP error"));

            var httpClient = new HttpClient(handlerMock.Object);

            var config = new StackOverflowApiConfiguration
            {
                TagsToFetch = 1,
                GetTagsUrl = "https://wp.pl"
            };

            var service = new TagsService(
                config,
                _repoMock!.Object,
                httpClient,
                _mapperMock!.Object
            );

            var result = await service.SyncTags(CancellationToken.None);

            ClassicAssert.IsTrue(result.IsNotValid);
        }
    }
}