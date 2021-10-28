﻿using OneOf;
using OneOf.Types;
using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

using VShop.SharedKernel.Application;
using VShop.SharedKernel.Application.Commands;
using VShop.SharedKernel.Infrastructure.Errors;
using VShop.Modules.Sales.API.Models;
using VShop.Modules.Sales.API.Application.Queries;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Sales.API.Application.Commands.Shared;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.Modules.Sales.Infrastructure.Entities;

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
        [ProducesResponseType(typeof(ShoppingCartInfo), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetShoppingCartAsync([FromQuery] Guid customerId)
        {
            ShoppingCartInfo shoppingCart = await _queryService.GetActiveShoppingCartByCustomerIdAsync(customerId);
            if (shoppingCart == null) return NotFound();

            return Ok(shoppingCart);
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateShoppingCartAsync([FromBody] CreateShoppingCartRequest request)
        {   
            // TODO - should this check be performed by API gateway, command handler or bounded context API?
            ShoppingCartInfo shoppingCart = await _queryService.GetActiveShoppingCartByCustomerIdAsync(request.CustomerId);
            if (shoppingCart != null)
            {
                return BadRequest("Only one active shopping cart is supported per customer.");
            }
            
            CreateShoppingCartCommand command = _mapper.Map<CreateShoppingCartCommand>(request);
            
            OneOf<Success<ShoppingCart>, ApplicationError> result = await _commandBus.Send(command);

            return HandleObjectResult(result, Created);
        }
        
        [HttpDelete]
        [Route("{shoppingCartId:guid}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteShoppingCartAsync([FromRoute] Guid shoppingCartId)
        {
            DeleteShoppingCartCommand command = new()
            {
                ShoppingCartId = shoppingCartId
            };

            OneOf<Success, ApplicationError> result = await _commandBus.Send(command);

            return HandleResult(result, NoContent);
        }
        
        [HttpPut]
        [Route("{shoppingCartId:guid}/actions/checkout")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> CheckoutShoppingCartAsync([FromRoute] Guid shoppingCartId)
        {
            CheckoutShoppingCartCommand command = new()
            {
                ShoppingCartId = shoppingCartId
            };

            OneOf<Success, ApplicationError> result = await _commandBus.Send(command);

            return HandleResult(result, NoContent);
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
            AddShoppingCartProductCommand command = new()
            {
                ShoppingCartItem = _mapper.Map<ShoppingCartItemDto>(request),
                ShoppingCartId = shoppingCartId
            };
            command.ShoppingCartItem.ProductId = productId;
            
            OneOf<Success, ApplicationError> result = await _commandBus.Send(command);

            return HandleResult(result, Created);
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
            
            OneOf<Success, ApplicationError> result = await _commandBus.Send(command);

            return HandleResult(result, NoContent);
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

            OneOf<Success, ApplicationError> result = await _commandBus.Send(command);

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

            OneOf<Success, ApplicationError> result = await _commandBus.Send(command);

            return HandleResult(result, Ok);
        }
    }
}