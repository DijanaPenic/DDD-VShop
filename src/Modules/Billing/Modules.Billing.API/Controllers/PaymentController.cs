using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using VShop.SharedKernel.Application;
using VShop.SharedKernel.Infrastructure;
using VShop.Modules.Billing.API.Models;
using VShop.Modules.Billing.Infrastructure.Commands;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Billing.API.Controllers
{
    [ApiController]
    [Route("api/payment")]
    [Authorize(Policy)]
    internal class PaymentController : ApplicationControllerBase
    {
        private const string Policy = "payments";
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMapper _mapper;
        
        public PaymentController(ICommandDispatcher commandDispatcher, IMapper mapper)
        {
            _commandDispatcher = commandDispatcher;
            _mapper = mapper;
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> TransferAsync([FromBody] TransferRequest request)
        {
            TransferCommand command = _mapper.Map<TransferCommand>(request);
            Result result = await _commandDispatcher.SendAsync(command);

            return HandleResult(result, NoContent);
        }
    }
}