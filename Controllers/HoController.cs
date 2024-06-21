using Insane_Mechanical.Models;
using Insane_Mechanical.Services;
using Microsoft.AspNetCore.Mvc;

namespace Insane_Mechanical.Controllers
{
    public class HoController : Controller
    {
        private readonly Insane_MechanicalDB _contextDB;

        public HoController(Insane_MechanicalDB contextDB)
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

        public IActionResult Index1()
        {
            Cookies();
            return View();
        }

        public IActionResult Index2()
        {
            Cookies();
            return View();
        }
        public IActionResult Index3()
        {
            Cookies();
            return View();
        }
        public IActionResult Index4()
        {
            Cookies();
            return View();
        }
        public IActionResult Index5()
        {
            Cookies();
            return View();
        }
    }
}
