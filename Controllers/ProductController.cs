using Asp.Versioning;
using CRNProductAPI.Interfaces;
using CRNProductAPI.Models.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace CRNProductAPI.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        #region Constructor
        private readonly IProductService _productService;
        private readonly IValidator<ProductCreateDto> _createValidator;
        private readonly IValidator<ProductUpdateDto> _updateValidator;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, IValidator<ProductCreateDto> createValidator, IValidator<ProductUpdateDto> updateValidator,
             ILogger<ProductsController> logger)
        {
            _productService = productService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }
        #endregion

        #region Get All Products with Pagination
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var (items, totalCount) = await _productService.GetAllProductsAsync(pageNumber, pageSize);

            var result = new
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = items
            };

            return Ok(result);
        }
        #endregion

        #region Get Product by Id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound(new { Message = $"Product with Id {id} not found" });

            return Ok(product);
        }
        #endregion

        #region Create Product
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });

            var created = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        #endregion

        #region Update Product
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });

            var updated = await _productService.UpdateProductAsync(id, dto);
            if (!updated)
                return NotFound(new { Message = $"Product with Id {id} not found" });

            return NoContent();
        }
        #endregion

        #region Delete Product
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productService.DeleteProductAsync(id);
            if (!deleted)
                return NotFound(new { Message = $"Product with Id {id} not found" });

            return NoContent();
        }
        #endregion
    }
}