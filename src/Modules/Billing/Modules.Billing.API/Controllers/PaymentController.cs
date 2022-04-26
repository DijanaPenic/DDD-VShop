using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using VShop.SharedKernel.Application;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Billing.Infrastructure.Models;
using VShop.Modules.Billing.Infrastructure.Commands;

namespace VShop.Modules.Billing.API.Controllers
{
    [ApiController]
    [Route("api/payment")]
    internal class PaymentController : ApplicationControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public PaymentController(ICommandDispatcher commandDispatcher) => _commandDispatcher = commandDispatcher;

        [HttpPost]
        [Route("intent")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(PaymentIntentInfo), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [AllowAnonymous] // TODO - check.
        public async Task<IActionResult> CreatePaymentIntentAsync([FromBody] CreatePaymentIntentCommand command)
        {
            Result<PaymentIntentInfo> result = await _commandDispatcher.SendAsync(command);
            return HandleResult(result, Ok);
        }

        [HttpPost("webhook")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [AllowAnonymous]
        public async Task<IActionResult> ProcessTransferAsync()
        {
            Result result = await _commandDispatcher.SendAsync(new ProcessTransferCommand());
            return HandleResult(result, NoContent);
        }
    }
}