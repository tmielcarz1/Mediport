using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebApplication1.Application.Models.Common;
using WebApplication1.Application.Models.Tags.GetTagDistributionPercentage.Responses;
using WebApplication1.Application.Models.Tags.GetTags.Requests;
using WebApplication1.Application.Models.Tags.GetTags.Responses;
using WebApplication1.Application.Services.Tags;

namespace WebApplication1.Controllers
{
    /// <summary>
    /// kontroler dla tagów
    /// </summary>
    [Route("api/Tags")]
    [ApiController]
    [AllowAnonymous]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class TagsController : ControllerBase
    {
        private readonly ITagsService _tagsService;

        /// <summary>
        /// konstruktor
        /// </summary>
        /// <param name="tagsService"></param>
        public TagsController(ITagsService tagsService)
        {
            _tagsService = tagsService;
        }

        /// <summary>
        /// wymuszenie ponownego pobrania tagów z API Stack Overflow (iloœæ pobieranych wyników jest w appsettings)
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost, Route("SyncTags")]
        [ProducesResponseType(typeof(State), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(State), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SyncTags(CancellationToken cancellationToken)
        {
            var _state = await _tagsService.SyncTags(cancellationToken);
            if (_state.IsNotValid)
                return BadRequest(_state);

            return Ok(_state);
        }

        /// <summary>
        /// Obliczenie procentowego udzia³u tagów w ca³ej pobranej populacji
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet, Route("GetTagDistributionPercentage")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(State), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(State<GetTagDistributionPercentageResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(State), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetTagDistributionPercentage(CancellationToken cancellationToken)
        {
            var _state = await _tagsService.GetTagDistributionPercentage(cancellationToken);
            if (_state.IsNotValid)
                return BadRequest(_state);

            return Ok(_state);
        }

        /// <summary>
        /// Lista tagów wraz z sortowaniem i paginacj¹
        /// </summary>
        /// <remarks>
        /// <b>page_number</b> - >= 1 (wymagane) <br /><br />
        /// <b>page_size</b> - >= 1 (wymagane) <br /> <br />
        /// <b>search_by</b> - wyszukiwanie (niewymagane) <br /><br />
        /// <b>order_by</b> - sortowanie po (niewymagane, gdy brak domyœlnie po nazwie)<br /> 0 po nazwie,<br />  1 po procentach, <br /><br />
        /// <b>sort_by</b> - (niewymagane, gdy brak domyœlnie rosn¹co) <br /> 0 rosn¹co,<br /> 1 malej¹co<br /><br />
        /// </remarks>
        /// <param name="getTagsRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost, Route("GetTags")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(State), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(State<GetTagsResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(State), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetTags([FromBody] GetTagsRequest getTagsRequest, CancellationToken cancellationToken)
        {
            var _state = await _tagsService.GetTags(getTagsRequest, cancellationToken);
            if (_state.IsNotValid)
                return BadRequest(_state);

            return Ok(_state);
        }
    }
}
