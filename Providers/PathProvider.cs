using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Ultragamma.Providers
{
    public enum Folders
    {
        HTML = 0, Articulos = 1
    }

    public class PathProvider
    {
        private IWebHostEnvironment hostEnvironment;

        public PathProvider(IWebHostEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment;
        }

        public string MapPath(string fileName, Folders folder)
        {
            string path = "";
            if(folder == Folders.HTML)
            {
                string carpeta = "HTML/";
                path = Path.Combine(this.hostEnvironment.WebRootPath, carpeta, fileName);
            }
            if (folder == Folders.Articulos)
            {
                string carpeta = "Articulos/";
                path = Path.Combine(this.hostEnvironment.WebRootPath, carpeta, fileName);
            }

            return path;
        }
    }
}
