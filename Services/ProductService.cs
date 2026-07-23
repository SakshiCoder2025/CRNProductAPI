using CRNProductAPI.Interfaces;
using CRNProductAPI.Models;
using CRNProductAPI.Models.DTOs;

namespace CRNProductAPI.Services
{
    public class ProductService : IProductService
    {
        #region Constructor
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository repository, ILogger<ProductService> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        #endregion
        public async Task<(IEnumerable<ProductResponseDto> Items, int TotalCount)> GetAllProductsAsync(int pageNumber, int pageSize)
        {
            try
            {
                var products = await _repository.GetAllAsync(pageNumber, pageSize);
                var totalCount = await _repository.GetTotalCountAsync();

                var dtos = products.Select(MapToDto);
                return (dtos, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching products");
                throw;
            }
        }
        public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _repository.GetByIdAsync(id);
                return product == null ? null : MapToDto(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching product with Id {Id}", id);
                throw;
            }
        }
        public async Task<ProductResponseDto> CreateProductAsync(ProductCreateDto dto)
        {
            try
            {
                var product = new Product
                {
                    ProductName = dto.ProductName,
                    CreatedBy = dto.CreatedBy,
                    CreatedOn = DateTime.UtcNow
                };

                var created = await _repository.AddAsync(product);
                _logger.LogInformation("Product created with Id {Id}", created.Id);
                return MapToDto(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating product");
                throw;
            }
        }
        public async Task<bool> UpdateProductAsync(int id, ProductUpdateDto dto)
        {
            try
            {
                var product = await _repository.GetByIdAsync(id);
                if (product == null)
                    return false;

                product.ProductName = dto.ProductName;
                product.ModifiedBy = dto.ModifiedBy;
                product.ModifiedOn = DateTime.UtcNow;

                await _repository.UpdateAsync(product);
                _logger.LogInformation("Product updated with Id {Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product with Id {Id}", id);
                throw;
            }
        }
        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _repository.GetByIdAsync(id);
                if (product == null)
                    return false;

                await _repository.DeleteAsync(product);
                _logger.LogInformation("Product deleted with Id {Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product with Id {Id}", id);
                throw;
            }
        }
        private static ProductResponseDto MapToDto(Product product)
        {
            return new ProductResponseDto
            {
                Id = product.Id,
                ProductName = product.ProductName,
                CreatedBy = product.CreatedBy,
                CreatedOn = product.CreatedOn,
                ModifiedBy = product.ModifiedBy,
                ModifiedOn = product.ModifiedOn
            };
        }
    }
}