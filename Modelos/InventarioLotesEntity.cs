using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace swConteo_Sismantec.Modelos
{
    public class InventarioLotesEntity
    {
        [Key]
        public int IdLote { get; set; }
        public int Id_producto { get; set; }
        public string? Codigo_producto { get; set; }
        public string Lote { get; set; }
        public DateTime Fecha_vencimiento { get; set; }
        public decimal Existencia { get; set; }
        public decimal Existencia_u { get; set; }
    }
}
