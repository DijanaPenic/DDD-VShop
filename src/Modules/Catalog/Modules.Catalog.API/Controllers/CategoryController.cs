using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.Application;
using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Parameters.Paging;
using VShop.SharedKernel.Infrastructure.Parameters.Options;
using VShop.SharedKernel.Infrastructure.Parameters.Sorting;
using VShop.Modules.Catalog.API.Models;
using VShop.Modules.Catalog.Infrastructure.DAL;
using VShop.Modules.Catalog.Infrastructure.DAL.Entities;

// TODO - missing integration and unit tests
namespace VShop.Modules.Catalog.API.Controllers
{
    [ApiController]
    [Route("api/catalog/categories")]
    [Authorize(Policy)]
    internal class CategoryController : ApplicationControllerBase
    {
        private const string Policy = "categories";
        private readonly IMapper _mapper;
        private readonly CatalogDbContext _catalogDbContext;
        
        public CategoryController(IMapper mapper, CatalogDbContext catalogDbContext)
        {
            _mapper = mapper;
            _catalogDbContext = catalogDbContext;
        }
        
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateCategoryAsync([FromBody] CategoryRequest request)
        {
            CatalogCategory category = _mapper.Map<CatalogCategory>(request);
            category.Id = SequentialGuid.Create();
            category.IsDeleted = false;

            await _catalogDbContext.AddAsync(category);
            await _catalogDbContext.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.Id });
        }
        
        [HttpPatch]
        [Route("{categoryId:guid}")]
        [Consumes("application/json")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> UpdateCategoryAsync
        (
            [FromRoute] Guid categoryId,
            [FromBody] CategoryRequest request
        )
        {
            CatalogCategory category = await _catalogDbContext.Categories.FindAsync(categoryId);
            
            if (category is null || category.IsDeleted)
                return NotFound("Requested category cannot be found.");

            _mapper.Map(request, category);

            _catalogDbContext.Update(category);
            await _catalogDbContext.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpDelete]
        [Route("{categoryId:guid}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteCategoryAsync([FromRoute] Guid categoryId)
        {
            CatalogCategory category = await _catalogDbContext.Categories.FindAsync(categoryId);
            
            if (category is null || category.IsDeleted)
                return NotFound("Requested category cannot be found.");

            category.IsDeleted = true;

            _catalogDbContext.Update(category);
            await _catalogDbContext.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpGet]
        [Route("{categoryId:guid}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(CatalogCategory), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCategoryAsync
        (
            [FromRoute] Guid categoryId,
            [FromQuery] string include = DefaultParameters.Include
        )
        {
            CatalogCategory category = await _catalogDbContext.Categories
                .Include(OptionsFactory.Create(include))
                .SingleOrDefaultAsync(c => c.Id == categoryId);

            if (category is null || category.IsDeleted)
                return NotFound("Requested category cannot be found.");
            
            return Ok(category);
        }
        
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(PagedItemsResponse<CatalogCategory>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCategoriesAsync
        (
            [FromQuery] string include = DefaultParameters.Include,
            [FromQuery] string searchPhrase = DefaultParameters.SearchPhrase,
            [FromQuery] int pageIndex = DefaultParameters.PageIndex,
            [FromQuery] int pageSize = DefaultParameters.PageSize,
            [FromQuery] string sortOrder = DefaultParameters.SortOrder
        )
        {
            IList<CatalogCategory> categories = await _catalogDbContext.Categories
                .OrderBy(SortingFactory.Create(sortOrder))
                .Filter(p => p.IsDeleted == false) // TODO - false check :D
                .Filter(searchPhrase, nameof(CatalogCategory.Name))
                .Include(OptionsFactory.Create(include))
                .SkipAndTake(PagingFactory.Create(pageIndex, pageSize))
                .ToListAsync();

            if (categories.Count is 0) return NoContent();

            // TODO - count is not correct.
            PagedItemsResponse<CatalogCategory> pagedList = new(pageIndex, pageSize, categories.Count, categories);
            
            return Ok(pagedList);
        }
        
        [HttpGet]
        [Route("{categoryId:Guid}/products")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(PagedItemsResponse<CatalogProduct>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> GetCategoryProductsAsync
        (
            [FromRoute] Guid categoryId,
            [FromQuery] string include = DefaultParameters.Include,
            [FromQuery] string searchPhrase = DefaultParameters.SearchPhrase,
            [FromQuery] int pageIndex = DefaultParameters.PageIndex,
            [FromQuery] int pageSize = DefaultParameters.PageSize,
            [FromQuery] string sortOrder = DefaultParameters.SortOrder
        )
        {
            IList<CatalogProduct> products = await _catalogDbContext.Products
                .OrderBy(SortingFactory.Create(sortOrder))
                .Filter(p => p.IsDeleted == false && p.CategoryId == categoryId)
                .Filter(searchPhrase, nameof(CatalogCategory.Name))
                .Include(OptionsFactory.Create(include))
                .SkipAndTake(PagingFactory.Create(pageIndex, pageSize))
                .ToListAsync();

            if (products.Count is 0) return NoContent();

            PagedItemsResponse<CatalogProduct> pagedList = new(pageIndex, pageSize, products.Count, products);
            
            return Ok(pagedList);
        }
    }
}