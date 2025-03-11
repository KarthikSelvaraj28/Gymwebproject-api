using System.ComponentModel.DataAnnotations;

namespace Gymwebproject.Model
{
    public class Ordersdb
    {
        public int ID { get; set; }

        public Guid OrderID { get; set; } = Guid.NewGuid(); // ✅ Default value

        public required string Equipments { get; set; }

        public required string Description { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public required string UserEmail { get; set; }

        public string? Imageurl { get; set; } // ✅ Added Image property
    }
}
