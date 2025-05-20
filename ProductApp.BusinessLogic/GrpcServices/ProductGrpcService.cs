using Grpc.Core;
using ProductApp.BusinessLogic.Models;
using ProductApp.BusinessLogic.Protos;
using ProductApp.BusinessLogic.Services;

namespace ProductApp.BusinessLogic.GrpcServices
{
    public class ProductGrpcService : Protos.ProductGrpcService.ProductGrpcServiceBase
    {
        private readonly ProductApiService _productApiService;
        private readonly ILogger<ProductGrpcService> _logger;

        public ProductGrpcService(ProductApiService productApiService, ILogger<ProductGrpcService> logger)
        {
            _productApiService = productApiService;
            _logger = logger;
        }

        public override async Task<ProductResponse> CreateProduct(ProductRequest request, ServerCallContext context)
        {
            try
            {
                var product = new Product
                {
                    Name = request.Name,
                    Description = request.Description,
                    Price = (decimal)request.Price
                };

                var createdProduct = await _productApiService.CreateProductAsync(product);

                return new ProductResponse
                {
                    Id = createdProduct.Id,
                    Name = createdProduct.Name,
                    Description = createdProduct.Description,
                    Price = (double)createdProduct.Price,
                    Success = true,
                    Message = "Producto creado exitosamente"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el producto");
                return new ProductResponse
                {
                    Success = false,
                    Message = $"Error al crear el producto: {ex.Message}"
                };
            }
        }

        public override async Task<ProductsListResponse> GetAllProducts(EmptyRequest request, ServerCallContext context)
        {
            try
            {
                var products = await _productApiService.GetAllProductsAsync();
                var response = new ProductsListResponse();

                foreach (var product in products)
                {
                    response.Products.Add(new ProductResponse
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Price = (double)product.Price,
                        Success = true
                    });
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los productos");
                return new ProductsListResponse();
            }
        }

        public override async Task<ProductResponse> GetProduct(ProductIdRequest request, ServerCallContext context)
        {
            try
            {
                var product = await _productApiService.GetProductAsync(request.Id);

                return new ProductResponse
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = (double)product.Price,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el producto con ID {request.Id}");
                return new ProductResponse
                {
                    Success = false,
                    Message = $"Error al obtener el producto: {ex.Message}"
                };
            }
        }

        public override async Task<ProductResponse> UpdateProduct(ProductRequest request, ServerCallContext context)
        {
            try
            {
                var product = new Product
                {
                    Id = request.Id,
                    Name = request.Name,
                    Description = request.Description,
                    Price = (decimal)request.Price
                };

                var updatedProduct = await _productApiService.UpdateProductAsync(request.Id, product);

                return new ProductResponse
                {
                    Id = updatedProduct.Id,
                    Name = updatedProduct.Name,
                    Description = updatedProduct.Description,
                    Price = (double)updatedProduct.Price,
                    Success = true,
                    Message = "Producto actualizado exitosamente"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el producto con ID {request.Id}");
                return new ProductResponse
                {
                    Success = false,
                    Message = $"Error al actualizar el producto: {ex.Message}"
                };
            }
        }

        public override async Task<DeleteResponse> DeleteProduct(ProductIdRequest request, ServerCallContext context)
        {
            try
            {
                var success = await _productApiService.DeleteProductAsync(request.Id);

                return new DeleteResponse
                {
                    Success = success,
                    Message = success ? "Producto eliminado exitosamente" : "No se pudo eliminar el producto"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el producto con ID {request.Id}");
                return new DeleteResponse
                {
                    Success = false,
                    Message = $"Error al eliminar el producto: {ex.Message}"
                };
            }
        }
    }
}