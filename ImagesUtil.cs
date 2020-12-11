using System;
using System.IO;
using System.Security.Permissions;
using SkiaSharp;

namespace SEP3_Tier3
{
    /// <summary>
    /// A class with static methods related to working with images
    /// </summary>
    public class ImagesUtil
    {
        public const string FILE_PATH = "C:/Users/Toni/RiderProjects/Images";

        /// <summary>
        /// Resizes a given image with the given dimensions
        /// </summary>
        /// <param name="initialImage">the initial image</param>
        /// <param name="width">the new width</param>
        /// <param name="height">the new height</param>
        /// <returns>the resized version of the image</returns>
        public static byte[] ResizeImage(byte[] initialImage, int width, int height)
        {
            SKBitmap source = SKBitmap.Decode(initialImage);
            using SKBitmap scaledBitmap = source.Resize(new SKImageInfo(width, height), SKFilterQuality.Medium);
            using SKImage scaledImage = SKImage.FromBitmap(scaledBitmap);
            using SKData data = scaledImage.Encode();
            return data.ToArray();
        }

        /// <summary>
        /// Writes a given image to a given file
        /// </summary>
        /// <param name="image">the given image</param>
        /// <param name="path">the file path</param>
        /// <param name="fileName">the file name</param>
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
                    ms.WriteTo(fs);
                }
            }
        }

        /// <summary>
        /// Deletes a given folder
        /// </summary>
        /// <param name="path">the file path</param>
        public static void DeleteUserFolder(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            foreach (var file in directoryInfo.GetFiles())
            {
                file.Delete();
                Console.WriteLine("Deleting file " + file.Name);
            }

            if (Directory.Exists(path))
            {
                Directory.Delete(path);
                Console.WriteLine("Deleted folder " + path);
            }
        }

        /// <summary>
        /// Deletes a given file
        /// </summary>
        /// <param name="path">the file path</param>
        /// <param name="fileName">the file name</param>
        public static void DeleteFile(string path, string fileName)
        {
            try {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                foreach (var file in directoryInfo.GetFiles())
                {
                    Console.WriteLine("File name is " + file.Name);
                    if (file.Name.Equals(fileName))
                    {
                        file.Delete();
                        Console.WriteLine("Deleting file " + file.Name);
                    }
                    
                }
            }
            catch (Exception e) {
                Console.WriteLine("Could not delete file with path " + path);
            }
        }
    }
}