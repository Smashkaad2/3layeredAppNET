using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductApp.Presentation.Models;
using ProductApp.Presentation.Services;

class Program
{
    static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        var productService = host.Services.GetRequiredService<ProductGrpcClientService>();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Menú de Productos ===");
            Console.WriteLine("1. Listar todos los productos");
            Console.WriteLine("2. Buscar producto por ID");
            Console.WriteLine("3. Crear nuevo producto");
            Console.WriteLine("4. Actualizar producto");
            Console.WriteLine("5. Eliminar producto");
            Console.WriteLine("6. Salir");
            Console.Write("\nSeleccione una opción: ");

            if (!int.TryParse(Console.ReadLine(), out int option))
            {
                Console.WriteLine("Opción no válida. Presione cualquier tecla para continuar...");
                Console.ReadKey();
                continue;
            }

            try
            {
                switch (option)
                {
                    case 1:
                        await ListarProductos(productService);
                        break;
                    case 2:
                        await BuscarProducto(productService);
                        break;
                    case 3:
                        await CrearProducto(productService);
                        break;
                    case 4:
                        await ActualizarProducto(productService);
                        break;
                    case 5:
                        await EliminarProducto(productService);
                        break;
                    case 6:
                        return;
                    default:
                        Console.WriteLine("Opción no válida");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddTransient<ProductGrpcClientService>();
            });

    private static async Task ListarProductos(ProductGrpcClientService service)
    {
        var productos = await service.GetAllProductsAsync();
        Console.WriteLine("\n=== Lista de Productos ===");
        foreach (var producto in productos)
        {
            Console.WriteLine(producto);
        }
    }

    private static async Task BuscarProducto(ProductGrpcClientService service)
    {
        Console.Write("\nIngrese el ID del producto: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            var producto = await service.GetProductAsync(id);
            if (producto != null)
                Console.WriteLine(producto);
            else
                Console.WriteLine("Producto no encontrado");
        }
    }

    private static async Task CrearProducto(ProductGrpcClientService service)
    {
        Console.WriteLine("\n=== Crear Nuevo Producto ===");
        var producto = new Product();
        
        Console.Write("Nombre: ");
        producto.Name = Console.ReadLine() ?? string.Empty;
        
        Console.Write("Descripción: ");
        producto.Description = Console.ReadLine() ?? string.Empty;
        
        Console.Write("Precio: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal precio))
        {
            producto.Price = precio;
            var resultado = await service.CreateProductAsync(producto);
            if (resultado != null)
                Console.WriteLine("Producto creado exitosamente");
            else
                Console.WriteLine("Error al crear el producto");
        }
    }

    private static async Task ActualizarProducto(ProductGrpcClientService service)
    {
        Console.Write("\nIngrese el ID del producto a actualizar: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            var productoExistente = await service.GetProductAsync(id);
            if (productoExistente == null)
            {
                Console.WriteLine("Producto no encontrado");
                return;
            }

            Console.WriteLine("Deje en blanco para mantener el valor actual");
            
            Console.Write($"Nombre ({productoExistente.Name}): ");
            string? nombre = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nombre))
                productoExistente.Name = nombre;

            Console.Write($"Descripción ({productoExistente.Description}): ");
            string? descripcion = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(descripcion))
                productoExistente.Description = descripcion;

            Console.Write($"Precio ({productoExistente.Price:C}): ");
            string? precioStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(precioStr) && decimal.TryParse(precioStr, out decimal precio))
                productoExistente.Price = precio;

            var resultado = await service.UpdateProductAsync(productoExistente);
            if (resultado != null)
                Console.WriteLine("Producto actualizado exitosamente");
            else
                Console.WriteLine("Error al actualizar el producto");
        }
    }

    private static async Task EliminarProducto(ProductGrpcClientService service)
    {
        Console.Write("\nIngrese el ID del producto a eliminar: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            var resultado = await service.DeleteProductAsync(id);
            if (resultado)
                Console.WriteLine("Producto eliminado exitosamente");
            else
                Console.WriteLine("Error al eliminar el producto");
        }
    }
}