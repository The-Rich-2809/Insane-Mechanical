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

            ViewBag.cookie = miCookie;

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

            for (int i = 0; i < 10; i++)
                Console.WriteLine("Arteaga es un pendejo");

            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult IndexLogeado()
        {
            Cookies();

            for (int i = 0; i < 10; i++)
                Console.WriteLine("Arteaga es un pendejo");

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
        public IActionResult Register(string Contrasena2, Usuario usuario, string selectedOption, string Correo2, string Telefono2)
        {
            UsuarioModel register = new UsuarioModel(_contextDB);

            register.Correo = usuario.Correo;
            register.Contrasena = usuario.Contrasena;
            register.Contrasena2 = Contrasena2;
            UsuarioModel.Telefono = usuario.Telefono;
            UsuarioModel.Nombre = usuario.Nombre;
            register.Genero = usuario.Genero;
            UsuarioModel.Telefono2 = Telefono2;
            register.Correo2 = Correo2;

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
                            new Usuario() {Correo = "gonzalezavilamanuelpatricio@gmail.com", Contrasena = "1234", TipoUsuario = "Admin", DireccionImagen = "/Images/Usuarios/ImgPatrick.jpg", Nombre = "Patrick", Telefono = "221 776 1280", Genero="Hombre"},
                                 new Usuario() {Correo = "emirodriguezcruz2004@gmail.com", Contrasena = "1234", TipoUsuario = "Admin", DireccionImagen = "/Images/Usuarios/ImgDavid.jpg", Nombre = "David", Telefono = "5534728461", Genero="Hombre"},
                                new Usuario() {Correo = "gonzalezavilamanuelpatricio@gmail.com", Contrasena = "1234", TipoUsuario = "Admin", DireccionImagen = "/Images/Usuarios/Rich.jpg", Nombre = "Manuel", Telefono = "2217761280", Genero="Hombre"},
                new Usuario() {Correo = "ricardo_138@outlook.com", Contrasena = "1234", TipoUsuario = "Admin", DireccionImagen = "/Images/Usuarios/Rich.jpg", Nombre = "Rich", Telefono = "56 1898 4344", Genero="Hombre"},
                new Usuario() {Correo = "aserranoacosta841@gmail.com", Contrasena = "1234", TipoUsuario = "Admin", DireccionImagen = "/Images/Usuarios/Alejandro.jpg", Nombre = "Alejandro", Telefono = "5541278159", Genero="Hombre"}
            };

            var insertarcategorias = new Categoria[]
            {
                new Categoria(){Titulo = "Funcionamiento", Descripcion = "Aqui se encontrara info para mantener tu choche", RutaImagen = "../Images/Categorias/ImgAuto.jpg" },
                new Categoria(){Titulo = "Cuidado y prevencion", Descripcion = "Aqui se encontrara info para reparar tu choche", RutaImagen = "../Images/Categorias/ImgAuto.jpg" },
                new Categoria(){Titulo = "Trámites y Normas de Tránsito", Descripcion = "Tramites importantes", RutaImagen = "../Images/Categorias/ImgAuto.jpg" },
            };

            var insertararticulo = new Articulo[]
            {
                new Articulo(){Titulo = "Cambio de aceite", Descripcion = "Info para cambiar tu aceite", RutaImagen = "../Images/Articulos/cambioDeAceiteEst.jpg", CategoriaId = 1, RutaHTML = "../HTML/1_1_Hola.cshtml" },
                new Articulo(){Titulo = "Funcionamiento básico del motor", Descripcion = "Aquí te explicamos sobre como funciona el motor de un auto estandar en general.", RutaImagen = "../Images/Articulos/MotorEst.jpg", CategoriaId = 1, RutaHTML = "../HTML/1_2Funcionamiento básico del motor.cshtml" },
                new Articulo(){Titulo = "Funcionamiento básico de la batería", Descripcion = "Aquí te explicamos sobre como funciona la batería de un auto estándar en general.", RutaImagen = "../Images/Articulos/BateríaEst.jpg", CategoriaId = 1, RutaHTML = "../HTML/1_8Funcionamiento básico de la batería.cshtml" },
                new Articulo(){Titulo = "Funcionamiento básico de la caja de cambios", Descripcion = "Aquí te explicamos sobre como funciona la caja de cambios de un auto estándar en general.", RutaImagen = "../Images/Articulos/CajaDeCambiosEst.jpg", CategoriaId = 1, RutaHTML = "../HTML/1_9Funcionamiento básico de la caja de cambios.cshtml" },
                new Articulo(){Titulo = "Funcionamiento básico del chasis", Descripcion = "Aquí te explicamos sobre como funciona el chasis de un auto estándar en general.", RutaImagen = "../Images/Cuidado y prevencion/ChasisEst.jpg", CategoriaId = 1, RutaHTML = "../HTML/1_10Funcionamiento básico del chasis.cshtml" },
                new Articulo(){Titulo = "Funcionamiento básico de los fusibles", Descripcion = "Aquí te explicamos sobre como funcionan los fusibles de un auto estándar en general.", RutaImagen = "../Images/Cuidado y prevencion/FusiblesEst.jpg", CategoriaId = 1, RutaHTML = "../HTML/1_11Funcionamiento básico de los fusibles.cshtml" },
                new Articulo(){Titulo = "Funcionamiento básico del alternador", Descripcion = "Aquí te explicamos sobre como funciona el alternador de un auto estándar en general.", RutaImagen = "../Images/Articulos/AlternadorEst.jpg", CategoriaId = 1, RutaHTML = "../HTML/1_12Funcionamiento básico del alternador.cshtml" },
                new Articulo(){Titulo = "Funcionamiento básico del radiador", Descripcion = "Aquí te explicamos sobre como funciona el radiador de un auto estándar en general.", RutaImagen = "../Images/Articulos/RadiadorEst.jpg", CategoriaId = 1, RutaHTML = "../HTML/1_13Funcionamiento básico del radiador.cshtml" },
                new Articulo(){Titulo = "Funcionamiento básico de la dirección", Descripcion = "Aquí te explicamos sobre como funciona la dirección de un auto estándar en general.", RutaImagen = "../Images/Articulos/DirecciónEst.jpg", CategoriaId = 1, RutaHTML = "../HTML/1_14Funcionamiento básico de la dirección.cshtml" },
                new Articulo(){Titulo = "Funcionamiento básico de la suspensión", Descripcion = "Aquí te explicamos sobre como funciona la suspensión de un auto estándar en general.", RutaImagen = "../Images/Articulos/SuspensiónEst.jpg", CategoriaId = 1, RutaHTML = "../HTML/1_15Funcionamiento básico de la suspensión.cshtml" },
                new Articulo(){Titulo = "Funcionamiento básico de los frenos", Descripcion = "Aquí te explicamos sobre como funcionan los frenos de un auto estándar en general.", RutaImagen = "../Images/Articulos/FrenosEst.jpg", CategoriaId = 1, RutaHTML = "../HTML/1_16Funcionamiento básico de los frenos.cshtml" },
                new Articulo(){Titulo = "Funcionamiento básico de las ruedas", Descripcion = "Aquí te explicamos sobre como funcionan las ruedas de un auto estándar en general.", RutaImagen = "../Images/Articulos/RuedaEst.jpg", CategoriaId = 1, RutaHTML = "../HTML/1_17Funcionamiento básico de las ruedas.cshtml" },
                new Articulo(){Titulo = "Funcionamiento básico de la válvula de escape", Descripcion = "Aquí te explicamos sobre como funcionan la válvula de escape de un auto estándar en general.", RutaImagen = "../Images/Articulos/ValvulaDeEscapeEst.jpg", CategoriaId = 1, RutaHTML = "../HTML/1_18Funcionamiento básico de la válvula de escape.cshtml" },
                /*Añadido por patrick*/
                 new Articulo(){Titulo = "Verificación vehicular", Descripcion = "Descubre cómo la verificación vehicular puede mejorar tu vida y proteger el medio ambiente.", RutaImagen = "../Images/Cuidado y prevencion/VerificacionEst.jpg", CategoriaId = 3, RutaHTML = "../HTML/3_16Verificación vehicular.cshtml" },
                        new Articulo(){Titulo = "Funcionamiento básico de la caja de cambios automatica Basico", Descripcion = "Aquí te explicamos sobre como funciona la caja de cambios automatica", RutaImagen = "../Images/Cuidado y prevencion/CajaAutomaticaEst.jpg", CategoriaId = 1, RutaHTML = "../HTML/1_15Funcionamiento básico de la caja de cambios automatica.cshtml" },
                          new Articulo(){Titulo = "Cuidado De La Bateria y Mas", Descripcion = "Aprende mas de tu bateria", RutaImagen = "../Images/Articulos/BateríaEst.jpg", CategoriaId = 2, RutaHTML = "../HTML/2_16Cuidado De La Bateria y Mas.cshtml" },
                            new Articulo(){Titulo = "Cuidado De Tus Frenos", Descripcion = "Aprende a Cuidar Tus Frenos Discos", RutaImagen = "../Images/Cuidado y prevencion/CuidadoDeFrenosEst.jpg", CategoriaId = 2, RutaHTML = "../HTML/2_17Cuidado De Tus Frenos.cshtml" },
                             new Articulo(){Titulo = "Sobrecalentamiento", Descripcion = "te ayudamos a saber si es tu radiador", RutaImagen = "../Images/Cuidado y prevencion/FugaDelRadiadorEst.jpg", CategoriaId = 2, RutaHTML = "../HTML/2_19Sobrecalentamiento del radiador .cshtml" },
                               new Articulo(){Titulo = "Problemas de alternador", Descripcion = "Si tienes problemas con este elemento, tu auto no acelerará de forma adecuada", RutaImagen = "../Images/Cuidado y prevencion/AlternadorEst.jpg", CategoriaId = 2, RutaHTML = "../HTML/2_19Problemas de alternador.cshtml" },
                                  new Articulo(){Titulo = "Cambio de bujias", Descripcion = "aqui aprenderas a como es un cambio de bujias", RutaImagen = "../Images/Cuidado y prevencion/bujiasEst.jpg", CategoriaId = 2, RutaHTML = "../HTML/2_20Cambio de bujias.cshtml" },
                                    new Articulo(){Titulo = "Llantas ponchadas", Descripcion = "Aqui aprenderas a cambiar una llanta de manera obtima", RutaImagen = "../Images/Cuidado y prevencion/CambioDeLlantaEst.jpg", CategoriaId = 2, RutaHTML = "../HTML/2_21Llantas ponchadas.cshtml" },
                                    new Articulo(){Titulo = "Reglamento de transito", Descripcion = "Articulo 1", RutaImagen = "../Images/Cuidado y prevencion/ReglamentoEst.jpg", CategoriaId = 3, RutaHTML = "../HTML/3_22Reglamento de transito .cshtml" },
                                    new Articulo(){Titulo = "Reglamento de transito", Descripcion = "Articulo 2", RutaImagen = "../Images/Cuidado y prevencion/PoliciaDeTransito.jpg", CategoriaId = 3, RutaHTML = "../HTML/3_23Reglamento de transito .cshtml" },
                                    new Articulo(){Titulo = "Reglamento de transito", Descripcion = "Articulo 3", RutaImagen = "../Images/Cuidado y prevencion/Art3Est.jpeg", CategoriaId = 3, RutaHTML = "../HTML/3_24Reglamento de transito .cshtml" },
                                    new Articulo(){Titulo = "Reglamento de transito", Descripcion = "Articulo 4", RutaImagen = "../Images/Cuidado y prevencion/Art4Est.jpg", CategoriaId = 3, RutaHTML = "../HTML/3_25Reglamento de transito .cshtml" },
                                     new Articulo(){Titulo = "Documentos que siempre debes de cargar", Descripcion = "Jamás conduzcas sin estos documentos", RutaImagen = "../Images/Cuidado y prevencion/DocumentacionEst.jpg", CategoriaId = 3, RutaHTML = "../HTML/3_26Documentos que siempre debes de cargar.cshtml" },
                                       new Articulo(){Titulo = "Seguros Vehiculares", Descripcion = "Aprende mas de los seguros", RutaImagen = "../Images/Cuidado y prevencion/SeguroEst.jpeg", CategoriaId = 3, RutaHTML = "../HTML/3_27Seguros Vehiculares.cshtml" },
                                        new Articulo(){Titulo = "Protección del Consumidor", Descripcion = "Información sobre los derechos del consumidor en el ámbito de los servicios de reparación y mantenimiento", RutaImagen = "../Images/Cuidado y prevencion/proteccion-al-consumidorEst.png", CategoriaId = 3, RutaHTML = "../HTML/3_28Protección del Consumidor" },
                                          new Articulo(){Titulo = "Modificaciones al reglamento de tránsito", Descripcion = "Nuevo reglamento de transito", RutaImagen = "../Images/Cuidado y prevencion/Nuevo reglamento de tRANSITO.jpg", CategoriaId = 3, RutaHTML = "../HTML/3_29Modificaciones al reglamento de tránsito.cshtml" },


            };

            var insertarcomentario = new Comentario[]
            {
                new Comentario(){IdArticulo = 1, IdUsuario = 1, Texto = "Hola, este es mi primer comentario" }
            };

            var insetarpregunta = new Pregunta[]
                {
                    new Pregunta() { Nombre = "¿En qué artículo se encuentra la función del reglamento de tránsito?", Tipo = "Seleccion" },
                    new Pregunta() { Nombre = "¿Bajo qué principios está basado el reglamento de tránsito?", Tipo = "Radio" },
                    new Pregunta() { Nombre = "¿Quiénes son las autoridades competentes para aplicar el reglamento de tránsito?", Tipo = "Seleccion" },
                    new Pregunta() { Nombre = "Según el artículo 4, ¿qué se entiende por agente?", Tipo = "Radio" },
                    new Pregunta() { Nombre = "¿Los conductores a qué están obligados?", Tipo = "Seleccion" },
                    new Pregunta() { Nombre = "¿Cuáles son las personas que deben contribuir a un ambiente sano de convivencia?", Tipo = "Radio" },
                    new Pregunta() { Nombre = "¿Si un conductor no obedece las indicaciones de los agentes autorizados para infraccionar, de cuánto será la sanción según la unidad de medida?", Tipo = "Seleccion" },
                    new Pregunta() { Nombre = "¿Cuál es la distancia que debe dejar el vehículo motorizado si adelanta a un ciclista o motociclista?", Tipo = "Radio" },
                    new Pregunta() { Nombre = "¿De qué habla el artículo número 12 del reglamento de tránsito?", Tipo = "Seleccion" },
                    new Pregunta() { Nombre = "¿Qué tipo de vehículos tiene preferencia de paso sobre vehículos motorizados?", Tipo = "Radio" },
                    new Pregunta() { Nombre = "¿Cuáles son lugares en los cuales está prohibido estacionar un vehículo?", Tipo = "Seleccion" },
                    new Pregunta() { Nombre = "¿Es posible inmovilizar un auto si el conductor o pasajero se encuentra en este?", Tipo = "Radio" },
                    new Pregunta() { Nombre = "¿En qué lugar deben colocar pasajeros menores de 12 años o que midan menos de 1.45 mts?", Tipo = "Seleccion" },
                    new Pregunta() { Nombre = "¿Cuáles son los elementos que debe contar un vehículo que circule en el territorio mexicano?", Tipo = "Radio" },
                    new Pregunta() { Nombre = "¿En qué artículo del reglamento de tránsito nos habla de los programas ambientales?", Tipo = "Seleccion" },
                    new Pregunta() { Nombre = "¿Qué vehículos están exentos del artículo 47 del reglamento de tránsito?", Tipo = "Radio" },
                    new Pregunta() { Nombre = "¿De qué habla el artículo 40 del reglamento de tránsito?", Tipo = "Seleccion" },
                    new Pregunta() { Nombre = "¿En qué momento se determina que la persona está bajo sustancias de alcohol si esta se encuentra manejando un vehículo motorizado?", Tipo = "Radio" },
                    new Pregunta() { Nombre = "¿En qué lugares los integrantes de seguridad ciudadana pueden detener la marcha para verificar que el conductor no se encuentre bajo los efectos del alcohol, narcóticos?", Tipo = "Seleccion" },
                    new Pregunta() { Nombre = "¿De qué forma se procede si hubo un hecho de tránsito con lesiones materiales (sin muertes)?", Tipo = "Radio" },
                    new Pregunta() { Nombre = "¿De qué forma se procede si hubo un hecho de tránsito con lesiones materiales y muertes?", Tipo = "Seleccion" }
                };

            var insertaropciones = new Opcion[]
                {
                    new Opcion() { Texto = "Artículo 1", EsCorrecta = true, idPregunta = 1 },
                    new Opcion() { Texto = "Artículo 10", EsCorrecta = false, idPregunta = 1 },
                    new Opcion() { Texto = "Artículo 3", EsCorrecta = false, idPregunta = 1 },
                    new Opcion() { Texto = "Artículo 15", EsCorrecta = false, idPregunta = 1 },

                    new Opcion() { Texto = "Seguridad vial, circulación en vía pública con cortesía, abstención de objetos en vías peatonales y de circulación.", EsCorrecta = true, idPregunta = 2 },
                    new Opcion() { Texto = "No molestar al peatón, mantener silencio, abastecerse de gasolina suficiente y sustentable.", EsCorrecta = false, idPregunta = 2 },
                    new Opcion() { Texto = "Aumentar la seguridad vital, circular en ambos sentidos, respetar semáforos y conducir de forma regular.", EsCorrecta = false, idPregunta = 2 },
                    new Opcion() { Texto = "Bajar las luces, tener permisos, buscar carriles libres y respetar los señalamientos.", EsCorrecta = false, idPregunta = 2 },

                    new Opcion() { Texto = "Seguridad ciudadana y las personas titulares de los juzgados.", EsCorrecta = true, idPregunta = 3 },
                    new Opcion() { Texto = "Los policías y propietarios de zonas privadas.", EsCorrecta = false, idPregunta = 3 },
                    new Opcion() { Texto = "Los conductores.", EsCorrecta = false, idPregunta = 3 },
                    new Opcion() { Texto = "Los que respetan los señalamientos.", EsCorrecta = false, idPregunta = 3 },

                    new Opcion() { Texto = "Integrante de la policía de control de tránsito.", EsCorrecta = true, idPregunta = 4 },
                    new Opcion() { Texto = "Trabajador secreto del gobierno.", EsCorrecta = false, idPregunta = 4 },
                    new Opcion() { Texto = "Personal entrenado.", EsCorrecta = false, idPregunta = 4 },
                    new Opcion() { Texto = "Integrante ejecutivo del Estado.", EsCorrecta = false, idPregunta = 4 },

                    new Opcion() { Texto = "Preferencias de paso a peatones, intersecciones ya sea que se tenga semáforo o no.", EsCorrecta = true, idPregunta = 5 },
                    new Opcion() { Texto = "A conducir en estado de sobriedad.", EsCorrecta = false, idPregunta = 5 },
                    new Opcion() { Texto = "Tapar los cruces peatonales.", EsCorrecta = false, idPregunta = 5 },
                    new Opcion() { Texto = "Respetar los límites de velocidad.", EsCorrecta = false, idPregunta = 5 },

                    new Opcion() { Texto = "Conductores y/o pasajeros.", EsCorrecta = true, idPregunta = 6 },
                    new Opcion() { Texto = "Las familias.", EsCorrecta = false, idPregunta = 6 },
                    new Opcion() { Texto = "Los peatones.", EsCorrecta = false, idPregunta = 6 },
                    new Opcion() { Texto = "Los policías de tránsito.", EsCorrecta = false, idPregunta = 6 },

                    new Opcion() { Texto = "10, 15 o 20.", EsCorrecta = true, idPregunta = 7 },
                    new Opcion() { Texto = "5, 10, 15.", EsCorrecta = false, idPregunta = 7 },
                    new Opcion() { Texto = "2, 6, 8.", EsCorrecta = false, idPregunta = 7 },
                    new Opcion() { Texto = "10, 20, 30.", EsCorrecta = false, idPregunta = 7 },

                    new Opcion() { Texto = "1.50 mts", EsCorrecta = true, idPregunta = 8 },
                    new Opcion() { Texto = "1.05 mts.", EsCorrecta = false, idPregunta = 8 },
                    new Opcion() { Texto = "1.10 mts", EsCorrecta = false, idPregunta = 8 },
                    new Opcion() { Texto = "1.00 mt.", EsCorrecta = false, idPregunta = 8 },

                    new Opcion() { Texto = "Está prohibido remolcar o empujar otros vehículos motorizados si no por medio de una grúa.", EsCorrecta = true, idPregunta = 9 },
                    new Opcion() { Texto = "De los señalamientos que debe tener el vehículo.", EsCorrecta = false, idPregunta = 9 },
                    new Opcion() { Texto = "Del espacio dentro de los vehículos.", EsCorrecta = false, idPregunta = 9 },
                    new Opcion() { Texto = "De los permisos para ser un vehículo de transporte público.", EsCorrecta = false, idPregunta = 9 },

                    new Opcion() { Texto = "Vehículos no motorizados", EsCorrecta = true, idPregunta = 10 },
                    new Opcion() { Texto = "Bicicletas.", EsCorrecta = false, idPregunta = 10 },
                    new Opcion() { Texto = "Carretas.", EsCorrecta = false, idPregunta = 10 },
                    new Opcion() { Texto = "Vehículos eléctricos.", EsCorrecta = false, idPregunta = 10 },

                    new Opcion() { Texto = "Vías peatonales, primarias, sobre o debajo de cualquier puente.", EsCorrecta = true, idPregunta = 11 },
                    new Opcion() { Texto = "Afuera de casas, escuelas y hospitales.", EsCorrecta = false, idPregunta = 11 },
                    new Opcion() { Texto = "En espacios reservados.", EsCorrecta = false, idPregunta = 11 },
                    new Opcion() { Texto = "En un bosque, parque o zona natural.", EsCorrecta = false, idPregunta = 11 },

                    new Opcion() { Texto = "Sí", EsCorrecta = true, idPregunta = 12 },
                    new Opcion() { Texto = "No.", EsCorrecta = false, idPregunta = 12 },

                    new Opcion() { Texto = "En la parte trasera asegurándose que estos ocupen una de las plazas usando un cinturón de 3 puntos y que estén detrás de los asientos del conductor o del copiloto.", EsCorrecta = true, idPregunta = 13 },
                    new Opcion() { Texto = "De copilotos.", EsCorrecta = false, idPregunta = 13 },
                    new Opcion() { Texto = "En la parte trasera en medio para mejor visión.", EsCorrecta = false, idPregunta = 13 },
                    new Opcion() { Texto = "En la cajuela.", EsCorrecta = false, idPregunta = 13 },

                    new Opcion() { Texto = "Placas, calcomanía, holograma y tarjeta de circulación.", EsCorrecta = true, idPregunta = 14 },
                    new Opcion() { Texto = "Llantas, ventanas y puertas.", EsCorrecta = false, idPregunta = 14 },
                    new Opcion() { Texto = "Placas, luces, y rines.", EsCorrecta = false, idPregunta = 14 },
                    new Opcion() { Texto = "Holograma, tarjeta de circunvalación y lentes para sol.", EsCorrecta = false, idPregunta = 14 },

                    new Opcion() { Texto = "Artículo 47.", EsCorrecta = true, idPregunta = 15 },
                    new Opcion() { Texto = "Artículo 7.", EsCorrecta = false, idPregunta = 15 },
                    new Opcion() { Texto = "Artículo 15.", EsCorrecta = false, idPregunta = 15 },
                    new Opcion() { Texto = "Artículo 27.", EsCorrecta = false, idPregunta = 15 },

                    new Opcion() { Texto = "Emergencia, tecnología sustentable, transporte escolar, servicios funerarios, manifestación de emergencia médica y los que apliquen disposiciones jurídicas y administrativas aplicables.", EsCorrecta = true, idPregunta = 16 },
                    new Opcion() { Texto = "Tuneados, antiguos y de servicios privados.", EsCorrecta = false, idPregunta = 16 },
                    new Opcion() { Texto = "De carga, motocicletas y todoterrenos.", EsCorrecta = false, idPregunta = 16 },
                    new Opcion() { Texto = "Automáticos y de tecnología sustentable.", EsCorrecta = false, idPregunta = 16 },

                    new Opcion() { Texto = "Alcoholímetro", EsCorrecta = true, idPregunta = 17 },
                    new Opcion() { Texto = "Señalamiento de tránsito.", EsCorrecta = false, idPregunta = 17 },
                    new Opcion() { Texto = "Posicionamiento de menores de edad.", EsCorrecta = false, idPregunta = 17 },
                    new Opcion() { Texto = "Lugares para estacionarse.", EsCorrecta = false, idPregunta = 17 },

                    new Opcion() { Texto = "Sangre: 0.8 gramos, Aire: 0.4 grm", EsCorrecta = true, idPregunta = 18 },
                    new Opcion() { Texto = "Sangre: 1 gramo, Aire: 0.1 grm", EsCorrecta = false, idPregunta = 18 },
                    new Opcion() { Texto = "Sangre: 1.6 gramos, Aire: 0.4 grm", EsCorrecta = false, idPregunta = 18 },
                    new Opcion() { Texto = "Sangre: 0.8 gramo, Aire: 0.1 grm", EsCorrecta = false, idPregunta = 18 },

                    new Opcion() { Texto = "Puntos de revisión establecidos, cualquier vía.", EsCorrecta = true, idPregunta = 19 },
                    new Opcion() { Texto = "Carretera, estacionados.", EsCorrecta = false, idPregunta = 19 },
                    new Opcion() { Texto = "Afuera de bares.", EsCorrecta = false, idPregunta = 19 },
                    new Opcion() { Texto = "En avenidas principales y secundarias.", EsCorrecta = false, idPregunta = 19 },

                    new Opcion() { Texto = "Detenerse en el lugar del incidente, encender luces intermitentes, llamar a la aseguradora.", EsCorrecta = true, idPregunta = 20 },
                    new Opcion() { Texto = "Dejar el lugar lo más rápido posible.", EsCorrecta = false, idPregunta = 20 },
                    new Opcion() { Texto = "Entrar en pánico y llamar a una autoridad.", EsCorrecta = false, idPregunta = 20 },
                    new Opcion() { Texto = "Llamar a un abogado y la aseguradora.", EsCorrecta = false, idPregunta = 20 },

                    new Opcion() { Texto = "Permanecer en el lugar del incidente, colocar luces intermitentes, no mover el cuerpo hasta que una autoridad lo autorice.", EsCorrecta = true, idPregunta = 21 },
                    new Opcion() { Texto = "Dejar el lugar lo más rápido posible.", EsCorrecta = false, idPregunta = 21 },
                    new Opcion() { Texto = "Grabar la escena y preguntar por ayuda.", EsCorrecta = false, idPregunta = 21 },
                    new Opcion() { Texto = "Iniciar una discusión para ignorar los hechos.", EsCorrecta = false, idPregunta = 21 }
                };

            foreach (var u in insetarpregunta)
                _contextDB.Preguntas.Add(u);
            _contextDB.SaveChanges();

            foreach (var u in insertarusuarios)
                _contextDB.Usuario.Add(u);

            foreach (var u in insertarcategorias)
                _contextDB.Categoria.Add(u);

            foreach (var u in insertararticulo)
                _contextDB.Articulo.Add(u);

            foreach (var u in insertarcomentario)
                _contextDB.Comentario.Add(u);

            foreach (var u in insertaropciones)
                _contextDB.Opciones.Add(u);

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
