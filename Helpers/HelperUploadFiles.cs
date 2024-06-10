using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insane_Mechanical.Providers;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Insane_Mechanical.Helpers
{
    public class HelperUploadFiles
    {
        private PathProvider pathProvider;

        public HelperUploadFiles(PathProvider pathProvider)
        {
            this.pathProvider = pathProvider;
        }

        public async Task<string> UploadFilesAsync(IFormFile formFile, string nombreImagen, Folders folder)
        {
            string path = this.pathProvider.MapPath(nombreImagen, folder);

            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            return path;
        }

        public async Task<string> UploadHtmlAsync(Stream htmlStream, string fileName, Folders folder)
        {
            string path = this.pathProvider.MapPath(fileName, folder);

            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                await htmlStream.CopyToAsync(stream);
            }

            return path;
        }
    }
}
