using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using VShop.SharedKernel.Application;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Helpers;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.Modules.Sales.API.Models;
using VShop.Modules.Sales.API.Application.Queries;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Sales.API.Application.Commands.Shared;
using VShop.Modules.Sales.Infrastructure.Entities;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.SharedKernel.Domain.ValueObjects;

namespace VShop.Modules.Sales.API.Controllers
{
    [ApiController]
    [Route("api/shopping-carts")]
    public class ShoppingCartController : ApplicationControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly IShoppingCartQueryService _queryService;

        public ShoppingCartController
        (
            ICommandBus commandBus,
            IShoppingCartQueryService queryService
        )
        {
            _commandBus = commandBus;
            _queryService = queryService;
        }
        
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
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
        public async Task<IActionResult> CreateShoppingCartAsync([FromBody] CreateShoppingCartRequest request)
        {   
            // TODO - uncomment; should this check be performed by API gateway, command handler or bounded context API?
            // ShoppingCartInfo shoppingCart = await _queryService.GetActiveShoppingCartByCustomerIdAsync(request.CustomerId);
            // if (shoppingCart is not null)
            // {
            //     return BadRequest("Only one active shopping cart is supported per customer.");
            // }

            // Are CQRS commands part of the domain model?
            // https://enterprisecraftsmanship.com/posts/cqrs-commands-part-domain-model/
            // https://enterprisecraftsmanship.com/posts/combining-asp-net-core-attributes-with-value-objects/
            CreateShoppingCartCommand command = new()
            {
                ShoppingCartId = EntityId.Create(request.ShoppingCartId).Data,
                CustomerId = EntityId.Create(request.CustomerId).Data,
                CustomerDiscount = Discount.Create(request.CustomerDiscount).Data,
                ShoppingCartItems = new List<ShoppingCartItemCommandDto>(),
                CorrelationId = SequentialGuid.Create()
            };

            foreach (CreateShoppingCartProductRequest shoppingCartItem in request.ShoppingCartItems)
            {
                command.ShoppingCartItems.Add(new ShoppingCartItemCommandDto()
                {
                    ProductId = EntityId.Create(shoppingCartItem.ProductId).Data,
                    UnitPrice = Price.Create(shoppingCartItem.UnitPrice).Data,
                    Quantity = ProductQuantity.Create(shoppingCartItem.Quantity).Data,
                });
            }

            Result<ShoppingCart> result = await _commandBus.SendAsync(command);

            return HandleResult(result, Created);
        }
        
        [HttpDelete]
        [Route("{shoppingCartId:guid}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteShoppingCartAsync([FromRoute] Guid shoppingCartId)
        {
            DeleteShoppingCartCommand command = new(EntityId.Create(shoppingCartId).Data)
            {
                CorrelationId = SequentialGuid.Create()
            };

            Result result = await _commandBus.SendAsync(command);

            return HandleResult(result, NoContent);
        }
        
        [HttpPut]
        [Route("{shoppingCartId:guid}/actions/checkout")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CheckoutOrder), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CheckoutShoppingCartAsync([FromRoute] Guid shoppingCartId)
        {
            CheckoutShoppingCartCommand command = new(EntityId.Create(shoppingCartId).Data)
            {
                CorrelationId = SequentialGuid.Create()
            };

            Result<CheckoutOrder> result = await _commandBus.SendAsync(command);

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
            AddShoppingCartProductCommand command = new()
            {
                ShoppingCartItem = new ShoppingCartItemCommandDto
                {
                    Quantity = ProductQuantity.Create(request.Quantity).Data,
                    ProductId = EntityId.Create(productId).Data,
                    UnitPrice = Price.Create(request.UnitPrice).Data
                },
                ShoppingCartId = EntityId.Create(shoppingCartId).Data,
                CorrelationId = SequentialGuid.Create()
            };

            Result result = await _commandBus.SendAsync(command);

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
            RemoveShoppingCartProductCommand command = new()
            {
                ShoppingCartId = EntityId.Create(shoppingCartId).Data,
                ProductId = EntityId.Create(productId).Data,
                Quantity = ProductQuantity.Create(request.Quantity).Data,
                CorrelationId = SequentialGuid.Create()
            };
            
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
            [FromBody] SetContactInformationRequest request
        )
        {
            Result<FullName> fullNameResult = FullName.Create(request.FirstName, request.MiddleName, request.LastName);
            if (fullNameResult.IsError) return HandleError(fullNameResult.Error);

            SetContactInformationCommand command = new()
            {
                ShoppingCartId = EntityId.Create(shoppingCartId).Data,
                FullName = fullNameResult.Data,
                PhoneNumber = PhoneNumber.Create(request.PhoneNumber).Data,
                EmailAddress = EmailAddress.Create(request.EmailAddress).Data,
                Gender = request.Gender,
                CorrelationId = SequentialGuid.Create()
            };
            
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
            [FromBody] SetDeliveryAddressRequest request
        )
        {
            Result<Address> addressResult = Address.Create
            (
                request.City,
                request.CountryCode,
                request.PostalCode,
                request.StateProvince,
                request.StreetAddress
            );
            if (addressResult.IsError) return HandleError(addressResult.Error);

            SetDeliveryAddressCommand command = new()
            {
                ShoppingCartId = EntityId.Create(shoppingCartId).Data,
                Address = addressResult.Data,
                CorrelationId = SequentialGuid.Create()
            };

            Result result = await _commandBus.SendAsync(command);

            return HandleResult(result, Ok);
        }
    }
}