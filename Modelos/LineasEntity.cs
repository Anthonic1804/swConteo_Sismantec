using System.ComponentModel.DataAnnotations;

namespace swConteo_Sismantec.Modelos
{
    public class LineasEntity
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
