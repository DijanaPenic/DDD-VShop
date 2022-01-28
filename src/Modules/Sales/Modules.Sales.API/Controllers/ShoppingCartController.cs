using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

using VShop.SharedKernel.Application;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Sales.API.Models;
using VShop.Modules.Sales.Infrastructure.Queries;
using VShop.Modules.Sales.Infrastructure.Commands;
using VShop.Modules.Sales.Infrastructure.Commands.Shared;
using VShop.Modules.Sales.Infrastructure.Commands.Handlers;
using VShop.Modules.Sales.Infrastructure.DAL.Entities;
using VShop.Modules.Sales.Infrastructure.Queries.Contracts;

namespace VShop.Modules.Sales.API.Controllers
{
    [ApiController]
    [Route("api/shopping-carts")]
    internal class ShoppingCartController : ApplicationControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMapper _mapper;
        private readonly IShoppingCartReadService _readService;

        public ShoppingCartController
        (
            ICommandDispatcher commandDispatcher,
            IMapper mapper,
            IShoppingCartReadService readService
        )
        {
            _commandDispatcher = commandDispatcher;
            _mapper = mapper;
            _readService = readService;
        }
        
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ShoppingCartInfo), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetShoppingCartAsync([FromQuery] Guid customerId)
        {
            ShoppingCartInfo shoppingCart = await _readService.GetActiveShoppingCartByCustomerIdAsync(customerId);
            if (shoppingCart is null) return NotFound();

            return Ok(shoppingCart);
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateShoppingCartAsync([FromBody] CreateShoppingCartRequest request)
        {
            CreateShoppingCartCommand command = _mapper.Map<CreateShoppingCartCommand>(request);
            Result result = await _commandDispatcher.SendAsync(command);

            return HandleResult(result, () => CreatedAtAction
            (
                "GetShoppingCart",
                new { id = request.ShoppingCartId })
            );
        }

        [HttpDelete]
        [Route("{shoppingCartId:guid}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteShoppingCartAsync([FromRoute] Guid shoppingCartId)
        {
            DeleteShoppingCartCommand command = new(shoppingCartId);
            Result result = await _commandDispatcher.SendAsync(command);
        
            return HandleResult(result, NoContent);
        }
        
        [HttpPut]
        [Route("{shoppingCartId:guid}/actions/checkout")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CheckoutResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CheckoutShoppingCartAsync([FromRoute] Guid shoppingCartId)
        {
            CheckoutShoppingCartCommand command = new(shoppingCartId);
            Result<CheckoutResponse> result = await _commandDispatcher.SendAsync(command);
        
            return HandleResult(result, Ok);
        }
        
        [HttpPost]
        [Route("{shoppingCartId:guid}/products/{productId:guid}")]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> AddProductAsync
        (
            [FromRoute] Guid shoppingCartId,
            [FromRoute] Guid productId,
            [FromBody] AddShoppingCartProductRequest request
        )
        {
            AddShoppingCartProductCommand command = new
            (
                shoppingCartId,
                _mapper.Map<ShoppingCartProductCommandDto>(request)
            )
            {
                ShoppingCartItem = { ProductId = productId }
            };

            Result result = await _commandDispatcher.SendAsync(command);
        
            return HandleResult(result, Created);
        }
        
        [HttpPut]
        [Route("{shoppingCartId:guid}/products/{productId:guid}")]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> SetProductPriceAsync
        (
            [FromRoute] Guid shoppingCartId,
            [FromRoute] Guid productId,
            [FromBody] SetShoppingCartProductPriceRequest request
        )
        {
            SetShoppingCartProductPriceCommand command = _mapper.Map<SetShoppingCartProductPriceCommand>(request);
            command.ShoppingCartId = shoppingCartId;
            command.ProductId = productId;

            Result commandResult = await _commandDispatcher.SendAsync(command);
        
            return HandleResult(commandResult, Ok);
        }
        
        [HttpDelete]
        [Route("{shoppingCartId:guid}/products/{productId:guid}")]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RemoveProductAsync
        (
            [FromRoute] Guid shoppingCartId,
            [FromRoute] Guid productId,
            [FromBody] RemoveShoppingCartProductRequest request
        )
        {
            RemoveShoppingCartProductCommand command = _mapper.Map<RemoveShoppingCartProductCommand>(request);
            command.ShoppingCartId = shoppingCartId;
            command.ProductId = productId;

            Result commandResult = await _commandDispatcher.SendAsync(command);
        
            return HandleResult(commandResult, NoContent);
        }
        
        [HttpPost]
        [Route("{shoppingCartId:guid}/customer/contact-information")]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> SetContactInformationAsync
        (
            [FromRoute] Guid shoppingCartId,
            [FromBody] SetContactInformationRequest request
        )
        {
            SetContactInformationCommand command = _mapper.Map<SetContactInformationCommand>(request);
            command.ShoppingCartId = shoppingCartId;

            Result result = await _commandDispatcher.SendAsync(command);
        
            return HandleResult(result, Ok);
        }
        
        [HttpPost]
        [Route("{shoppingCartId:guid}/customer/delivery-address")]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> SetDeliveryAddressAsync
        (
            [FromRoute] Guid shoppingCartId,
            [FromBody] SetDeliveryAddressRequest request
        )
        {
            SetDeliveryAddressCommand command = _mapper.Map<SetDeliveryAddressCommand>(request);
            command.ShoppingCartId = shoppingCartId;

            Result result = await _commandDispatcher.SendAsync(command);
        
            return HandleResult(result, Ok);
        }
    }
}