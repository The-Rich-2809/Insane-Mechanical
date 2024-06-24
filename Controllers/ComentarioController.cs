using Insane_Mechanical.Models;
using Insane_Mechanical.Services;
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
        private readonly WhatsAppServices _whatsappService;

        public ComentarioController(Insane_MechanicalDB context, WhatsAppServices whatsAppServices)
        {
            _context = context;
            _whatsappService = whatsAppServices;
        }

        [HttpGet]
        public async Task<ActionResult> GetComentarios(int idArticulo)
        {
            var comentarios = await _context.Comentario
                                            .Where(c => c.IdArticulo == idArticulo)
                                            .OrderByDescending(c => c.Fecha)
                                            .ToListAsync();

            var usuarios = await _context.Usuario.ToListAsync();

            return Ok(new { Comentarios = comentarios, Usuarios = usuarios });
        }

        [HttpPost]
        public async Task<ActionResult<Comentario>> PostComentario([FromBody] Comentario newComentario)
        {
            var usuarios = _context.Usuario.FirstOrDefaultAsync(i => i.ID == newComentario.IdUsuario);
            var articulo = _context.Articulo.FirstOrDefaultAsync(a => a.ID == newComentario.IdArticulo);

            if (newComentario.ComentarioPadreId > 0)
            {
                if (usuarios.Result.TipoUsuario == "Admin")
                    _whatsappService.SendAdminReplyMessage(usuarios.Result.Telefono, articulo.Result.Titulo);      
            }

            newComentario.Fecha = DateTime.UtcNow;
            _context.Comentario.Add(newComentario);
            await _context.SaveChangesAsync();
            return Ok(newComentario);
        }

    }
}
