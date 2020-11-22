using System.IO;
using SkiaSharp;

namespace SEP3_Tier3
{
    public class ImagesUtil
    {
        public const string FILE_PATH = "C:/Users/Toni/RiderProjects/Images";
        
        public static byte[] ResizeImage(byte[] initialImage, int width, int height)
        {
            SKBitmap source = SKBitmap.Decode(initialImage);
            using SKBitmap scaledBitmap = source.Resize(new SKImageInfo(width, height), SKFilterQuality.Medium);
            using SKImage scaledImage = SKImage.FromBitmap(scaledBitmap);
            using SKData data = scaledImage.Encode();
            return data.ToArray();
        }

        public static void WriteImageToPath(byte[] image, string path)
        {
            using (var ms = new MemoryStream(image))
            {
                using (var fs = new FileStream(path, FileMode.Create))
                {
                    // FileIOPermission f = new FileIOPermission(PermissionState.None);
                    // f.AllLocalFiles = FileIOPermissionAccess.Write;
                    // f.Demand();
                    
                    ms.WriteTo(fs);
                }
            }
        }
    }
}