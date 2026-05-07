using System.ComponentModel.DataAnnotations;

namespace swConteo_Sismantec.Modelos
{
    public class AppConteoInventarioEntity
    {
        [Key]
        public int Id { get; set; }
        public int? Id_empleado { get; set; }
        public DateTime Fecha_inicio { get; set; }
        public DateTime Fecha_fin { get; set; }
        public DateTime Fecha_envio { get; set; }
        public string Ubicacion { get; set; }
        public string? Dispositivo_identidad { get; set; }
        public int Id_ajuste_inventario { get; set; }
        public string? Nombre_empleado { get; set; }
        public int Id_bodega { get; set; }
        public string? IdConteoApp { get; set; }
    }
}
