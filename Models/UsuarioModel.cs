﻿using Insane_Mechanical.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Threading.RateLimiting;

namespace Insane_Mechanical.Models
{
    public class UsuarioModel
    {
        public readonly Insane_MechanicalDB _contextDB;

        public UsuarioModel(Insane_MechanicalDB contextDB)
        {
            _contextDB = contextDB;
        }

        public string Correo { get; set; }
        public string Correo2 { get; set; }
        public static string Telefono { get; set; }
        public static string Telefono2 { get; set; }
        public string Contrasena { get; set; }
        public string Contrasena2 { get; set; }
        public string Genero { get; set; }
        public static string Mensaje { get; set; }
        public static string Nombre { get; set; }
        public static string TipoUsuario { get; set; }
        public static string DireccionImagen { get; set; }

        public bool Login()
        {
            List<Usuario> ListaUsuarios = _contextDB.Usuario.ToList();

            foreach (var user in ListaUsuarios)
            {
                if (Correo == user.Correo)
                {
                    if (Contrasena == user.Contrasena)
                    {
                        TipoUsuario = user.TipoUsuario;

                        DireccionImagen = user.DireccionImagen;

                        Nombre = user.Nombre;

                        return true;
                    }
                }
            }
            Mensaje = "Correo y/o Contrasena incorrecta";

            return false;
        }

        public bool Register()
        {
            if (Correo2 == Correo)
            {
                if (Telefono2 == Telefono)
                {
                    if (Contrasena == Contrasena2)
                    {
                        string mensaje = "";
                        int i = 0;

                        List<Usuario> ListaUsuarios = _contextDB.Usuario.ToList();

                        foreach (var user in ListaUsuarios)
                        {
                            if (user.Correo == Correo)
                            {
                                mensaje = "Correo";
                                break;
                            }
                            else if (user.Telefono == Telefono)
                            {
                                mensaje = "Telefono";
                                break;
                            }
                            else
                                i++;
                        }
                        if (i == ListaUsuarios.Count)
                        {
                            var u = new Usuario[]
                                {
                            new Usuario() {Correo = Correo, Contrasena = Contrasena, TipoUsuario = "Cliente", Nombre = Nombre, DireccionImagen = "../Images/Usuarios/Usuario.png", Telefono = Telefono, Genero = Genero}
                                };

                            foreach (var us in u)
                            {
                                _contextDB.Usuario.Add(us);
                            }
                            _contextDB.SaveChanges();

                            return true;
                        }
                        else
                        {
                            Mensaje = $"El {mensaje} ya esta registrado";
                            return false;
                        }
                    }
                    else
                    {
                        Mensaje = "La contraseña no coincide";
                        return false;
                    }
                }
                else
                {
                    Mensaje = "El telefono no coincide";
                    return false;
                }
            }
            else
            {
                Mensaje = "El Correo no coincide";
                return false;
            }
        }

        public void EditarUsuario(Usuario usuario)
        {
            _contextDB.Usuario.Update(usuario);
            _contextDB.SaveChanges();
        }
    }
}