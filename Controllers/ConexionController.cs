using Microsoft.AspNetCore.Mvc;
using swConteo_Sismantec.Modelos;

namespace swConteo_Sismantec.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ConexionController : ControllerBase
    {

        [HttpGet("conexion")]
        public IActionResult getConexion() {
            return StatusCode(StatusCodes.Status200OK, new RespuestaConexion { Response = "CONEXION_EXITOSA" });
        }

    }
}
