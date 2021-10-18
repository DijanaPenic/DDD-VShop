using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using VShop.Services.Basket.API.Models;
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

        public BasketController
        (
            IMediator mediator,
            IMapper mapper
        )
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        private readonly IMapper _mapper;

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        // TODO - need better error/response handling
        public async Task<IActionResult> CreateBasketAsync([FromBody]BasketPostDto basket)
        {
            CreateBasketCommand command = _mapper.Map<CreateBasketCommand>(basket);
            
            bool result = await _mediator.Send(command);   
            if (!result)
            {
                return BadRequest();
            }

            return Ok();
        }
        
        [HttpPost]
        [Route("{customerId:guid}")]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddBasketItemAsync([FromRoute]Guid customerId, [FromBody]BasketItemPostDto basketItem)
        {
            AddBasketItemCommand command = _mapper.Map<AddBasketItemCommand>(basketItem);
            command.CustomerId = customerId;
            
            bool result = await _mediator.Send(command);   
            if (!result)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}