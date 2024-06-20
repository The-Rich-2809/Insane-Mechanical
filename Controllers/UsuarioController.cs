using Insane_Mechanical.Helpers;
using Insane_Mechanical.Models;
using Microsoft.AspNetCore.Mvc;

namespace Insane_Mechanical.Controllers
{
    public class UsuarioController : Controller
    {
        public readonly Insane_MechanicalDB _contextDB;

        public UsuarioController(Insane_MechanicalDB contextDB)
        {
            _contextDB = contextDB;
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

        public IActionResult Cotizador()
        {
            Cookies();   
            return View();
        }
    }
}
