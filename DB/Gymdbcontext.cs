
using Microsoft.EntityFrameworkCore;

namespace Gymwebproject.DB

{
    public class Gymdbcontext: DbContext
    {
        public Gymdbcontext(DbContextOptions<Gymdbcontext> options)
           : base(options)
        {
        }

        public DbSet<Gymwebproject.Model.Userinformation> Userinformation { get; set; } = default!;

        public DbSet<Gymwebproject.Model.Equipmentsdb> Equipmentsdb { get; set; } = default!;

        public DbSet<Gymwebproject.Model.Cart> Cart { get; set; } = default!;


        public DbSet<Gymwebproject.Model.Ordersdb> Orders { get; set; } = default!;



    }
}