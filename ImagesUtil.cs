using System;
using System.IO;
using System.Security.Permissions;
using SkiaSharp;

namespace SEP3_Tier3
{
    public class ImagesUtil
    {
        public const string FILE_PATH = "C:/Users/Przemo/RiderProjects/Images";
        
        public static byte[] ResizeImage(byte[] initialImage, int width, int height)
        {
            SKBitmap source = SKBitmap.Decode(initialImage);
            using SKBitmap scaledBitmap = source.Resize(new SKImageInfo(width, height), SKFilterQuality.Medium);
            using SKImage scaledImage = SKImage.FromBitmap(scaledBitmap);
            using SKData data = scaledImage.Encode();
            return data.ToArray();
        }

        public static void WriteImageToPath(byte[] image, string path, string fileName)
        {
            using (var ms = new MemoryStream(image))
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Console.WriteLine("New directory:" + Directory.GetCreationTime(path));
                }
                using (var fs = new FileStream(path + fileName, FileMode.Create))
                {
                    
                    FileIOPermission f = new FileIOPermission(PermissionState.None);
                    f.AllLocalFiles = FileIOPermissionAccess.Write;
                    f.Demand();

                    Console.WriteLine("*************************Before writing " + path);
                    
                    ms.WriteTo(fs);
                    Console.WriteLine("******************WROTE TO PATH " + path);
                }
            }
        }
    }
}