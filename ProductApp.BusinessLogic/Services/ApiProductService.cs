using ProductApp.BusinessLogic.Protos;
using ProductApp.BusinessLogic.Models;
using ProductApp.BusinessLogic.Repositories;


namespace ProductApp.BusinessLogic.Services
{
    public class ApiProductService
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<ApiProductService> _logger;

        public ApiProductService(IProductRepository repository, ILogger<ApiProductService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<List<Product>> GetAllProducts() => await _repository.GetAllProductsAsync();
        public async Task<Product?> GetProduct(int id) => await _repository.GetProductAsync(id);
        public async Task<Product> CreateProduct(Product product) => await _repository.CreateProductAsync(product);
        public async Task<Product?> UpdateProduct(Product product) => await _repository.UpdateProductAsync(product);
        public async Task<bool> DeleteProduct(int id) => await _repository.DeleteProductAsync(id);
    }
}