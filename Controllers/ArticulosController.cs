using Insane_Mechanical.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Insane_Mechanical.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticulosController : ControllerBase
    {
        private readonly Insane_MechanicalDB _context;
        private IWebHostEnvironment hostEnvironment;

        public ArticulosController(Insane_MechanicalDB context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this.hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public IActionResult GetHtmlContent(int idArticulo)
        {
            var articulo = _context.Articulo.FirstOrDefault(a => a.ID == idArticulo);
            if (articulo == null)
            {
                return NotFound();
            }

            var rutaHtml = articulo.RutaHTML;

            return Ok(new { rutaHtml });
        }
    }
}
