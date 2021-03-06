﻿using System.Drawing;
using System.IO;

namespace Flummery.Core.ContentPipeline
{
    public class JPGImporter : ContentImporter
    {
        public override string GetExtension() { return "jpg"; }

        public override Asset Import(string path)
        {
            Texture texture = new Texture();

            texture.FileName = path;

            FileInfo fi = new FileInfo(path);
            using (Bitmap bitmap = new Bitmap(path)) { texture.CreateFromBitmap(bitmap, fi.Name.Replace(fi.Extension, "")); }

            return texture;
        }
    }
}
