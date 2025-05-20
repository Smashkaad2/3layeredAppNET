using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using ProductApp.BusinessLogic.Protos;
using ProductApp.Presentation.Models;

namespace ProductApp.Presentation.Services
{
    public class ProductGrpcClientService
    {
        private readonly ProductGrpcService.ProductGrpcServiceClient _client;

        public ProductGrpcClientService(IConfiguration configuration)
        {
            var grpcUrl = configuration["GrpcSettings:ProductGrpcUrl"] ?? "https://localhost:7042";
            var channel = GrpcChannel.ForAddress(grpcUrl);
            _client = new ProductGrpcService.ProductGrpcServiceClient(channel);
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            var request = new EmptyRequest();
            var response = await _client.GetAllProductsAsync(request);

            var products = new List<Product>();
            foreach (var item in response.Products)
            {
                products.Add(new Product
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Price = (decimal)item.Price
                });
            }

            return products;
        }

        public async Task<Product?> GetProductAsync(int id)
        {
            var request = new ProductIdRequest { Id = id };
            
            try
            {
                var response = await _client.GetProductAsync(request);
                
                if (!response.Success)
                {
                    Console.WriteLine($"Error: {response.Message}");
                    return null;
                }

                return new Product
                {
                    Id = response.Id,
                    Name = response.Name,
                    Description = response.Description,
                    Price = (decimal)response.Price
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el producto: {ex.Message}");
                return null;
            }
        }

        public async Task<Product?> CreateProductAsync(Product product)
        {
            var request = new ProductRequest
            {
                Name = product.Name,
                Description = product.Description,
                Price = (double)product.Price
            };

            try
            {
                var response = await _client.CreateProductAsync(request);
                
                if (!response.Success)
                {
                    Console.WriteLine($"Error: {response.Message}");
                    return null;
                }

                return new Product
                {
                    Id = response.Id,
                    Name = response.Name,
                    Description = response.Description,
                    Price = (decimal)response.Price
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear el producto: {ex.Message}");
                return null;
            }
        }

        public async Task<Product?> UpdateProductAsync(Product product)
        {
            var request = new ProductRequest
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = (double)product.Price
            };

            try
            {
                var response = await _client.UpdateProductAsync(request);
                
                if (!response.Success)
                {
                    Console.WriteLine($"Error: {response.Message}");
                    return null;
                }

                return new Product
                {
                    Id = response.Id,
                    Name = response.Name,
                    Description = response.Description,
                    Price = (decimal)response.Price
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el producto: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var request = new ProductIdRequest { Id = id };
            
            try
            {
                var response = await _client.DeleteProductAsync(request);
                
                if (!response.Success)
                {
                    Console.WriteLine($"Error: {response.Message}");
                }

                return response.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el producto: {ex.Message}");
                return false;
            }
        }
    }
}