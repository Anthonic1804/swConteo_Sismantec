using System.ComponentModel.DataAnnotations;

namespace swConteo_Sismantec.Modelos
{
    public class AjusteInventarioEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime Fecha_fin { get; set; }
        public decimal Diferencia { get; set; }
        public string Estado { get; set; }
        public string? DTEEstablecimiento { get; set; }
        public string? DTEPuntodeVenta{ get; set; }
        public int? Id_user { get; set; }
        public int? IdEmpresa { get; set; }
        public string? Ip { get; set; }
        public string? Pc { get; set; }
        public string? pc_red { get; set; }
        public string? Usuario { get; set; }
        public string? UUID_Ajuste_manual { get; set; }
    }
}
