using OneOf;
using OneOf.Types;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

using VShop.SharedKernel.Application;
using VShop.SharedKernel.Application.Commands;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.Modules.Sales.API.Application.Commands;

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