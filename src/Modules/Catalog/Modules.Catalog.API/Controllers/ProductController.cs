using System;
using System.Net;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.Application;
using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Parameters.Paging;
using VShop.SharedKernel.Infrastructure.Parameters.Options;
using VShop.SharedKernel.Infrastructure.Parameters.Sorting;
using VShop.Modules.Catalog.API.Models;
using VShop.Modules.Catalog.Infrastructure.DAL;
using VShop.Modules.Catalog.Infrastructure.DAL.Entities;

namespace VShop.Modules.Catalog.API.Controllers
{
    [ApiController]
    [Route("api/catalog/products")]
    internal class ProductController : ApplicationControllerBase
    {
        private readonly IMapper _mapper;
        private readonly CatalogDbContext _catalogDbContext;
        
        public ProductController(IMapper mapper, CatalogDbContext catalogDbContext)
        {
            _mapper = mapper;
            _catalogDbContext = catalogDbContext;
        }
        
        [HttpPost]
        [Route("")]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateProductAsync([FromBody] ProductRequest request)
        {
            CatalogProduct product = _mapper.Map<CatalogProduct>(request);
            product.Id = SequentialGuid.Create();
            product.IsDeleted = false;

            await _catalogDbContext.AddAsync(product);
            await _catalogDbContext.SaveChangesAsync();

            return Created(product);
        }
        
        [HttpPatch]
        [Route("{productId:guid}")]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> UpdateProductAsync
        (
            [FromRoute] Guid productId,
            [FromBody] ProductRequest request
        )
        {
            CatalogProduct product = await _catalogDbContext.Products.FindAsync(productId);
            
            if (product is null || product.IsDeleted is true)
                return NotFound("Requested product cannot be found.");

            _mapper.Map(request, product);

            _catalogDbContext.Update(product);
            await _catalogDbContext.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpDelete]
        [Route("{productId:guid}")]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteProductAsync([FromRoute] Guid productId)
        {
            CatalogProduct product = await _catalogDbContext.Products.FindAsync(productId);
            
            if (product is null || product.IsDeleted is true)
                return NotFound("Requested product cannot be found.");

            product.IsDeleted = true;

            _catalogDbContext.Update(product);
            await _catalogDbContext.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpGet]
        [Route("{productId:Guid}")]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProductAsync
        (
            [FromRoute] Guid productId,
            [FromQuery] string include = DefaultParameters.Include
        )
        {
            CatalogProduct product = await _catalogDbContext.Products
                .Include(OptionsFactory.Create(include))
                .SingleOrDefaultAsync(c => c.Id == productId);
            
            if (product is null || product.IsDeleted is true)
                return NotFound("Requested product cannot be found.");

            return Ok(product);
        }
        
        [HttpGet]
        [Route("")]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> GetProductsAsync
        (
            [FromQuery] string include = DefaultParameters.Include,
            [FromQuery] string searchPhrase = DefaultParameters.SearchPhrase,
            [FromQuery] int pageIndex = DefaultParameters.PageIndex,
            [FromQuery] int pageSize = DefaultParameters.PageSize,
            [FromQuery] string sortOrder = DefaultParameters.SortOrder,
            [FromQuery] string ids = null
        )
        {
            Result<Expression<Func<CatalogProduct, bool>>> getProductsByIdsExpressionResult = GetProductsByIdsExpression(ids);
            if (getProductsByIdsExpressionResult.IsError) return BadRequest(getProductsByIdsExpressionResult.Error.ToString());

            IList<CatalogProduct> products = await _catalogDbContext.Products
                .OrderBy(SortingFactory.Create(sortOrder))
                .Filter(p => p.IsDeleted == false)
                .Filter(searchPhrase, nameof(CatalogProduct.Name))
                .Filter(getProductsByIdsExpressionResult.Data)
                .Include(OptionsFactory.Create(include))
                .SkipAndTake(PagingFactory.Create(pageIndex, pageSize))
                .ToListAsync();

            if (products.Count is 0) return NoContent();

            PagedItemsResponse<CatalogProduct> pagedList = new(pageIndex, pageSize, products.Count, products);
            
            return Ok(pagedList);
        }
        
        private static Result<Expression<Func<CatalogProduct, bool>>> GetProductsByIdsExpression(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids)) return null;

            IList<(bool Ok, Guid Value)> guidIds = ids.Split(',').Select(id =>
            (
                Ok: Guid.TryParse(id, out Guid value),
                Value: value
            )).ToList();

            if (!guidIds.All(id => id.Ok)) 
                return Result.ValidationError("ids value invalid. Must be comma-separated list of guids.");

            IEnumerable<Guid> idsToSelect = guidIds.Select(id => id.Value);

            Expression<Func<CatalogProduct, bool>> filterExpression = ci => idsToSelect.Contains(ci.Id);
            
            return filterExpression;
        }
    }
}