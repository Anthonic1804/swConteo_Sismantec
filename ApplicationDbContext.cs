using Microsoft.EntityFrameworkCore;
using swConteo_Sismantec.Modelos;

namespace swConteo_Sismantec
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Inventario> Inventario { get; set; }
        public DbSet<Empleado> Empleados { get; set; }

    }
}
