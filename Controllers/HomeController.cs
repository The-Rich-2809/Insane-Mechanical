using Insane_Mechanical.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Insane_Mechanical.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Insane_MechanicalDB _contextDB;

        public HomeController(ILogger<HomeController> logger, Insane_MechanicalDB contextDB)
        {
            _logger = logger;
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
            Initialize();
            Cookies();
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(Usuario usuario)
        {
            UsuarioModel login = new UsuarioModel(_contextDB);

            login.Correo = usuario.Correo;
            login.Contrasena = usuario.Contrasena;

            if (login.Login())
            {
                CookieOptions options = new CookieOptions();
                options.Expires = DateTime.Now.AddDays(365);
                options.IsEssential = true;
                options.Path = "/";
                HttpContext.Response.Cookies.Append("MiCookie", usuario.Correo, options);

                return RedirectToAction("Index");
            }
            ViewBag.Mensaje = UsuarioModel.Mensaje;
            return View(usuario);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(string Contrasena2, Usuario usuario)
        {
            UsuarioModel register = new UsuarioModel(_contextDB);

            register.Correo = usuario.Correo;
            register.Contrasena = usuario.Contrasena;
            register.Contrasena2 = Contrasena2;
            UsuarioModel.Nombre = usuario.Nombre;

            if (register.Register())
            {
                CookieOptions options = new CookieOptions();
                options.Expires = DateTime.Now.AddDays(365);
                options.IsEssential = true;
                options.Path = "/";
                HttpContext.Response.Cookies.Append("MiCookie", usuario.Correo, options);

                if (register.Login())
                    return RedirectToAction("Index");
            }
            else
                ViewBag.Mensaje = UsuarioModel.Mensaje;

            return View(usuario);
        }
        [HttpGet]
        public IActionResult ContrasenaOlvidada()
        {
            return View();
        }
        [HttpGet]
        public IActionResult CerrarSesion()
        {
            HttpContext.Response.Cookies.Delete("MiCookie");
            return RedirectToAction("Index");
        }


        public void Initialize()
        {
            _contextDB.Database.EnsureCreated();

            if (_contextDB.Usuario.Any())
            {
                return;
            }

            var insertarusuarios = new Usuario[]
                {
                    new Usuario() {Correo = "ricardo_138@outlook.com", Contrasena = "1234", TipoUsuario = "Admin", DireccionImagen = "/Images/Usuarios/Rich.jpg", Nombre = "Rich"},
                    new Usuario() {Correo = "aserranoacosta841@gmail.com", Contrasena = "1234", TipoUsuario = "Admin", DireccionImagen = "/Images/Usuarios/Alejandro.jpg", Nombre = "Alejandro"}
                };

            var insertarcategorias = new Categoria[]
            {
                new Categoria(){Titulo = "Mantenimiento", Descripcion = "Aqui se encontrara info para mantener tu choche", RutaImagen = "../Images/Categorias/ImgAuto.jpg" },
                new Categoria(){Titulo = "Reparaciones", Descripcion = "Aqui se encontrara info para reparar tu choche", RutaImagen = "../Images/Categorias/ImgAuto.jpg" }
            };
            var insertararticulo = new Articulo[]
            {
                new Articulo(){Titulo = "Cambio de aceite", Descripcion = "Info para cambiar tu aceite", RutaImagen = "../Images/Categorias/ImgAuto.jpg", CategoriaId = 1, RutaHTML = "../HTML/1_1_Hola.cshtml" },
                new Articulo(){Titulo = "Cambio de marcha", Descripcion = "Info para cambiar tu marcha", RutaImagen = "../Images/Categorias/ImgAuto.jpg", CategoriaId = 2, RutaHTML = "../HTML/1_1_Hola.cshtml" },
            };
            var insertarcomentario = new Comentario[]
            {
                new Comentario(){IdArticulo = 1, IdUsuario = 1, Texto = "Hola, este es mi primer comentario" }
            };

            foreach (var u in insertarusuarios)
                _contextDB.Usuario.Add(u);

            foreach(var u in insertarcategorias)
                _contextDB.Categoria.Add(u);

            foreach (var u in insertararticulo)
                _contextDB.Articulo.Add(u);

            foreach(var u in insertarcomentario)
                _contextDB.Comentario.Add(u);

            _contextDB.SaveChanges();
        }

            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
