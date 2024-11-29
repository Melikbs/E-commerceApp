using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Statements;
using E_commerceApp.Server.Dtos;
using E_commerceApp.Server.Errors;
using E_commerceApp.Server.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace E_commerceApp.Server.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productsRepo;
        private readonly IGenericRepository<ProductBrand> _productBrandRepo;
        private readonly IGenericRepository<ProductType> _productTypeRepo;
        private readonly IMapper _mapper;

        public ProductsController(
            IGenericRepository<Product> productsRepo,
            IGenericRepository<ProductBrand> productBrandRepo,
            IGenericRepository<ProductType> productTypeRepo,
            IMapper mapper)
        {
            _productsRepo = productsRepo;
            _productBrandRepo = productBrandRepo;
            _productTypeRepo = productTypeRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts(
            string sort = "default",
            int? brandId = null,
            int? typeId = null,
            int pageIndex = 1,
            int pageSize = 10)
        {
            // Build the filtering/sorting specification
            var state = new ProductsWithTypesAndBrandsStatement(sort, brandId, typeId);

            // Get the filtered and sorted list of products
            var products = await _productsRepo.ListAsync(state);

            // Apply pagination
            var totalItems = products.Count();
            var paginatedProducts = products
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Map to DTOs
            var productsToReturn = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(paginatedProducts);

            // Build and return the pagination response
            var paginationResponse = new Pagination<ProductToReturnDto>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Count = totalItems,
                Data = productsToReturn
            };

            return Ok(paginationResponse);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var state = new ProductsWithTypesAndBrandsStatement(id);
            var product = await _productsRepo.GetEntityWithState(state);

            if (product == null) return NotFound(new ApiResponse(404));

            return _mapper.Map<Product, ProductToReturnDto>(product);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            return Ok(await _productBrandRepo.ListAllAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            return Ok(await _productTypeRepo.ListAllAsync());
        }
    }
}