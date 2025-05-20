using System.Text;
using Newtonsoft.Json;
using ProductApp.BusinessLogic.Models;

namespace ProductApp.BusinessLogic.Services
{
    public class ProductApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ProductApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:ProductsApiUrl"] ?? "https://localhost:7001/api/Products";
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            var response = await _httpClient.GetAsync(_baseUrl);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<Product>>(content) ?? new List<Product>();
        }

        public async Task<Product> GetProductAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Product>(content) ?? new Product();
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            var json = JsonConvert.SerializeObject(product);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_baseUrl, content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Product>(responseContent) ?? new Product();
        }

        public async Task<Product> UpdateProductAsync(int id, Product product)
        {
            var json = JsonConvert.SerializeObject(product);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseUrl}/{id}", content);
            response.EnsureSuccessStatusCode();
            return product;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}