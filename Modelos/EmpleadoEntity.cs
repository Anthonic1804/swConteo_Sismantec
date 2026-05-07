using System.ComponentModel.DataAnnotations;

namespace swConteo_Sismantec.Modelos
{
    public class EmpleadoEntity
    {
        [Key]
        public int Id { get; set; }
        public string Empleado { get; set; }
        public string? Usuario_App_Inventario { get; set; }
        public string? Clave_App_Inventario { get; set; }
        public string? Identidad_App_Inventario { get; set; }
        public string? Estado_App_Inventario { get; set; }
        public DateTime? Ultima_Conexion_App_Inventario { get; set; }
        public int Generar_Token { get; set; }
    }
}
