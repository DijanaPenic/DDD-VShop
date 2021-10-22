﻿using OneOf;
using OneOf.Types;
using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

using VShop.Services.Basket.API.Models;
using VShop.Services.Basket.API.Application.Queries;
using VShop.Services.Basket.API.Application.Commands;
using VShop.Services.Basket.Infrastructure.Entities;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Errors;

namespace VShop.Services.Basket.API.Controllers
{
    [ApiController]
    [Route("api/baskets")]
    // TODO - change query parameters to snake case
    // TODO - resolve enum by value
    public class BasketController : ApplicationControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IBasketQueryService _queryService;

        public BasketController
        (
            IMediator mediator,
            IMapper mapper,
            IBasketQueryService queryService
        )
        {
            _mediator = mediator;
            _mapper = mapper;
            _queryService = queryService;
        }
        
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BasketDetails), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetBasketAsync([FromQuery]Guid customerId)
        {
            BasketDetails basket = await _queryService.GetActiveBasketByCustomerIdAsync(customerId);
            if (basket == null)
            {
                return NotFound();
            }
            
            return Ok(basket);
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(Domain.Models.BasketAggregate.Basket), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateBasketAsync([FromBody]CreateBasketRequest request)
        {
            CreateBasketCommand command = _mapper.Map<CreateBasketCommand>(request);
            
            OneOf<Success<Domain.Models.BasketAggregate.Basket>, ApplicationError> result = await _mediator.Send(command);

            return HandleObjectResult(result, Created);
        }
        
        [HttpDelete]
        [Route("{basketId:guid}")]
        public Task<IActionResult> DeleteBasketAsync([FromRoute]Guid basketId)
        {
            throw new NotImplementedException();
        }
        
        [HttpPut]
        [Route("{basketId:guid}")]
        public Task<IActionResult> CheckoutBasketAsync([FromRoute]Guid basketId)
        {
            throw new NotImplementedException();
        }
        
        [HttpPost]
        [Route("{basketId:guid}/products")]
        [Consumes("application/json")]
        public async Task<IActionResult> AddProductAsync([FromRoute]Guid basketId, [FromBody]AddBasketProductRequest request)
        {
            AddBasketProductCommand command = _mapper.Map<AddBasketProductCommand>(request);
            command.BasketId = basketId;
            
            OneOf<Success, ApplicationError> result = await _mediator.Send(command);

            return HandleResult(result, Created);
        }
        
        [HttpDelete]
        [Route("{basketId:guid}/products/{productId:guid}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RemoveProductAsync([FromRoute]Guid basketId, [FromRoute]Guid productId)
        {
            RemoveBasketProductCommand command = new()
            {
                BasketId = basketId,
                ProductId = productId
            };
            
            OneOf<Success, ApplicationError> result = await _mediator.Send(command);

            return HandleResult(result, NoContent);
        }
        
        [HttpPut]
        [Route("{basketId:guid}/products/{productId:guid}/actions/increase")]
        public Task<IActionResult> IncreaseProductQuantityAsync([FromRoute]Guid basketId, [FromRoute]Guid productId)
        {
            throw new NotImplementedException();
        }
        
        [HttpPut]
        [Route("{basketId:guid}/products/{productId:guid}/actions/decrease")]
        public Task<IActionResult> DecreaseProductQuantityAsync([FromRoute]Guid basketId, [FromRoute]Guid productId)
        {
            throw new NotImplementedException();
        }
        
        [HttpPut]
        [Route("{basketId:guid}/customer/contact-information")]
        public Task<IActionResult> SetContactInformationAsync([FromRoute]Guid basketId)
        {
            throw new NotImplementedException();
        }
        
        [HttpPut]
        [Route("{basketId:guid}/customer/delivery-address")]
        public Task<IActionResult> SetDeliveryAddressAsync([FromRoute]Guid basketId)
        {
            throw new NotImplementedException();
        }
    }
}