namespace swConteo_Sismantec.Modelos
{
    public class ConteoInventario
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaEnvio { get; set; }
        public string Ubicacion { get; set; }
        public int IdBodega { get; set; }
        public int IdAjusteInventario { get; set; }
        public int IdEmpleado { get; set; }
        public string Empleado { get; set; }
        public List<DetalleConteo> Detalle { get; set; }
    }
}
