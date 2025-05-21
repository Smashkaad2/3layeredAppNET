using Grpc.Core;
using ProductApp.BusinessLogic.Models;
using ProductApp.BusinessLogic.Protos;

namespace ProductApp.BusinessLogic.Services
{
    public class ProductGrpcServiceImpl : ProductGrpcService.ProductGrpcServiceBase
    {
        private readonly ProductService _productService;
        private readonly ILogger<ProductGrpcServiceImpl> _logger;

        public ProductGrpcServiceImpl(ProductService productService, ILogger<ProductGrpcServiceImpl> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public override Task<ProductListResponse> GetAllProducts(EmptyRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Obteniendo todos los productos");

            var response = new ProductListResponse();
            var products = _productService.GetAllProducts();

            foreach (var product in products)
            {
                response.Products.Add(new ProductItem
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = (double)product.Price
                });
            }

            return Task.FromResult(response);
        }

        public override Task<ProductResponse> GetProduct(ProductIdRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Buscando producto con ID: {request.Id}");

            var product = _productService.GetProduct(request.Id);
            
            if (product == null)
            {
                return Task.FromResult(new ProductResponse
                {
                    Success = false,
                    Message = $"No se encontró el producto con ID {request.Id}"
                });
            }

            return Task.FromResult(new ProductResponse
            {
                Success = true,
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = (double)product.Price
            });
        }

        public override Task<ProductResponse> CreateProduct(ProductRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Creando nuevo producto");

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = (decimal)request.Price
            };

            var createdProduct = _productService.CreateProduct(product);

            return Task.FromResult(new ProductResponse
            {
                Success = true,
                Message = "Producto creado exitosamente",
                Id = createdProduct.Id,
                Name = createdProduct.Name,
                Description = createdProduct.Description,
                Price = (double)createdProduct.Price
            });
        }

        public override Task<ProductResponse> UpdateProduct(ProductRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Actualizando producto con ID: {request.Id}");

            var product = new Product
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
                Price = (decimal)request.Price
            };

            var updatedProduct = _productService.UpdateProduct(product);

            if (updatedProduct == null)
            {
                return Task.FromResult(new ProductResponse
                {
                    Success = false,
                    Message = $"No se encontró el producto con ID {request.Id}"
                });
            }

            return Task.FromResult(new ProductResponse
            {
                Success = true,
                Message = "Producto actualizado exitosamente",
                Id = updatedProduct.Id,
                Name = updatedProduct.Name,
                Description = updatedProduct.Description,
                Price = (double)updatedProduct.Price
            });
        }

        public override Task<DeleteResponse> DeleteProduct(ProductIdRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Eliminando producto con ID: {request.Id}");

            var result = _productService.DeleteProduct(request.Id);

            return Task.FromResult(new DeleteResponse
            {
                Success = result,
                Message = result ? "Producto eliminado exitosamente" : $"No se encontró el producto con ID {request.Id}"
            });
        }
    }
}