using AutoMapper;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using VShop.SharedKernel.Application;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.Modules.Billing.API.Models;
using VShop.Modules.Billing.API.Application.Commands;

namespace VShop.Modules.Billing.API.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ApplicationControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly IMapper _mapper;
        
        public PaymentController(ICommandBus commandBus, IMapper mapper)
        {
            _commandBus = commandBus;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("")]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> RequestPaymentAsync([FromBody]InitiatePaymentRequest request)
        {
            InitiateTransferCommand command = _mapper.Map<InitiateTransferCommand>(request);
            Result result = await _commandBus.SendAsync(command);

            return HandleResult(result, Ok);
        }
    }
}