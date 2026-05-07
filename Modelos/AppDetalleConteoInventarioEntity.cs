using System.ComponentModel.DataAnnotations;

namespace swConteo_Sismantec.Modelos
{
    public class AppDetalleConteoInventarioEntity
    {
        [Key]
        public int Id { get; set; }
        public int Id_app_conteo_inventario { get; set; }
        public int Id_inventario { get; set; }
        public decimal Existencia { get; set; }
        public decimal Existencia_u { get; set; }
        public int? IdLote { get; set; }
        public string? Lote { get; set; }
        public DateTime? Fecha_vencimiento { get; set; }
        public int? IdTalla { get; set; }
        public string? Talla { get; set; }
    }
}
