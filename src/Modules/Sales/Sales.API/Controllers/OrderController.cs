using AutoMapper;
using Microsoft.AspNetCore.Mvc;

using VShop.SharedKernel.Application;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Sales.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ApplicationControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMapper _mapper;

        public OrderController
        (
            ICommandDispatcher commandDispatcher,
            IMapper mapper
        )
        {
            _commandDispatcher = commandDispatcher;
            _mapper = mapper;
        }
        
        // [HttpPost]
        // [Consumes("application/json")]
        // [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        // [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        // [ProducesResponseType(typeof(Order), (int)HttpStatusCode.Created)]
        // public async Task<IActionResult> PlaceOrderAsync([FromBody] PlaceOrderRequest request)
        // {
        //     PlaceOrderCommand command = _mapper.Map<PlaceOrderCommand>(request);
        //     
        //     Result<Order> result = await _commandBus.Send(command);
        //
        //     return HandleObjectResult(result, Created);
        // }
    }
}