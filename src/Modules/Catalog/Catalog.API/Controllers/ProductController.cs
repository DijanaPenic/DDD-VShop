using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.Application;
using VShop.Modules.Catalog.API.Models;
using VShop.Modules.Catalog.API.Application;
using VShop.Modules.Catalog.Infrastructure;
using VShop.Modules.Catalog.Infrastructure.Entities;
using VShop.SharedKernel.Infrastructure.Helpers;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Parameters.Paging;
using VShop.SharedKernel.Infrastructure.Parameters.Options;
using VShop.SharedKernel.Infrastructure.Parameters.Sorting;

namespace VShop.Modules.Catalog.API.Controllers
{
    [ApiController]
    [Route("api/catalog/products")]
    public class ProductController : ApplicationControllerBase
    {
        private readonly IMapper _mapper;
        private readonly CatalogContext _catalogContext;
        
        public ProductController(IMapper mapper, CatalogContext catalogContext)
        {
            _mapper = mapper;
            _catalogContext = catalogContext;
        }
        
        [HttpPost]
        [Route("")]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateProductAsync([FromBody] ProductRequest request)
        {
            CatalogProduct product = _mapper.Map<CatalogProduct>(request);
            product.Id = SequentialGuid.Create();
            product.IsDeleted = false;

            await _catalogContext.AddAsync(product);
            await _catalogContext.SaveChangesAsync();

            return Created(product);
        }
        
        [HttpPut]
        [Route("{productId:Guid}")]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateProductAsync([FromRoute] Guid productId, [FromBody] ProductRequest request)
        {
            CatalogProduct product = await _catalogContext.Products.FindAsync(productId);
            if (product is null)
                return BadRequest("Requested product cannot be found.");

            _mapper.Map(request, product);

            _catalogContext.Update(product);
            await _catalogContext.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpDelete]
        [Route("{productId:Guid}")]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteProductAsync([FromRoute] Guid productId)
        {
            CatalogProduct product = await _catalogContext.Products.FindAsync(productId);
            if (product is null)
                return BadRequest("Requested product cannot be found.");

            product.IsDeleted = true;

            _catalogContext.Update(product);
            await _catalogContext.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpGet]
        [Route("{productId:Guid}")]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProductAsync([FromRoute] Guid productId)
        {
            CatalogProduct product = await _catalogContext.Products.FindAsync(productId);
            if (product is null)
                return BadRequest("Requested product cannot be found.");

            return Ok(product);
        }
        
        [HttpGet]
        [Route("")]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProductsAsync
        (
            [FromQuery] string include = DefaultParameters.Include,
            [FromQuery] string searchString = DefaultParameters.SearchString,
            [FromQuery] int pageIndex = DefaultParameters.PageIndex,
            [FromQuery] int pageSize = DefaultParameters.PageSize,
            [FromQuery] string sortOrder = DefaultParameters.SortOrder
        )
        {
            IList<CatalogProduct> products = await _catalogContext.Products
                .OrderBy(SortingFactory.Create(sortOrder))
                .Filter(p => p.IsDeleted == false)
                .Filter(string.IsNullOrWhiteSpace(searchString) ? null : p => p.Name.Contains(searchString))
                .Include(OptionsFactory.Create(include))
                .SkipAndTake(PagingFactory.Create(pageIndex, pageSize))
                .ToListAsync();

            if (products.Count is 0) return NoContent();

            PagedItemsResponse<CatalogProduct> pagedList = new(pageIndex, pageSize, products.Count, products);
            
            return Ok(pagedList);
        }
    }
}