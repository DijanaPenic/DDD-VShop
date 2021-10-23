using OneOf;
using OneOf.Types;
using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

using VShop.Services.ShoppingCarts.API.Models;
using VShop.Services.ShoppingCarts.API.Application.Queries;
using VShop.Services.ShoppingCarts.API.Application.Commands;
using VShop.Services.ShoppingCarts.Infrastructure.Entities;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;

namespace VShop.Services.ShoppingCarts.API.Controllers
{
    [ApiController]
    [Route("api/shopping-carts")]
    // TODO - change query parameters to snake case
    // TODO - resolve enum by value
    public class ShoppingCartController : ApplicationControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IShoppingCartQueryService _queryService;

        public ShoppingCartController
        (
            IMediator mediator,
            IMapper mapper,
            IShoppingCartQueryService queryService
        )
        {
            _mediator = mediator;
            _mapper = mapper;
            _queryService = queryService;
        }
        
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetShoppingCartAsync([FromQuery]Guid customerId)
        {
            ShoppingCart shoppingCart = await _queryService.GetActiveShoppingCartByCustomerIdAsync(customerId);
            if (shoppingCart == null)
            {
                return NotFound();
            }
            
            return Ok(shoppingCart);
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(Domain.Models.ShoppingCartAggregate.ShoppingCart), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateShoppingCartAsync([FromBody]CreateShoppingCartRequest request)
        {
            CreateShoppingCartCommand command = _mapper.Map<CreateShoppingCartCommand>(request);
            
            OneOf<Success<Domain.Models.ShoppingCartAggregate.ShoppingCart>, ApplicationError> result = await _mediator.Send(command);

            return HandleObjectResult(result, Created);
        }
        
        [HttpDelete]
        [Route("{shoppingCartId:guid}")]
        public Task<IActionResult> DeleteShoppingCartAsync([FromRoute]Guid shoppingCartId)
        {
            throw new NotImplementedException();
        }
        
        [HttpPut]
        [Route("{shoppingCartId:guid}")]
        public Task<IActionResult> CheckoutShoppingCartAsync([FromRoute]Guid shoppingCartId)
        {
            throw new NotImplementedException();
        }
        
        [HttpPost]
        [Route("{shoppingCartId:guid}/products")]
        [Consumes("application/json")]
        public async Task<IActionResult> AddProductAsync([FromRoute]Guid shoppingCartId, [FromBody]AddShoppingCartProductRequest request)
        {
            AddShoppingCartProductCommand command = _mapper.Map<AddShoppingCartProductCommand>(request);
            command.ShoppingCartId = shoppingCartId;
            
            OneOf<Success, ApplicationError> result = await _mediator.Send(command);

            return HandleResult(result, Created);
        }
        
        [HttpDelete]
        [Route("{shoppingCartId:guid}/products/{productId:guid}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RemoveProductAsync([FromRoute]Guid shoppingCartId, [FromRoute]Guid productId)
        {
            RemoveShoppingCartProductCommand command = new()
            {
                ShoppingCartId = shoppingCartId,
                ProductId = productId
            };
            
            OneOf<Success, ApplicationError> result = await _mediator.Send(command);

            return HandleResult(result, NoContent);
        }
        
        [HttpPut]
        [Route("{shoppingCartId:guid}/products/{productId:guid}/actions/increase")]
        public Task<IActionResult> IncreaseProductQuantityAsync([FromRoute]Guid shoppingCartId, [FromRoute]Guid productId)
        {
            throw new NotImplementedException();
        }
        
        [HttpPut]
        [Route("{shoppingCartId:guid}/products/{productId:guid}/actions/decrease")]
        public Task<IActionResult> DecreaseProductQuantityAsync([FromRoute]Guid shoppingCartId, [FromRoute]Guid productId)
        {
            throw new NotImplementedException();
        }
        
        [HttpPut]
        [Route("{shoppingCartId:guid}/customer/contact-information")]
        public Task<IActionResult> SetContactInformationAsync([FromRoute]Guid shoppingCartId)
        {
            throw new NotImplementedException();
        }
        
        [HttpPut]
        [Route("{shoppingCartId:guid}/customer/delivery-address")]
        public Task<IActionResult> SetDeliveryAddressAsync([FromRoute]Guid shoppingCartId)
        {
            throw new NotImplementedException();
        }
    }
}