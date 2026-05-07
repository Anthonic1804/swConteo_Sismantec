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
        public async Task<ActionResult<IReadOnlyList<Inventario>>> ObtenerInventario()
        {
            var lista = await context.Inventario
                .AsNoTracking()
                .Select(s => new Inventario
                {
                    Id = s.Id,
                    Codigo = s.Codigo != null ? s.Codigo.Trim() : "",
                    Descripcion = s.Descripcion != null ? s.Descripcion.Trim() : "",
                    Existencia = s.Existencia,
                    Existencia_u = s.Existencia_u,
                    Fraccion = s.Fraccion,
                    Id_Linea = s.Id_Linea
                })
                .ToListAsync();

            return Ok(lista);
        }

        /*public IActionResult GetInventario()
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
        }*/

        //ENDPOINT PARA REGISTRAR EL CONTEO Y DETALLE EN LA BD
        [HttpPost("registrar")]
        public IActionResult RegistrarConteo([FromBody] ConteoInventario parametros)
        {
            if (parametros != null)
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var ajusteInventario = context.Ajuste_inventario
                            .AsNoTracking()
                            .FirstOrDefault(a => a.Id == parametros.IdAjusteInventario
                                && a.Estado.Trim() == "ABIERTO");

                        if (ajusteInventario==null)
                        {
                            return StatusCode(StatusCodes.Status404NotFound, new RespuestaConexion { Response = "AJUSTE_INVENTARIO_INCORRECTO" });
                        }

                        var conteoExistente = context.App_Conteo_Inventario
                            .AsNoTracking()
                            .FirstOrDefault(c => c.IdConteoApp == parametros.IdConteoApp
                                && c.Fecha_envio.Date == parametros.FechaEnvio.Date);

                        if (conteoExistente != null)
                        {
                            return StatusCode(StatusCodes.Status404NotFound, new RespuestaConexion { Response = "CONTEO_YA_REGISTRADO" });
                        }
                        else
                        {

                            var nuevoConteo = new AppConteoInventarioEntity
                            {
                                Id_empleado = parametros.IdEmpleado,
                                Fecha_inicio = parametros.FechaInicio,
                                Fecha_fin = parametros.FechaFin,
                                Fecha_envio = parametros.FechaEnvio,
                                Ubicacion = parametros.Ubicacion,
                                Id_ajuste_inventario = parametros.IdAjusteInventario,
                                Nombre_empleado = parametros.Empleado,
                                Id_bodega = parametros.IdBodega,
                                IdConteoApp = parametros.IdConteoApp
                            };

                            context.App_Conteo_Inventario.Add(nuevoConteo);
                            context.SaveChanges();

                            foreach (var item in parametros.Detalle)
                            {
                                var detalleConteo = new AppDetalleConteoInventarioEntity
                                {
                                    Id_app_conteo_inventario = nuevoConteo.Id,
                                    Id_inventario = item.Id_Inventario,
                                    Existencia = item.Existencias,
                                    Existencia_u = item.Existencias_u,
                                    IdLote = item.IdLote,
                                    Lote = item.Lote,
                                    Fecha_vencimiento = item.FechaVencimiento,
                                    IdTalla = item.IdTalla,
                                    Talla = item.Talla
                                };
                                context.App_Detalle_Conteo_Inventario.Add(detalleConteo);
                            }

                            context.SaveChanges();
                            transaction.Commit();

                            return Ok(new RespuestaConexion { Response = "CONTEO_REGISTRADO" });

                        }
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new RespuestaConexion { Response = "ERROR_REGISTRAR_CONTEO: " + ex.Message });
                    }
                }
            }
            else {
                return BadRequest(new RespuestaConexion { Response = "PARAMETROS_VACIOS" });
            }

        }

        /*public IActionResult RegistrarConteo([FromBody] ConteoInventario parametros)
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
        }*/

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
