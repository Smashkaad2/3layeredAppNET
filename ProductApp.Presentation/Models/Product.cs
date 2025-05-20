namespace ProductApp.Presentation.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public override string ToString()
        {
            return $"ID: {Id}, Nombre: {Name}, Descripción: {Description}, Precio: {Price:C}";
        }
    }
}