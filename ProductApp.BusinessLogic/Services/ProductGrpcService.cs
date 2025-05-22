using Grpc.Core;
using ProductApp.BusinessLogic.Models;
using ProductApp.BusinessLogic.Protos;

namespace ProductApp.BusinessLogic.Services
{
    public class ProductGrpcServiceImpl : ProductGrpcService.ProductGrpcServiceBase
    {
        private readonly GrpcProductService _grpcService;
        private readonly ApiProductService _apiService;
        private readonly ILogger<ProductGrpcServiceImpl> _logger;

        public ProductGrpcServiceImpl(GrpcProductService grpcService, ApiProductService apiService,
            ILogger<ProductGrpcServiceImpl> logger)
        {
            _grpcService = grpcService;
            _apiService = apiService;
            _logger = logger;
        }

        public override async Task<ProductListResponse> GetAllProducts(EmptyRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Obteniendo todos los productos");

            try
            {
                // Obtener productos de la API
                var productsFromApi = await _apiService.GetAllProducts();
                
                // Actualizar memoria local con datos de la API
                foreach (var product in productsFromApi)
                {
                    _grpcService.CreateProduct(product);
                }

                var response = new ProductListResponse();
                foreach (var product in productsFromApi)
                {
                    response.Products.Add(new ProductItem
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Price = (double)product.Price
                    });
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos");
                return new ProductListResponse();
            }
        }

        public override async Task<ProductResponse> GetProduct(ProductIdRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Buscando producto con ID: {request.Id}");

            try
            {
                // Intentar obtener de la API primero
                var productFromApi = await _apiService.GetProduct(request.Id);
                if (productFromApi == null)
                {
                    return new ProductResponse
                    {
                        Success = false,
                        Message = $"No se encontró el producto con ID {request.Id}"
                    };
                }

                // Actualizar o crear en memoria local
                _grpcService.CreateProduct(productFromApi);

                return new ProductResponse
                {
                    Success = true,
                    Id = productFromApi.Id,
                    Name = productFromApi.Name,
                    Description = productFromApi.Description,
                    Price = (double)productFromApi.Price
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener producto {request.Id}");
                return new ProductResponse
                {
                    Success = false,
                    Message = $"Error al obtener producto: {ex.Message}"
                };
            }
        }

        public override async Task<ProductResponse> CreateProduct(ProductRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Creando nuevo producto");

            try
            {
                var product = new Product
                {
                    Name = request.Name,
                    Description = request.Description,
                    Price = (decimal)request.Price
                };

                // Crear primero en la API
                var createdProductInDb = await _apiService.CreateProduct(product);
                
                // Si se creó exitosamente en la API, actualizar memoria local
                if (createdProductInDb != null)
                {
                    _grpcService.CreateProduct(createdProductInDb);
                }

                return new ProductResponse
                {
                    Success = true,
                    Message = "Producto creado exitosamente",
                    Id = createdProductInDb.Id,
                    Name = createdProductInDb.Name,
                    Description = createdProductInDb.Description,
                    Price = (double)createdProductInDb.Price
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                return new ProductResponse
                {
                    Success = false,
                    Message = $"Error al crear producto: {ex.Message}"
                };
            }
        }

        public override async Task<ProductResponse> UpdateProduct(ProductRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Actualizando producto con ID: {request.Id}");

            try
            {
                var product = new Product
                {
                    Id = request.Id,
                    Name = request.Name,
                    Description = request.Description,
                    Price = (decimal)request.Price
                };

                // Actualizar primero en la API
                var updatedProductInDb = await _apiService.UpdateProduct(product);
                if (updatedProductInDb == null)
                {
                    return new ProductResponse
                    {
                        Success = false,
                        Message = $"No se encontró el producto con ID {request.Id} en la base de datos"
                    };
                }

                // Si se actualizó exitosamente en la API, actualizar memoria local
                _grpcService.UpdateProduct(updatedProductInDb);

                return new ProductResponse
                {
                    Success = true,
                    Message = "Producto actualizado exitosamente",
                    Id = updatedProductInDb.Id,
                    Name = updatedProductInDb.Name,
                    Description = updatedProductInDb.Description,
                    Price = (double)updatedProductInDb.Price
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar producto {request.Id}");
                return new ProductResponse
                {
                    Success = false,
                    Message = $"Error al actualizar producto: {ex.Message}"
                };
            }
        }

        public override async Task<DeleteResponse> DeleteProduct(ProductIdRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Eliminando producto con ID: {request.Id}");

            try
            {
                // Eliminar primero de la API
                var resultDb = await _apiService.DeleteProduct(request.Id);
                if (resultDb)
                {
                    // Si se eliminó exitosamente de la API, eliminar de memoria local
                    _grpcService.DeleteProduct(request.Id);
                }

                return new DeleteResponse
                {
                    Success = resultDb,
                    Message = resultDb ? "Producto eliminado exitosamente" : $"No se encontró el producto con ID {request.Id}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar producto {request.Id}");
                return new DeleteResponse
                {
                    Success = false,
                    Message = $"Error al eliminar producto: {ex.Message}"
                };
            }
        }
    }
}