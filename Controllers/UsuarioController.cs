using Insane_Mechanical.Helpers;
using Insane_Mechanical.Models;
using Insane_Mechanical.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Insane_Mechanical.Controllers
{
    public class UsuarioController : Controller
    {
        public readonly Insane_MechanicalDB _contextDB;
        private HelperUploadFiles helperUpload;

        private static string FotoPerfil { get; set; } = string.Empty;

        public static int ID { get; set; }

        public UsuarioController(Insane_MechanicalDB contextDB, HelperUploadFiles helperUploadFiles)
        {
            _contextDB = contextDB;
            helperUpload = helperUploadFiles;
        }

        public void Cookies()
        {
            var miCookie = HttpContext.Request.Cookies["MiCookie"];

            if (miCookie != null)
            {
                List<Usuario> listaUsuarios = _contextDB.Usuario.ToList();
                foreach (var user in listaUsuarios)
                {
                    if (miCookie == user.Correo)
                    {
                        ID = user.ID;
                        ViewBag.Nombre = user.Nombre;
                        ViewBag.Nivel = user.TipoUsuario;
                        ViewBag.FotoPerfil = user.DireccionImagen;
                    }
                }
            }
            List<Categoria> categorias = _contextDB.Categoria.ToList();
            ViewBag.Categorias = categorias;
        }

        public IActionResult Index()
        {
            Cookies();
            return View();
        }

        public IActionResult Cotizador()
        {
            Cookies();
            return View();
        }

        [HttpGet]
        public IActionResult Editar()
        {
            Cookies();
            var usuario = _contextDB.Usuario.AsNoTracking().FirstOrDefault(c => c.ID == ID);

            FotoPerfil = usuario.DireccionImagen;

            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Usuario usuario, IFormFile Imag)
        {
            Cookies();

            var usuarioExistente = await _contextDB.Usuario.FindAsync(usuario.ID);
            if (usuarioExistente == null)
            {
                return NotFound();
            }

            if (Imag != null)
            {
                string nombreImagen = usuario.Correo + Imag.FileName;
                await this.helperUpload.UploadFilesAsync(Imag, nombreImagen, Folders.Images);
                usuarioExistente.DireccionImagen = "../Images/Usuarios/" + nombreImagen;
            }
            else
            {
                usuarioExistente.DireccionImagen = FotoPerfil;
            }

            usuarioExistente.Nombre = usuario.Nombre;
            usuarioExistente.Correo = usuario.Correo;
            // Actualiza otras propiedades según sea necesario

            _contextDB.Usuario.Update(usuarioExistente);
            await _contextDB.SaveChangesAsync();

            Cookies();

            return RedirectToAction("Index");
        }
    }
}
