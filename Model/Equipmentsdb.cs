namespace Gymwebproject.Model
{
    public class Equipmentsdb
    {

        public int ID { get; set; }

        public required string Equipments { get; set; }

        public  int Kg { get; set; }

        public Decimal Price { get; set; }

        public required string  Description { get; set; }


        public required string Warranty { get; set; }

        public required string Brand { get; set; }
        public required string Imageurl { get; set; }



    }
}
