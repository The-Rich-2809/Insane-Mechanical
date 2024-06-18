using Insane_Mechanical.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace Insane_Mechanical.Controllers
{
    public class CuestionarioController : Controller
    {
        private readonly Insane_MechanicalDB _contextDB;
        public int IDUsuario { get; set; }
        public string Correo { get; set; }

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
                        IDUsuario = user.ID;
                        Correo = user.Correo;
                        ViewBag.Nombre = user.Nombre;
                        ViewBag.Nivel = user.TipoUsuario;
                        ViewBag.FotoPerfil = user.DireccionImagen;
                    }
                }
            }
            List<Categoria> categorias = _contextDB.Categoria.ToList();
            ViewBag.Categorias = categorias;
        }

        public CuestionarioController(Insane_MechanicalDB insane_MechanicalDB) {
            _contextDB = insane_MechanicalDB;
        }

        [HttpGet]
        public async Task<IActionResult> Llenar(bool repetir)
        {
            Cookies();
            var usuarioActual = IDUsuario;

            var yaContesto = await _contextDB.RespuestaUsuario
                .AnyAsync(r => r.idUsuario == usuarioActual);

            if (repetir == true)
            {
                var pregunta = _contextDB.Preguntas.Include(p => p.Opciones).ToList();
                return View(pregunta);
            }

            if (yaContesto)
            {
                ViewBag.YaContesto = true;
                return View();
            }

            var preguntas = _contextDB.Preguntas.Include(p => p.Opciones).ToList();

            if (preguntas.Count == 0)
                ViewBag.Conteo = 0;

            return View(preguntas);
        }


        [HttpPost]
        public async Task<IActionResult> Llenar(List<RespuestasUsuario> respuestas)
        {
            Cookies();

            var usuarioActual = IDUsuario;

            var respuestasIncorrectas = new List<int>();
            var respuestasCorrectas = new List<int>();

            foreach (var respuesta in respuestas)
            {
                var opcionCorrecta = await _contextDB.Opciones
                    .FirstOrDefaultAsync(o => o.ID == respuesta.idOpcion && o.EsCorrecta);

                if (opcionCorrecta == null)
                {
                    var res = await _contextDB.Opciones
                        .FirstOrDefaultAsync(o => o.ID == respuesta.idOpcion);
                    respuestasIncorrectas.Add(respuesta.idPregunta);
                    _contextDB.RespuestaUsuario.Add(new RespuestasUsuario
                    {
                        idPregunta = respuesta.idPregunta,
                        idOpcion = respuesta.idOpcion,
                        idUsuario = usuarioActual,
                        Respuesta = res.Texto
                    });
                    await _contextDB.SaveChangesAsync();
                }
                else
                {
                    var res = await _contextDB.Opciones
                        .FirstOrDefaultAsync(o => o.ID == respuesta.idOpcion);

                    respuestasCorrectas.Add(respuesta.idPregunta);
                    _contextDB.RespuestaUsuario.Add(new RespuestasUsuario
                    {
                        idPregunta = respuesta.idPregunta,
                        idOpcion = respuesta.idOpcion,
                        idUsuario = usuarioActual,
                        Respuesta = res.Texto
                    });
                    await _contextDB.SaveChangesAsync();
                }
            }

            if (respuestasIncorrectas.Any())
            {
                var emailBody = "Tus respuestas:<br><br>";
                foreach (var respuesta in respuestas)
                {
                    var pregunta = await _contextDB.Preguntas
                        .Include(p => p.Opciones)
                        .FirstOrDefaultAsync(p => p.ID == respuesta.idPregunta);

                    var opcionSeleccionada = pregunta.Opciones.FirstOrDefault(o => o.ID == respuesta.idOpcion);

                    emailBody += $"<strong>{pregunta.Nombre}</strong><br>";
                    foreach (var opcion in pregunta.Opciones)
                    {
                        emailBody += $"{(opcion.ID == respuesta.idOpcion ? (opcion.EsCorrecta ? "<b style='color:green;'>" : "<b style='color:red;'>") : "")}{opcion.Texto}{(opcion.ID == respuesta.idOpcion ? "</b>" : "")}<br>";
                    }
                    emailBody += "<br>";
                }

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("centrokarpi@gmail.com", "aawjbzinzxsseuma"),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("centrokarpi@gmail.com"),
                    Subject = "Resultados del Cuestionario",
                    Body = emailBody,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add($"{Correo}");

                smtpClient.Send(mailMessage);
            }

            return RedirectToAction("Gracias");
        }

        public IActionResult Gracias()
        {
            Cookies();
            return View();
        }
    }
}
