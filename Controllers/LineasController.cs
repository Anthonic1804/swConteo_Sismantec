using Microsoft.AspNetCore.Mvc;
using swConteo_Sismantec.Modelos;
using Microsoft.EntityFrameworkCore;

namespace swConteo_Sismantec.Controllers
{
    [Route("Lineas")]
    [ApiController]
    public class LineasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public LineasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<LineasEntity>>> ObtenerLineas()
        { 
            var lista = await _context.Lineas
                .AsNoTracking()
                .Select(l => new LineasEntity
                {
                    Id = l.Id,
                    Nombre = l.Nombre.Trim()
                })
                .ToListAsync();

            return Ok(lista);
        }

    }
}
