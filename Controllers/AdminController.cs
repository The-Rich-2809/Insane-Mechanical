using Insane_Mechanical.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using Insane_Mechanical.Helpers;
using Insane_Mechanical.Providers;

namespace Insane_Mechanical.Controllers
{
    public class AdminController : Controller
    {
        public readonly Insane_MechanicalDB _contextDB;
        private HelperUploadFiles helperUpload;
        private IWebHostEnvironment hostEnvironment;

        public AdminController(Insane_MechanicalDB contextDB, HelperUploadFiles helperUploadFiles, IWebHostEnvironment hostEnvironment)
        {
            _contextDB = contextDB;
            this.helperUpload = helperUploadFiles;
            this.hostEnvironment = hostEnvironment;
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
            if (Imagen != null)
            {
                var htmlStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
                var id = _contextDB.Articulo.Max(c => c.ID) + 1;
                string fileName = post.CategoriaId + "_" + id + post.Titulo + ".cshtml";
                string imageName = $"{Guid.NewGuid()}.png";

                string imagePath = await helperUpload.UploadFilesAsync(Imagen, imageName, Folders.Articulos);
                // Guardar el archivo HTML usando el método UploadHtmlAsync
                string filePath = await helperUpload.UploadHtmlAsync(htmlStream, fileName, Folders.HTML);


                post.RutaHTML = "../HTML/" + fileName;
                post.RutaImagen = "../Images/Articulos/" + imageName;

                _contextDB.Articulo.Add(post);
                _contextDB.SaveChanges();

                return RedirectToAction("Articulos");
            }
            else
            {
                Cookies();

                ViewBag.Mensaje = "Seleccione una imagen";
                ViewBag.Categorias = new SelectList(_contextDB.Categoria, "ID", "Titulo");

                return View();
            }                
        }

        [HttpGet]
        public IActionResult NuevaCategoria()
        {
            Cookies();
            return View();
        }

        [HttpPost]
        public IActionResult NuevaCategoria(Categoria categoria)
        {
            Cookies();

            categoria.RutaImagen = "../Images/Categorias/ImgAuto.jpg";

            _contextDB.Categoria.Add(categoria);
            _contextDB.SaveChanges();

            return RedirectToAction("Articulos");
        }

