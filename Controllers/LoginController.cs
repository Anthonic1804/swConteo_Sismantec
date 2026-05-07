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
        /*public IActionResult Login([FromBody] Login parametros ) {
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
        }*/

        public IActionResult IniciarSesion([FromBody] Login parametros)
        {
            if (parametros == null || string.IsNullOrEmpty(parametros.Usuario) || string.IsNullOrEmpty(parametros.Clave)) 
            { 
                return BadRequest(new Empleado { Id = 0, Nombre = "PARAMETROS_EQUIVOCADOS", Generar_Token = 0});
            }

            try
            {
                var validacion = context.Empleados
                    .FirstOrDefault(e => e.Usuario_App_Inventario == parametros.Usuario 
                        && e.Clave_App_Inventario == parametros.Clave);

                if (validacion == null)
                {
                    return BadRequest(new Empleado { Id = 0, Nombre = "NO_SE_ENCONTRO", Generar_Token = 0 });
                }

                var fecha = DateTime.Now;

                validacion.Estado_App_Inventario = "ACTIVO";
                validacion.Identidad_App_Inventario = parametros.IdentidadApp;
                validacion.Ultima_Conexion_App_Inventario = fecha;

                context.SaveChanges();
                return Ok(new Empleado { Id = validacion.Id, Nombre = validacion.Empleado.Trim(), Generar_Token = validacion.Generar_Token });

            }
            catch (Exception ex) { 
                return StatusCode(StatusCodes.Status500InternalServerError, new Empleado { Id = 0, Nombre = "ERROR_INTERNO", Generar_Token = 0 });
            }

        }

        [HttpPut("logout")]
        public IActionResult CerrarSesion([FromBody] Logout parametros)
        {
            if (parametros == null || parametros.Id <= 0)
            {
                return BadRequest(new RespuestaConexion { Response = "PARAMETROS_EQUIVOCADOS" });
            }
            try
            {
                var empleado = context.Empleados.FirstOrDefault(e => e.Id == parametros.Id);
                if (empleado == null)
                {
                    return NotFound(new RespuestaConexion { Response = "EMPLEADO_NO_ENCONTRADO" });
                }

                empleado.Estado_App_Inventario = "INACTIVO";
                empleado.Identidad_App_Inventario = null;
                context.SaveChanges();

                return Ok(new RespuestaConexion { Response = "PROCESO_EXITOSO" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new RespuestaConexion { Response = "ERROR_INTERNO" });
            }
        }
        /*public IActionResult Logout([FromBody] Logout parametros ) {
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
        }*/

    }
}
