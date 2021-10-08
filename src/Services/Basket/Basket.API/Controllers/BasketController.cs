using System.Net;
using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using VShop.Services.Basket.API.Application.Commands;

namespace VShop.Services.Basket.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BasketController> _logger;
        
        public BasketController
        (
            IMediator mediator,
            ILogger<BasketController> logger
        )
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateBasketAsync([FromBody] CreateBasketCommand basket)
        {
            bool result = await _mediator.Send(basket);   
            if (!result)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}