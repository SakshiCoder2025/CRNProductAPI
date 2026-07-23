using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using CRNProductAPI.Interfaces;
using CRNProductAPI.Models;
using CRNProductAPI.Models.DTOs;
using CRNProductAPI.Services;

namespace CRNProductAPI.Tests
{
    public class ProductServiceTests
    {
        #region Constructor 
        private readonly Mock<IProductRepository> _mockRepository;
        private readonly Mock<ILogger<ProductService>> _mockLogger;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
           _mockRepository = new Mock<IProductRepository>();
            _mockLogger = new Mock<ILogger<ProductService>>();
            _productService = new ProductService(_mockRepository.Object, _mockLogger.Object);
        }
        #endregion
        [Fact]
        public async Task CreateProductAsync_ValidData_ReturnsProductWithId()
        {
            var createDto = new ProductCreateDto
            {
                ProductName = "Laptop",
                CreatedBy = "TestUser"
            };

            var savedProduct = new Product
            {
                Id = 1,
                ProductName = "Laptop",
                CreatedBy = "TestUser",
                CreatedOn = DateTime.UtcNow
            };

            _mockRepository
                .Setup(repo => repo.AddAsync(It.IsAny<Product>()))
                .ReturnsAsync(savedProduct);

            var result = await _productService.CreateProductAsync(createDto);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Laptop", result.ProductName);
            Assert.Equal("TestUser", result.CreatedBy);
        }

        [Fact]
        public async Task GetProductByIdAsync_ExistingId_ReturnsProduct()
        {
            var existingProduct = new Product
            {
                Id = 5,
                ProductName = "Mobile",
                CreatedBy = "TestUser",
                CreatedOn = DateTime.UtcNow
            };

            _mockRepository
                .Setup(repo => repo.GetByIdAsync(5))
                .ReturnsAsync(existingProduct);

            var result = await _productService.GetProductByIdAsync(5);

            Assert.NotNull(result);
            Assert.Equal("Mobile", result!.ProductName);
        }

        [Fact]
        public async Task GetProductByIdAsync_NonExistingId_ReturnsNull()
        {
            _mockRepository
                .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Product?)null);

            var result = await _productService.GetProductByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteProductAsync_ExistingId_ReturnsTrue()
        {
            var existingProduct = new Product { Id = 3, ProductName = "Tablet", CreatedBy = "TestUser" };

            _mockRepository
                .Setup(repo => repo.GetByIdAsync(3))
                .ReturnsAsync(existingProduct);

            _mockRepository
                .Setup(repo => repo.DeleteAsync(existingProduct))
                .Returns(Task.CompletedTask);
            var result = await _productService.DeleteProductAsync(3);
            Assert.True(result);
            _mockRepository.Verify(repo => repo.DeleteAsync(existingProduct), Times.Once);
        }

        [Fact]
        public async Task DeleteProductAsync_NonExistingId_ReturnsFalse()
        {
            _mockRepository
                .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Product?)null);
            var result = await _productService.DeleteProductAsync(999);
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateProductAsync_ExistingId_ReturnsTrue()
        {
            var existingProduct = new Product { Id = 7, ProductName = "Old Name", CreatedBy = "TestUser" };
            var updateDto = new ProductUpdateDto { ProductName = "New Name", ModifiedBy = "Updater" };

            _mockRepository
                .Setup(repo => repo.GetByIdAsync(7))
                .ReturnsAsync(existingProduct);

            _mockRepository
                .Setup(repo => repo.UpdateAsync(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);
            var result = await _productService.UpdateProductAsync(7, updateDto);
            Assert.True(result);
            Assert.Equal("New Name", existingProduct.ProductName);
            Assert.Equal("Updater", existingProduct.ModifiedBy);
        }
    }
}