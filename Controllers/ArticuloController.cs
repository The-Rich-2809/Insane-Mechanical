using Insane_Mechanical.Models;
using Microsoft.AspNetCore.Mvc;
using Insane_Mechanical.Models;
using System.Collections.Generic;

namespace Insane_Mechanical.Controllers
{
    public class ArticuloController : Controller
    {
        private readonly Insane_MechanicalDB _contextDB;
        public ArticuloController(ILogger<ArticuloController> logger, Insane_MechanicalDB contextDB)
        {
            _contextDB = contextDB;
        }
        public static int IdCategoria { get; set; }
        public void Cookies()
        {
            var miCookie = HttpContext.Request.Cookies["MiCookie"];

            ViewBag.cookie = miCookie;

            if (miCookie != null)
            {
                List<Usuario> listaUsuarios = _contextDB.Usuario.ToList();
                foreach (var user in listaUsuarios)
                {
                    if (miCookie == user.Correo)
                    {
                        ViewBag.ID = user.ID;
                        ViewBag.Nombre = user.Nombre;
                        ViewBag.Nivel = user.TipoUsuario;
                        ViewBag.FotoPerfil = user.DireccionImagen;
                    }
                }
            }
            List<Categoria> categorias = _contextDB.Categoria.ToList();
            ViewBag.Categorias = categorias;
        }
        [HttpGet]
        public IActionResult Categorias(int CategoriaId)
        {
            IdCategoria = CategoriaId;
            ViewBag.CategoriaId = CategoriaId;
            List<Articulo> articulos = _contextDB.Articulo.ToList();
            Cookies();
            return View(articulos);
        }
        [HttpPost]
        public IActionResult Categorias(string Categora)
        {
            return View();
        }
        [HttpGet]
        public IActionResult Articulo(string RutaHtml, int IdArticulo)
        {
            ViewBag.RutaHtml = RutaHtml;
            ViewBag.IdArticulo = IdArticulo;

            List<Comentario> comentarios = _contextDB.Comentario.ToList();
            ViewBag.Comentarios = comentarios;

            List<Articulo> articulos = _contextDB.Articulo.ToList();
            Cookies();
            return View(articulos);
        }
    }
}