        [HttpGet]
        public IActionResult EditarArticulo(int ID)
        {
            Cookies();
            var post = _contextDB.Articulo.Find(ID);
            if (post == null)
            {
                return NotFound();
            }

            ViewBag.Categorias = new SelectList(_contextDB.Categoria, "ID", "Titulo");

            // Leer el contenido HTML desde el archivo
            var htmlFilePath = Path.Combine(hostEnvironment.ContentRootPath, "Views", "HTML", Path.GetFileName(post.RutaHTML));

            // Depurar la ruta generada
            Console.WriteLine("Ruta del archivo HTML: " + htmlFilePath);

            if (System.IO.File.Exists(htmlFilePath))
            {
                Console.WriteLine("El archivo HTML existe.");
                ViewBag.HtmlContent = System.IO.File.ReadAllText(htmlFilePath);
            }
            else
            {
                Console.WriteLine("El archivo HTML no existe.");
                ViewBag.HtmlContent = string.Empty; // o manejar el caso en el que el archivo no exista
            }

            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> EditarArticulo(Articulo post, string content, IFormFile Imagen)
        {
            Cookies();
            var existingPost = _contextDB.Articulo.Find(post.ID);
            if (existingPost == null)
            {
                return NotFound();
            }

            existingPost.Descripcion = post.Descripcion;
            existingPost.CategoriaId = post.CategoriaId;
            existingPost.Titulo = post.Titulo;

            var htmlFilePath = Path.Combine(hostEnvironment.ContentRootPath, "Views", "HTML", Path.GetFileName(existingPost.RutaHTML));

            if (Imagen != null)
            {
                string imageName = $"{Guid.NewGuid()}.png";
                string imagePath = await helperUpload.UploadFilesAsync(Imagen, imageName, Folders.Articulos);
                existingPost.RutaImagen = "../Images/Articulos/" + imageName;
            }

            try
            {
                using (var writer = new StreamWriter(htmlFilePath, false, Encoding.UTF8))
                {
                    await writer.WriteAsync(content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al escribir el archivo: " + ex.Message);
                ModelState.AddModelError("", "No se pudo guardar el contenido HTML.");
                ViewBag.Categorias = new SelectList(_contextDB.Categoria, "ID", "Titulo", post.CategoriaId);
                return View(post);
            }

            _contextDB.Articulo.Update(existingPost);
            await _contextDB.SaveChangesAsync();

            return RedirectToAction("Articulos");
        }

        [HttpGet]
        public IActionResult EliminarArticulo(int id)
        {
            Cookies();

            var articulo = _contextDB.Articulo.Find(id);
            if (articulo == null)
            {
                return NotFound();
            }

            return View(articulo);
        }

        [HttpPost]
        public async Task<IActionResult> EliminarArticuloConfirmado(int id)
        {
            var articulo = _contextDB.Articulo.Find(id);
            if (articulo == null)
            {
                return NotFound();
            }

            // Eliminar archivos asociados
            var htmlFilePath = Path.Combine("Views", "BlogPosts", Path.GetFileName(articulo.RutaHTML));
            if (System.IO.File.Exists(htmlFilePath))
            {
                System.IO.File.Delete(htmlFilePath);
            }

            var imageFilePath = Path.Combine("Images", "Articulos", Path.GetFileName(articulo.RutaImagen));
            if (System.IO.File.Exists(imageFilePath))
            {
                System.IO.File.Delete(imageFilePath);
            }

            _contextDB.Articulo.Remove(articulo);
            await _contextDB.SaveChangesAsync();

            return RedirectToAction("Articulos");
        }

        [HttpGet]
        public IActionResult ListarPreguntas()
        {
            Cookies();
            var preguntas = _contextDB.Preguntas.Include(p => p.Opciones).ToList();
            return View(preguntas);
        }

        [HttpGet]
        public IActionResult CrearPregunta()
        {
            Cookies();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearPregunta(Pregunta pregunta)
        {
            _contextDB.Preguntas.Add(pregunta);
            await _contextDB.SaveChangesAsync();
            return RedirectToAction(nameof(ListarPreguntas));
        }

        [HttpGet]
        public IActionResult CrearOpcion(int preguntaId)
        {
            Cookies();
            var opcion = new Opcion { idPregunta = preguntaId };
            return View(opcion);
        }

        [HttpPost]
        public async Task<IActionResult> CrearOpcion(Opcion opcion)
        {
            _contextDB.Opciones.Add(opcion);
            await _contextDB.SaveChangesAsync();
            return RedirectToAction(nameof(ListarPreguntas));
        }

        [HttpGet]
        public async Task<IActionResult> EditarPregunta(int id)
        {
            Cookies();

            var pregunta = await _contextDB.Preguntas.FindAsync(id);
            if (pregunta == null)
            {
                return NotFound();
            }
            return View(pregunta);
        }

        [HttpPost]
        public async Task<IActionResult> EditarPregunta(Pregunta pregunta)
        {
            _contextDB.Entry(pregunta).State = EntityState.Modified;
            await _contextDB.SaveChangesAsync();
            return RedirectToAction(nameof(ListarPreguntas));
        }

        [HttpGet]
        public async Task<IActionResult> EditarOpcion(int id)
        {
            Cookies();

            var opcion = await _contextDB.Opciones.FindAsync(id);
            if (opcion == null)
            {
                return NotFound();
            }
            return View(opcion);
        }

        [HttpPost]
        public async Task<IActionResult> EditarOpcion(Opcion opcion)
        {
            _contextDB.Opciones.Update(opcion);
            await _contextDB.SaveChangesAsync();
            return RedirectToAction(nameof(ListarPreguntas));
        }

        [HttpGet]
        public async Task<IActionResult> EliminarPregunta(int id)
        {
            var pregunta = await _contextDB.Preguntas.FindAsync(id);
            if (pregunta == null)
            {
                return NotFound();
            }

            _contextDB.Preguntas.Remove(pregunta);
            await _contextDB.SaveChangesAsync();
            return RedirectToAction(nameof(ListarPreguntas));
        }

        [HttpGet]
        public async Task<IActionResult> EliminarOpcion(int id)
        {
            var opcion = await _contextDB.Opciones.FindAsync(id);
            if (opcion == null)
            {
                return NotFound();
            }

            _contextDB.Opciones.Remove(opcion);
            await _contextDB.SaveChangesAsync();
            return RedirectToAction(nameof(ListarPreguntas));
        }

        public async Task<IActionResult> Graficas()
        {
            Cookies();
            var preguntas = await _contextDB.Preguntas.ToListAsync();
            var respuestas = await _contextDB.RespuestaUsuario.Include(r => r.Opcion).ToListAsync();

            var datosPorPregunta = preguntas
                .Select(p => new
                {
                    Pregunta = p.Nombre,
                    RespuestasIncorrectas = respuestas.Count(r => r.idPregunta == p.ID && !r.Opcion.EsCorrecta),
                    RespuestasCorrectas = respuestas.Count(r => r.idPregunta == p.ID && r.Opcion.EsCorrecta)
                })
                .ToList();

            var preguntasMasErrores = datosPorPregunta
                .OrderByDescending(x => x.RespuestasIncorrectas)
                .ToList();

            var preguntasMenosErrores = datosPorPregunta
                .OrderBy(x => x.RespuestasIncorrectas)
                .ToList();

            ViewBag.PreguntasMasErrores = preguntasMasErrores.Select(x => x.Pregunta).ToList();
            ViewBag.RespuestasIncorrectasMasErrores = preguntasMasErrores.Select(x => x.RespuestasIncorrectas).ToList();
            ViewBag.RespuestasCorrectasMasErrores = preguntasMasErrores.Select(x => x.RespuestasCorrectas).ToList();

            ViewBag.PreguntasMenosErrores = preguntasMenosErrores.Select(x => x.Pregunta).ToList();
            ViewBag.RespuestasIncorrectasMenosErrores = preguntasMenosErrores.Select(x => x.RespuestasIncorrectas).ToList();
            ViewBag.RespuestasCorrectasMenosErrores = preguntasMenosErrores.Select(x => x.RespuestasCorrectas).ToList();

            return View();
        }
    }
}
