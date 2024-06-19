using Insane_Mechanical.Models;
using Insane_Mechanical.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using System.Net;

namespace Insane_Mechanical.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly VerificationService _verificationService;
        private readonly WhatsAppServices _whatsappService;
        private readonly Insane_MechanicalDB _contextDB;

        public static Usuario usuario1 { get; set; }
        public static string Correo {  get; set; }
        public static int opcion {  get; set; }

        public HomeController(ILogger<HomeController> logger, Insane_MechanicalDB contextDB, VerificationService verificationService, WhatsAppServices whatsappService)
        {
            _logger = logger;
            _contextDB = contextDB;
            _verificationService = verificationService;
            _whatsappService = whatsappService;
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
        public IActionResult Login(Usuario usuario, string selectedOption)
        {
            UsuarioModel login = new UsuarioModel(_contextDB);

            login.Correo = usuario.Correo;
            login.Contrasena = usuario.Contrasena;

            if (login.Login())
            {
                string verificationCode = GenerateVerificationCode();

                var idusuario = _contextDB.Usuario.FirstOrDefault(u => u.Correo == usuario.Correo).ID;
                usuario1 = usuario;
                usuario1.ID = idusuario;

                if (selectedOption == "Correo")
                {
                    _verificationService.SaveVerificationCode(idusuario, verificationCode);

                    var smtpClient = new SmtpClient("smtp.gmail.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential("insanusmechanica@gmail.com", "kgfzujdveakfnlip"),
                        EnableSsl = true,
                    };

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("insanusmechanica@gmail.com"),
                        Subject = "Codigo de verificacion",
                        Body =  $"Tu codigo de verificacion es {verificationCode}",
                        IsBodyHtml = true,
                    };
                    mailMessage.To.Add($"{usuario.Correo}");

                    smtpClient.Send(mailMessage);

                    opcion = 1;

                    return RedirectToAction("Verificar", new { userId = usuario1.ID });
                }
                else
                {
                    var whatsappNumber = _contextDB.Usuario.FirstOrDefault(u => u.Correo == usuario.Correo).Telefono;

                    // Guardar el código de verificación en la base de datos
                    _verificationService.SaveVerificationCode(idusuario, verificationCode);

                    // Enviar el código de verificación a WhatsApp
                    _whatsappService.SendVerificationCode(whatsappNumber, verificationCode);

                    // Redirigir al usuario a la página de verificación
                    return RedirectToAction("Verificar", new { userId = usuario1.ID });
                }
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
        public IActionResult Register(string Contrasena2, Usuario usuario, string selectedOption)
        {
            UsuarioModel register = new UsuarioModel(_contextDB);

            register.Correo = usuario.Correo;
            register.Contrasena = usuario.Contrasena;
            register.Contrasena2 = Contrasena2;
            UsuarioModel.Telefono = usuario.Telefono;
            UsuarioModel.Nombre = usuario.Nombre;
            register.Genero = usuario.Genero;

            if (register.Register())
            {
                opcion = 2;

                string verificationCode = GenerateVerificationCode();

                var idusuario = _contextDB.Usuario.FirstOrDefault(u => u.Correo == usuario.Correo).ID;
                usuario1 = usuario;
                usuario1.ID = idusuario;

                if (selectedOption == "Correo")
                {
                    _verificationService.SaveVerificationCode(idusuario, verificationCode);

                    var smtpClient = new SmtpClient("smtp.gmail.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential("insanusmechanica@gmail.com", "kgfzujdveakfnlip"),
                        EnableSsl = true,
                    };

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("insanusmechanica@gmail.com"),
                        Subject = "Codigo de Verificacion",
                        Body = $"Tu codigo de verificacion es {verificationCode}",
                        IsBodyHtml = true,
                    };
                    mailMessage.To.Add($"{usuario.Correo}");

                    smtpClient.Send(mailMessage);

                    opcion = 2;

                    return RedirectToAction("Verificar", new { userId = usuario1.ID });
                }
                else
                {
                    var whatsappNumber = _contextDB.Usuario.FirstOrDefault(u => u.Correo == usuario.Correo).Telefono;

                    // Guardar el código de verificación en la base de datos
                    _verificationService.SaveVerificationCode(idusuario, verificationCode);

                    // Enviar el código de verificación a WhatsApp
                    _whatsappService.SendVerificationCode(whatsappNumber, verificationCode);

                    // Redirigir al usuario a la página de verificación
                    return RedirectToAction("Verificar", new { userId = usuario1.ID });
                }
            }
            else
                ViewBag.Mensaje = UsuarioModel.Mensaje;

            return View(usuario);
        }

        [HttpGet]
        public IActionResult Verificar(int userId)
        {
            Cookies();
            return View(new VerifyViewModel { idUsuario = userId });
        }

        [HttpPost]
        public IActionResult Verificar(VerifyViewModel model)
        {
            if (_verificationService.VerifyCode(model.idUsuario, model.CodigoIngresado))
            {
                MarkUserAsVerified(model.idUsuario);
                CookieOptions options = new CookieOptions();
                options.Expires = DateTime.Now.AddDays(365);
                options.IsEssential = true;
                options.Path = "/";
                HttpContext.Response.Cookies.Append("MiCookie", usuario1.Correo, options);

                if (opcion == 1)
                    return RedirectToAction("VerificacionExitosa");
                else
                {
                    UsuarioModel register = new UsuarioModel(_contextDB);

                    var usuario = _contextDB.Usuario.Find(model.idUsuario);

                    register.Contrasena = usuario.Contrasena;
                    register.Correo = usuario.Correo;

                    if (register.Login())
                        return RedirectToAction("VerificacionExitosa");
                }

                return RedirectToAction("VerificacionExitosa");
            }
            else
            {
                return View("VerificacionFallida");
            }
        }

        [HttpGet]
        public IActionResult VerificacionExitosa()
        {
            Cookies();
            return View();
        }
        [HttpGet]
        public IActionResult VerificacionFallida()
        {
            Cookies();
            return View();
        }

        [HttpGet]
        public IActionResult ContrasenaOlvidada()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ContrasenaOlvidada(string correo)
        {
            var email = _contextDB.Usuario.FirstOrDefault(u => u.Correo == correo);

            if (email != null)
            {
                Correo = correo;
                return RedirectToAction("CambiarContrasena");
            }

            ViewBag.Mensaje = "Correo no encontrado";

            return View();
        }

        [HttpGet]
        public IActionResult CambiarContrasena()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CambiarContrasena(string Contrasena, string Contrasena2)
        {
            if (Contrasena == Contrasena2)
            {
                var usuario = _contextDB.Usuario.FirstOrDefault(u => u.Correo == Correo);

                usuario.Contrasena = Contrasena2;

                _contextDB.Usuario.Update(usuario);
                _contextDB.SaveChanges();

                return RedirectToAction("Login");
            }

            ViewBag.Mensaje = "Las contraseñas no coinciden";

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
                new Usuario() {Correo = "ricardo_138@outlook.com", Contrasena = "1234", TipoUsuario = "Admin", DireccionImagen = "/Images/Usuarios/Rich.jpg", Nombre = "Rich", Telefono = "56 1898 4344", Genero="Hombre"},
                new Usuario() {Correo = "aserranoacosta841@gmail.com", Contrasena = "1234", TipoUsuario = "Admin", DireccionImagen = "/Images/Usuarios/Alejandro.jpg", Nombre = "Alejandro", Telefono = "5541278159", Genero="Hombre"}
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

            foreach (var u in insertarcategorias)
                _contextDB.Categoria.Add(u);

            foreach (var u in insertararticulo)
                _contextDB.Articulo.Add(u);

            foreach (var u in insertarcomentario)
                _contextDB.Comentario.Add(u);

            _contextDB.SaveChanges();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string GenerateVerificationCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();  // Genera un código de 6 dígitos
        }

        private void MarkUserAsVerified(int userId)
        {
            var user = _contextDB.Usuario.Find(userId);
            if (user != null)
            {
                user.Verificado = true;
                _contextDB.SaveChanges();
            }
        }

        public IActionResult Buscar(string query)
        {
            Cookies();

            if (string.IsNullOrEmpty(query))
            {
                ViewBag.Message = "Por favor, ingrese un término de búsqueda.";
                return View(new List<Articulo>());
            }

            var resultados = _contextDB.Articulo
                .Where(a => a.Titulo.Contains(query) || a.Descripcion.Contains(query))
                .ToList();

            if (!resultados.Any())
            {
                ViewBag.Message = "No se encontraron artículos que coincidan con su búsqueda.";
            }

            return View(resultados);
        }
    }
}
