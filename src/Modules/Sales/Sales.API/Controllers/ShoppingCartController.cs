using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Application;
using VShop.SharedKernel.Infrastructure;
using VShop.Modules.Sales.API.Models;
using VShop.Modules.Sales.API.Application.Queries;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Sales.API.Application.Commands.Shared;
using VShop.Modules.Sales.Infrastructure.Entities;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.API.Controllers
{
    [ApiController]
    [Route("api/shopping-carts")]
    public class ShoppingCartController : ApplicationControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly IMapper _mapper;
        private readonly IShoppingCartQueryService _queryService;

        public ShoppingCartController
        (
            ICommandBus commandBus,
            IMapper mapper,
            IShoppingCartQueryService queryService
        )
        {
            _commandBus = commandBus;
            _mapper = mapper;
            _queryService = queryService;
        }
        
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ShoppingCartInfo), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetShoppingCartAsync([FromQuery] Guid customerId)
        {
            ShoppingCartInfo shoppingCart = await _queryService.GetActiveShoppingCartByCustomerIdAsync(customerId);
            if (shoppingCart is null) return NotFound();

            return Ok(shoppingCart);
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateShoppingCartAsync
        (
            [FromBody] CreateShoppingCartRequest request,
            [FromHeader(Name = "x-request-id")] Guid requestId,
            [FromHeader(Name = "x-correlation-id")] Guid correlationId
        )
        {
            CreateShoppingCartCommand command = _mapper.Map<CreateShoppingCartCommand>(request);
            command.Metadata = new MessageMetadata(requestId, Guid.Empty, correlationId);

            Result<ShoppingCart> result = await _commandBus.SendAsync(command);

            return HandleResult(result, Created);
        }

        [HttpDelete]
        [Route("{shoppingCartId:guid}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteShoppingCartAsync
        (
            [FromRoute] Guid shoppingCartId,
            [FromHeader(Name = "x-request-id")] Guid requestId,
            [FromHeader(Name = "x-correlation-id")] Guid correlationId
        )
        {
            DeleteShoppingCartCommand command = new
            (
                shoppingCartId,
                new MessageMetadata(requestId, Guid.Empty, correlationId)
            );

            Result result = await _commandBus.SendAsync(command);
        
            return HandleResult(result, NoContent);
        }
        
        [HttpPut]
        [Route("{shoppingCartId:guid}/actions/checkout")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CheckoutResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CheckoutShoppingCartAsync
        (
            [FromRoute] Guid shoppingCartId,
            [FromHeader(Name = "x-request-id")] Guid requestId,
            [FromHeader(Name = "x-correlation-id")] Guid correlationId
        )
        {
            CheckoutShoppingCartCommand command = new
            (
                shoppingCartId,
                new MessageMetadata(requestId, Guid.Empty, correlationId)
            );

            Result<CheckoutResponse> result = await _commandBus.SendAsync(command);
        
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
            [FromBody] AddShoppingCartProductRequest request,
            [FromHeader(Name = "x-request-id")] Guid requestId,
            [FromHeader(Name = "x-correlation-id")] Guid correlationId
        )
        {
            AddShoppingCartProductCommand command = new
            (
                shoppingCartId,
                _mapper.Map<ShoppingCartItemCommand>(request),
                new MessageMetadata(requestId, Guid.Empty, correlationId)
            )
            {
                ShoppingCartItem = { ProductId = productId }
            };

            Result result = await _commandBus.SendAsync(command);
        
            return HandleResult(result, Created);
        }
        
        [HttpPut]
        [Route("{shoppingCartId:guid}/products/{productId:guid}")]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> SetProductPriceAsync
        (
            [FromRoute] Guid shoppingCartId,
            [FromRoute] Guid productId,
            [FromBody] SetShoppingCartProductPriceRequest request,
            [FromHeader(Name = "x-request-id")] Guid requestId,
            [FromHeader(Name = "x-correlation-id")] Guid correlationId
        )
        {
            SetShoppingCartProductPriceCommand command = _mapper.Map<SetShoppingCartProductPriceCommand>(request);
            command.ShoppingCartId = shoppingCartId;
            command.ProductId = productId;
            command.Metadata = new MessageMetadata(requestId, Guid.Empty, correlationId);
                
            Result commandResult = await _commandBus.SendAsync(command);
        
            return HandleResult(commandResult, NoContent);
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
            [FromBody] RemoveShoppingCartProductRequest request,
            [FromHeader(Name = "x-request-id")] Guid requestId,
            [FromHeader(Name = "x-correlation-id")] Guid correlationId
        )
        {
            RemoveShoppingCartProductCommand command = _mapper.Map<RemoveShoppingCartProductCommand>(request);
            command.ShoppingCartId = shoppingCartId;
            command.ProductId = productId;
            command.Metadata = new MessageMetadata(requestId, Guid.Empty, correlationId);

            Result commandResult = await _commandBus.SendAsync(command);
        
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
            [FromBody] SetContactInformationRequest request,
            [FromHeader(Name = "x-request-id")] Guid requestId,
            [FromHeader(Name = "x-correlation-id")] Guid correlationId
        )
        {
            SetContactInformationCommand command = _mapper.Map<SetContactInformationCommand>(request);
            command.ShoppingCartId = shoppingCartId;
            command.Metadata = new MessageMetadata(requestId, Guid.Empty, correlationId);
            
            Result result = await _commandBus.SendAsync(command);
        
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
            [FromBody] SetDeliveryAddressRequest request,
            [FromHeader(Name = "x-request-id")] Guid requestId,
            [FromHeader(Name = "x-correlation-id")] Guid correlationId
        )
        {
            SetDeliveryAddressCommand command = _mapper.Map<SetDeliveryAddressCommand>(request);
            command.ShoppingCartId = shoppingCartId;
            command.Metadata = new MessageMetadata(requestId, Guid.Empty, correlationId);

            Result result = await _commandBus.SendAsync(command);
        
            return HandleResult(result, Ok);
        }
    }
}