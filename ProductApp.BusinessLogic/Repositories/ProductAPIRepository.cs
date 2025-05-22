using System.Text;
using System.Text.Json;
using ProductApp.BusinessLogic.Models;

namespace ProductApp.BusinessLogic.Repositories
{
    public class ProductApiRepository : IProductRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductApiRepository> _logger;

        public ProductApiRepository(IConfiguration configuration, ILogger<ProductApiRepository> logger)
        {
            _logger = logger;
            var baseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5206";
            _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/Products");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Product>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<Product>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos");
                throw;
            }
        }

        public async Task<Product?> GetProductAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Products/{id}");
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
                
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Product>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener producto {id}");
                throw;
            }
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            try
            {
                var json = JsonSerializer.Serialize(product);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/Products", content);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Product>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? throw new Exception("Error al deserializar la respuesta");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                throw;
            }
        }

        public async Task<Product?> UpdateProductAsync(Product product)
        {
            try
            {
                var json = JsonSerializer.Serialize(product);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"/api/Products/{product.Id}", content);
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
                
                response.EnsureSuccessStatusCode();
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar producto {product.Id}");
                throw;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/Products/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar producto {id}");
                throw;
            }
        }
    }
}