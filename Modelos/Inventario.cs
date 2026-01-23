namespace swConteo_Sismantec.Modelos
{
    public class Inventario
    {
        public int Id { get; set; }
        public string? Codigo { get; set; }
        public string? Descripcion { get; set; }
        public decimal Existencia { get; set; }
        public decimal Existencia_u { get; set; }
        public decimal Fraccion { get; set; }
    }
}