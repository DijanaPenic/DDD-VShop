using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

using VShop.SharedKernel.Application;
using VShop.SharedKernel.Infrastructure;
using VShop.Modules.Billing.API.Models;
using VShop.Modules.Billing.Infrastructure.Commands;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;

namespace VShop.Modules.Billing.API.Controllers
{
    [ApiController]
    [Route("api/payment")]
    internal class PaymentController : ApplicationControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMapper _mapper;
        
        public PaymentController(ICommandDispatcher commandDispatcher, IMapper mapper)
        {
            _commandDispatcher = commandDispatcher;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("")]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> TransferAsync
        (
            [FromBody] TransferRequest request,
            [FromHeader(Name = "x-request-id")] Guid requestId,
            [FromHeader(Name = "x-correlation-id")] Guid correlationId
        )
        {
            TransferCommand command = _mapper.Map<TransferCommand>(request);
            command.Metadata = new MessageMetadata(requestId, Guid.Empty, correlationId);
            
            Result result = await _commandDispatcher.SendAsync(command);

            return HandleResult(result, Ok);
        }
    }
}