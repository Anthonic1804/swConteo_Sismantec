using Microsoft.EntityFrameworkCore;
using swConteo_Sismantec.Modelos;

namespace swConteo_Sismantec
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            Console.WriteLine("CADENA: " + Database.GetDbConnection().ConnectionString);
        }

        public DbSet<Inventario> Inventario { get; set; }
        public DbSet<EmpleadoEntity> Empleados { get; set; }
        public DbSet<Bodega> Bodegas { get; set; }
        public DbSet<InventarioLotesEntity> Inventario_lotes { get; set; }
        public DbSet<LineasEntity> Lineas { get; set; }
        public DbSet<AppConteoInventarioEntity> App_Conteo_Inventario { get; set; }
        public DbSet<AppDetalleConteoInventarioEntity> App_Detalle_Conteo_Inventario { get; set; }
        public DbSet<AjusteInventarioEntity> Ajuste_inventario { get; set; }

    }
}
