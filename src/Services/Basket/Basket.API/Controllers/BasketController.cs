using MediatR;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using VShop.Services.Basket.API.Application.Models;
using VShop.Services.Basket.API.Application.Commands;

namespace VShop.Services.Basket.API.Controllers
{
    [ApiController]
    [Route("api/basket")]
    // TODO - change query parameters to snake case
    // TODO - resolve enum by value
    public class BasketController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BasketController> _logger;
        private readonly IMapper _mapper;
        
        public BasketController
        (
            IMediator mediator,
            ILogger<BasketController> logger,
            IMapper mapper
        )
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        // TODO - need better error/response handling
        public async Task<IActionResult> CreateBasketAsync([FromBody]BasketDto basket)
        {
            CreateBasketCommand basketCommand = _mapper.Map<CreateBasketCommand>(basket);
            
            bool result = await _mediator.Send(basketCommand);   
            if (!result)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}