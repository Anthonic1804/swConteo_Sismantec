using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using swConteo_Sismantec.Modelos;

namespace swConteo_Sismantec.Controllers
{
    [ApiController]
    [Route("inventario")]
    public class InventarioController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public InventarioController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult GetInventario()
        {
            try
            {
                List<Inventario> inventarioList = new List<Inventario>();

                using (SqlConnection conexion = new SqlConnection(context.Database.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("APP_CONTEO_TBL_INVENTARIO", conexion))
                    {
                        conexion.Open();
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Inventario item = new Inventario();
                                item.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                item.Codigo = reader.GetString(reader.GetOrdinal("Codigo"));
                                item.Descripcion = reader.GetString(reader.GetOrdinal("Descripcion"));

                                inventarioList.Add(item);
                            }
                        } 
                    } 
                } 

                return Ok(inventarioList);
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
