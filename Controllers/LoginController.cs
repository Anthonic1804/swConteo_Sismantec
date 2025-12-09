using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using swConteo_Sismantec.Modelos;

namespace swConteo_Sismantec.Controllers
{
    [ApiController]
    [Route("Login")]
    public class LoginController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public LoginController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Login parametros ) {
            try
            {
                string nombre = "";
                int id = 0;
                int admin = 0;
                using (SqlConnection conexion = new SqlConnection(context.Database.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("APP_CONTEO_TBL_EMPLEADOS_LOGIN", conexion))
                    {
                        conexion.Open();
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@USUARIO", parametros.Usuario);
                        cmd.Parameters.AddWithValue("@CLAVE", parametros.Clave);
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            id = reader.GetInt32(reader.GetOrdinal("Id"));
                            nombre = reader.GetString(reader.GetOrdinal("Empleado"));
                            admin = reader.GetInt32(reader.GetOrdinal("Generar_Token"));

                            reader.Close();

                            using (SqlCommand update = new SqlCommand("APP_CONTEO_TBL_EMPLEADO_UPDATE", conexion))
                            {
                                update.CommandType = System.Data.CommandType.StoredProcedure;
                                update.Parameters.AddWithValue("@ID", id);
                                update.Parameters.AddWithValue("@ESTADO", "ACTIVO");
                                update.ExecuteNonQuery();
                            }

                            return Ok(new Empleado { Id = id, Nombre = nombre, Generar_Token = admin });
                        }
                        else
                        {
                            return BadRequest(new Empleado { Id = 0, Nombre = "PARAMETROS_EQUIVOCADOS" });
                        }
                    }
                }
            }
            catch {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("logout")]
        public IActionResult Logout([FromBody] Logout parametros ) {
            try
            {
                using (SqlConnection conexion = new SqlConnection(context.Database.GetConnectionString()))
                {
                    using (SqlCommand update = new SqlCommand("APP_CONTEO_TBL_EMPLEADO_UPDATE", conexion))
                    {
                        conexion.Open();
                        update.CommandType = System.Data.CommandType.StoredProcedure;
                        update.Parameters.AddWithValue("@ID", parametros.Id);
                        update.Parameters.AddWithValue("@ESTADO", "INACTIVO");
                        update.ExecuteNonQuery();
                        conexion.Close();
                    }
                    return Ok(new RespuestaConexion { Response = "PROCESO_EXITOSO" });
                }
            }
            catch {
                return BadRequest(new RespuestaConexion { Response = "PARAMETROS_EQUIVOCADOS" });
            }
        }

    }
}
