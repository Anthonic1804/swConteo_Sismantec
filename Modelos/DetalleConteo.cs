namespace swConteo_Sismantec.Modelos
{
    public class DetalleConteo
    {
        public int Id_Inventario { get; set; }
        public int Existencias { get; set; }
        public int Existencias_u { get; set; }
        public int? IdLote { get; set; }
        public string? Lote { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public int? IdTalla { get; set; }
        public string? Talla { get; set; }
    }
}
