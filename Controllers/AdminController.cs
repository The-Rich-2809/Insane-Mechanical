using Insane_Mechanical.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using Ultragamma.Helpers;
using Ultragamma.Providers;

namespace Insane_Mechanical.Controllers
{
    public class AdminController : Controller
    {
        public readonly Insane_MechanicalDB _contextDB;
        private HelperUploadFiles helperUpload;

        public AdminController(Insane_MechanicalDB contextDB, HelperUploadFiles helperUploadFiles)
        {
            _contextDB = contextDB;
            this.helperUpload = helperUploadFiles;
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

        [HttpGet]
        public IActionResult Articulos()
        {
            Cookies();

            var articulo = _contextDB.Articulo.ToList();

            return View(articulo);
        }

        [HttpGet]
        public IActionResult AgregarArticulos()
        {
            Cookies();

            ViewBag.Categorias = new SelectList(_contextDB.Categoria, "ID", "Titulo");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AgregarArticulos(Articulo post, string content, IFormFile Imagen)
        {
            var htmlStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            string fileName = $"{Guid.NewGuid()}.html";
            string imageName = $"{Guid.NewGuid()}";

            // Guardar el archivo HTML usando el método UploadHtmlAsync
            string filePath = await helperUpload.UploadHtmlAsync(htmlStream, fileName, Folders.HTML);
            string imagePath = await helperUpload.UploadFilesAsync(Imagen, imageName, Folders.Articulos);


            post.RutaHTML = "HTML/" + fileName;
            post.RutaImagen = "HTML/" + imageName;

            _contextDB.Articulo.Add(post);
            _contextDB.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
