using CRNProductAPI.Models.DTOs;

namespace CRNProductAPI.Interfaces
{
    public interface IProductService
    {
        Task<(IEnumerable<ProductResponseDto> Items, int TotalCount)> GetAllProductsAsync(int pageNumber, int pageSize);
        Task<ProductResponseDto?> GetProductByIdAsync(int id);
        Task<ProductResponseDto> CreateProductAsync(ProductCreateDto dto);
        Task<bool> UpdateProductAsync(int id, ProductUpdateDto dto);
        Task<bool> DeleteProductAsync(int id);
    }
}