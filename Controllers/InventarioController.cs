using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using swConteo_Sismantec.Modelos;
using System.Data;

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

        //ENDPOINT PARA OBTERNER EL INVENTARIO
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
                                item.Existencia = reader.GetDecimal(reader.GetOrdinal("Unidades"));
                                item.Existencia_u = reader.GetDecimal(reader.GetOrdinal("Fracciones"));
                                item.Fraccion = reader.GetDecimal(reader.GetOrdinal("Fraccion"));

                                inventarioList.Add(item);
                            }
                        }
                        conexion.Close();
                    } 
                } 

                return Ok(inventarioList);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //ENDPOINT PARA REGISTRAR EL CONTEO Y DETALLE EN LA BD
        [HttpPost("registrar")]
        public IActionResult RegistrarConteo([FromBody] ConteoInventario parametros)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(context.Database.GetConnectionString()))
                {
                    conexion.Open();

                    using (SqlCommand cmd = new SqlCommand("APP_CONTEO_TBL_AJUSTE_INVENTARIO", conexion))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IDAJUSTE", parametros.IdAjusteInventario);
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            reader.Close();

                            using (SqlCommand registrar = new SqlCommand("APP_CONTEO_TBL_INVENTARIO_INSERT", conexion))
                            {
                                registrar.CommandType = System.Data.CommandType.StoredProcedure;
                                registrar.Parameters.AddWithValue("@FECHA_INICIO", parametros.FechaInicio);
                                registrar.Parameters.AddWithValue("@FECHA_FIN", parametros.FechaFin);
                                registrar.Parameters.AddWithValue("@FECHA_ENVIO", parametros.FechaEnvio);
                                registrar.Parameters.AddWithValue("@UBICACION", parametros.Ubicacion);
                                registrar.Parameters.AddWithValue("@ID_BODEGA", parametros.IdBodega);
                                registrar.Parameters.AddWithValue("@ID_AJUSTE", parametros.IdAjusteInventario);
                                registrar.Parameters.AddWithValue("@ID_EMPLEADO", parametros.IdEmpleado);
                                registrar.Parameters.AddWithValue("@EMPLEADO", parametros.Empleado);

                                var detalleConteo = new DataTable();
                                detalleConteo.Columns.Add("ID_INVENTARIO", typeof(int));
                                detalleConteo.Columns.Add("EXISTENCIA", typeof(int));
                                detalleConteo.Columns.Add("EXISTENCIA_U", typeof(int));
                                detalleConteo.Columns.Add("IDLOTE", typeof(int));
                                detalleConteo.Columns.Add("LOTE", typeof(string));
                                detalleConteo.Columns.Add("FECHAVENCIMIENTO", typeof(DateTime));
                                detalleConteo.Columns.Add("IDTALLA", typeof(int));
                                detalleConteo.Columns.Add("TALLA", typeof(string));

                                foreach (var item in parametros.Detalle)
                                {
                                    detalleConteo.Rows.Add(item.Id_Inventario, 
                                        item.Existencias, 
                                        item.Existencias_u, 
                                        item.IdLote, 
                                        item.Lote, 
                                        item.FechaVencimiento, 
                                        item.IdTalla, 
                                        item.Talla);
                                }

                                var detalle = registrar.Parameters.AddWithValue("@DETALLE", detalleConteo);
                                detalle.SqlDbType = SqlDbType.Structured;

                                int insertCorrecto = registrar.ExecuteNonQuery();

                                if (insertCorrecto > 0)
                                {
                                    return Ok(new RespuestaConexion { Response = "CONTEO_REGISTRADO" });
                                }
                                else
                                {
                                    return BadRequest(new RespuestaConexion { Response = "ERROR_REGISTRAR_CONTEO" });
                                }
                            }
                        }
                        else
                        {
                            return BadRequest(new RespuestaConexion { Response = "AJUSTE_INVENTARIO_INCORRECTO" });
                        }
                    }
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        //--------------------------------------------------------
        //  Endpoint para obtener los lotes 
        //--------------------------------------------------------
        [HttpGet("cantidadlotes")]
        public async Task<ActionResult<int>> ObtenerCantidadLotes()
        {
            var cantidad = await context.Inventario_lotes
                .AsNoTracking()
                .CountAsync();

            return Ok(cantidad);
        }

        [HttpGet("lotes")]
        public async Task<ActionResult<IReadOnlyList<InventarioLotesEntity>>> ObtenerInventarioLotes()
        {
            var lotes = await context.Inventario_lotes
                .AsNoTracking()
                .Select(s => new InventarioLotesDTO
                {
                    IdLote = s.IdLote,
                    IdProducto = s.Id_producto,
                    CodigoProducto = s.Codigo_producto != null ? s.Codigo_producto.Trim() : "",
                    Lote = s.Lote != null ? s.Lote.Trim() : "",
                    FechaVencimiento = s.Fecha_vencimiento.ToString("yyyy-MM-dd")
                })
                .ToListAsync();

            return Ok(lotes);
        }
    }
}
