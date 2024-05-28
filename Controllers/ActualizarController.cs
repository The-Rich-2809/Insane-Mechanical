using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Insane_Mechanical.Models;

namespace Insane_Mechanical.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActualizarController : ControllerBase
    {
        private readonly Insane_MechanicalDB _contextDB;
        public ActualizarController(ILogger<ArticuloController> logger, Insane_MechanicalDB contextDB)
        {
            _contextDB = contextDB;
        }

        public static int IdUser {  get; set; }

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
                        IdUser = user.ID;
                    }
                }
            }
        }

        [HttpGet("Comentario")]
        public ActionResult<List<Comentarios>> Comentario(string parametro, int i)
        {
            Cookies();
            var insertarcomentario = new Comentario[]
            {
                new Comentario(){IdArticulo = i, IdUsuario = IdUser, Texto = parametro }
            };

            foreach (var u in insertarcomentario)
                _contextDB.Comentario.Add(u);

            _contextDB.SaveChanges();

            List<Comentario> comentario = _contextDB.Comentario.ToList();

            List<Usuario> usuarios = _contextDB.Usuario.ToList();

            var lista = new List<Comentarios>();

            foreach(var u in comentario)
            {
                if(u.IdArticulo == i)
                {
                    foreach (var usuario in usuarios)
                    {
                        if(u.IdUsuario == usuario.ID)
                        {
                            lista.Add(new Comentarios() {Mensaje = u.Texto, Foto = usuario.DireccionImagen, Nombre = usuario.Nombre });
                            break;
                        }
                    }
                }
            }

            
            return lista;
        }
    }
    public class Comentarios
    {
        public string Mensaje { get; set; }
        public string Nombre { get; set; }
        public string Foto { get; set; }
    }
}
