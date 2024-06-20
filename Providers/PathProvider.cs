using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Insane_Mechanical.Providers
{
    public enum Folders
    {
        HTML = 0, Articulos = 1, Documents = 2, Temp = 3, Images = 4
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
                string carpeta = "Views/HTML/";
                path = Path.Combine(this.hostEnvironment.ContentRootPath, carpeta, fileName);
            }
            if (folder == Folders.Articulos)
            {
                string carpeta = "Images/Articulos/";
                path = Path.Combine(this.hostEnvironment.WebRootPath, carpeta, fileName);
            }

            return path;
        }
    }
}
