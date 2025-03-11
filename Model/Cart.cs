using Microsoft.AspNetCore.Http.HttpResults;

namespace Gymwebproject.Model
{
    public class Cart

    {
        public int ID { get; set;}

        public int OrderID { get; set; }

        public int ProductID { get; set; }

        public required string Equipments { get; set; }

        public required string Description { get; set; }

       // public decimal Price { get; set; }

        public int Quantity { get; set; }

        public Decimal TotalPrice { get; set; }

        public required string UserEmail { get; set; }

        public DateOnly OrderDate { get; set; }
    }
}

