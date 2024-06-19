using Insane_Mechanical.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Insane_Mechanical.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComentarioController : ControllerBase
    {

        private readonly Insane_MechanicalDB _context;

        public ComentarioController(Insane_MechanicalDB context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetComentarios(int idArticulo)
        {
            var comentarios = await _context.Comentario
                                            .Where(c => c.IdArticulo == idArticulo)
                                            .OrderByDescending(c => c.Fecha)
                                            .ToListAsync();

            var usuarios = await _context.Usuario.ToListAsync(); // Suponiendo que tienes una tabla de usuarios

            return Ok(new { Comentarios = comentarios, Usuarios = usuarios });
        }

        [HttpPost]
        public async Task<ActionResult<Comentario>> PostComentario([FromBody] Comentario newComentario)
        {
            newComentario.Fecha = DateTime.UtcNow;
            _context.Comentario.Add(newComentario);
            await _context.SaveChangesAsync();
            return Ok(newComentario);
        }

    }
}
