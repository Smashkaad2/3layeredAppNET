using ProductApp.BusinessLogic.Models;

namespace ProductApp.BusinessLogic.Services
{
    public class GrpcProductService
    {
        private readonly List<Product> _products;
        private int _nextId = 1;

        public GrpcProductService()
        {
            _products = new List<Product>
            {
                new Product { Id = _nextId++, Name = "Producto 1", Description = "Descripción 1", Price = 10.99m },
                new Product { Id = _nextId++, Name = "Producto 2", Description = "Descripción 2", Price = 20.50m }
            };
        }

        public List<Product> GetAllProducts() => _products;

        public Product? GetProduct(int id) => _products.FirstOrDefault(p => p.Id == id);

        public Product CreateProduct(Product product)
        {
            product.Id = _nextId++;
            _products.Add(product);
            return product;
        }

        public Product? UpdateProduct(Product product)
        {
            var existingProduct = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct == null) return null;

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            return existingProduct;
        }

        public bool DeleteProduct(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return product != null && _products.Remove(product);
        }
    }
}