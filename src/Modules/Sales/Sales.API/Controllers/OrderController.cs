using AutoMapper;
using Microsoft.AspNetCore.Mvc;

using VShop.SharedKernel.Application;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;

namespace VShop.Modules.Sales.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ApplicationControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly IMapper _mapper;

        public OrderController
        (
            ICommandBus commandBus,
            IMapper mapper
        )
        {
            _commandBus = commandBus;
            _mapper = mapper;
        }
        
        // [HttpPost]
        // [Consumes("application/json")]
        // [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        // [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        // [ProducesResponseType(typeof(Order), (int)HttpStatusCode.Created)]
        // public async Task<IActionResult> PlaceOrderAsync([FromBody] PalceOrderRequest request)
        // {
        //     PlaceOrderCommand command = _mapper.Map<PlaceOrderCommand>(request);
        //     
        //     OneOf<Success<Order>, ApplicationError> result = await _commandBus.Send(command);
        //
        //     return HandleObjectResult(result, Created);
        // }
    }
}