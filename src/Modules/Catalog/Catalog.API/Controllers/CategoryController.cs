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
    [Route("api/catalog/categories")]
    public class CategoryController : ApplicationControllerBase
    {
        private readonly IMapper _mapper;
        private readonly CatalogContext _catalogContext;
        
        public CategoryController(IMapper mapper, CatalogContext catalogContext)
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
        public async Task<IActionResult> CreateCategoryAsync([FromBody] CategoryRequest request)
        {
            CatalogCategory category = _mapper.Map<CatalogCategory>(request);
            category.Id = SequentialGuid.Create();
            category.IsDeleted = false;

            await _catalogContext.AddAsync(category);
            await _catalogContext.SaveChangesAsync();

            return Created(category);
        }
        
        [HttpPut]
        [Route("{categoryId:Guid}")]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateCategoryAsync([FromRoute] Guid categoryId, [FromBody] CategoryRequest request)
        {
            CatalogCategory category = await _catalogContext.Categories.FindAsync(categoryId);
            if (category is null)
                return BadRequest("Requested category cannot be found.");

            _mapper.Map(request, category);

            _catalogContext.Update(category);
            await _catalogContext.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpDelete]
        [Route("{categoryId:Guid}")]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteCategoryAsync([FromRoute] Guid categoryId)
        {
            CatalogCategory category = await _catalogContext.Categories.FindAsync(categoryId);
            if (category is null)
                return BadRequest("Requested category cannot be found.");

            category.IsDeleted = true;

            _catalogContext.Update(category);
            await _catalogContext.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpGet]
        [Route("{categoryId:Guid}")]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCategoryAsync([FromRoute] Guid categoryId)
        {
            CatalogCategory category = await _catalogContext.Categories.FindAsync(categoryId);
            if (category is null)
                return BadRequest("Requested category cannot be found.");

            return Ok(category);
        }
        
        [HttpGet]
        [Route("")]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCategoriesAsync
        (
            [FromQuery] string include = DefaultParameters.Include,
            [FromQuery] string searchString = DefaultParameters.SearchString,
            [FromQuery] int pageIndex = DefaultParameters.PageIndex,
            [FromQuery] int pageSize = DefaultParameters.PageSize,
            [FromQuery] string sortOrder = DefaultParameters.SortOrder
        )
        {
            IList<CatalogCategory> categories = await _catalogContext.Categories
                .OrderBy(SortingFactory.Create(sortOrder))
                .Filter(string.IsNullOrWhiteSpace(searchString) ? null : p => p.Name.Contains(searchString))
                .Include(OptionsFactory.Create(include))
                .SkipAndTake(PagingFactory.Create(pageIndex, pageSize))
                .ToListAsync();

            if (categories.Count is 0) return NoContent();

            PagedItemsResponse<CatalogCategory> pagedList = new(pageIndex, pageSize, categories.Count, categories);
            
            return Ok(pagedList);
        }
        
        [HttpGet]
        [Route("{categoryId:Guid}/products")]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCategoryProductsAsync
        (
            [FromRoute] Guid categoryId,
            [FromQuery] string include = DefaultParameters.Include,
            [FromQuery] string searchString = DefaultParameters.SearchString,
            [FromQuery] int pageIndex = DefaultParameters.PageIndex,
            [FromQuery] int pageSize = DefaultParameters.PageSize,
            [FromQuery] string sortOrder = DefaultParameters.SortOrder
        )
        {
            IList<CatalogProduct> products = await _catalogContext.Products
                .OrderBy(SortingFactory.Create(sortOrder))
                .Filter(c => c.IsDeleted == false && c.CategoryId == categoryId)
                .Filter(string.IsNullOrWhiteSpace(searchString) ? null : c => c.Name.Contains(searchString))
                .Include(OptionsFactory.Create(include))
                .SkipAndTake(PagingFactory.Create(pageIndex, pageSize))
                .ToListAsync();

            if (products.Count is 0) return NoContent();

            PagedItemsResponse<CatalogProduct> pagedList = new(pageIndex, pageSize, products.Count, products);
            
            return Ok(pagedList);
        }
    }
}