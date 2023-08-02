using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using swConteo_Sismantec.Modelos;

namespace swConteo_Sismantec.Controllers
{
    [Route("Bodega")]
    [ApiController]
    public class BodegaController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public BodegaController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult getBodegas()
        {
            try
            {
                List<Bodega> bodegasList = new List<Bodega>();
                using (SqlConnection conexion = new SqlConnection(context.Database.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("APP_CONTEO_TBL_BODEGAS", conexion))
                    {
                        conexion.Open();
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Bodega item = new Bodega();
                                item.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                item.Nombre = reader.GetString(reader.GetOrdinal("Nombre"));

                                bodegasList.Add(item);
                            }
                        }
                        conexion.Close();
                    }
                }
                return Ok(bodegasList);
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
