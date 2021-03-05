using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BootScreenUtils
{
    public static class SplashUtils
    {
        public static async Task UnpackBitmapFromImage(string image, string workFolder, int bufferSize)
        {
            Console.WriteLine($"Splash Image: {Path.GetFullPath(image)}");
            Console.WriteLine($"Output Folder: {Path.GetFullPath(workFolder)}");
            Console.WriteLine($"Buffer Size: {bufferSize}");
            await using FileStream stream = new(image, FileMode.Open);
            Memory<byte> buffer = new byte[bufferSize];
            
            Console.WriteLine($"Parsing...");
            while (stream.Position < stream.Length - 54)
            {
                // 记录当前位置
                var offset = stream.Position;
                // 读取两字节
                await stream.ReadAsync(buffer[..2]).ConfigureAwait(false);
                // 判断是否不是Bitmap文件头
                if (!"BM".Equals(Encoding.ASCII.GetString(buffer[..2].Span)))
                    // 这不是Bitmap文件
                    continue;

                // 获取文件大小
                await stream.ReadAsync(buffer[..4]).ConfigureAwait(false);
                int fileSize;
                if (!BitConverter.IsLittleEndian)
                {
                    var tmp = buffer[..4].ToArray();
                    Array.Reverse(tmp);
                    fileSize = BitConverter.ToInt32(tmp);
                }
                fileSize = BitConverter.ToInt32(buffer[..4].Span);

                Console.WriteLine($"Find Bitmap File : Offset : 0x{offset:X8} Size : \t{fileSize}");

                if (!Directory.Exists(workFolder))
                    Directory.CreateDirectory(workFolder);
                // 复制到文件
                stream.Seek(offset, SeekOrigin.Begin);
                await stream.ReadAsync(buffer[..fileSize]).ConfigureAwait(false);
                await using FileStream fs = new(Path.Combine(workFolder, $"0x{offset:X}.bmp"), FileMode.Create);
                await fs.WriteAsync(buffer[..fileSize]).ConfigureAwait(false);
            }
            Console.WriteLine("All Done!");
        }

        public static async Task PatchImage(string image, string workFolder, string newImage)
        {
            if (string.IsNullOrEmpty(newImage))
                newImage = Path.GetFileNameWithoutExtension(image) + ".new.img";

            Console.WriteLine($"Splash Image: {Path.GetFullPath(image)}");
            Console.WriteLine($"Output Folder: {Path.GetFullPath(workFolder)}");
            Console.WriteLine($"New Splash Image: {Path.GetFullPath(newImage)}");

            var baseFile = new FileInfo(image);
            var newFile = new FileInfo(newImage);

            if (newFile.Exists)
                newFile.Delete();// 删除已存在文件
            baseFile.CopyTo(newImage);// 拷贝文件

            var files = new DirectoryInfo(workFolder).GetFiles("*.bmp");
            var enters = new Dictionary<int, FileInfo>();
            foreach (var file in files)
            {
                try
                {
                    var offset = Convert.ToInt32(file.Name.Replace(file.Extension, string.Empty), 16);
                    Console.WriteLine("Find File:{0}", file.Name.PadLeft(24));
                    enters.Add(offset, file);
                }
                catch (FormatException)// 无法转换为整数
                {
                    continue;// 跳过这个文件
                }
            }

            Console.WriteLine("Patching...");
            await using var stream = newFile.OpenWrite();
            foreach (var enter in enters)
            {
                Console.WriteLine($"Patch File at Offset : 0x{enter.Key:X8}");
                stream.Seek(enter.Key, SeekOrigin.Begin);
                await using var imgStream = enter.Value.OpenRead();
                await imgStream.CopyToAsync(stream);
            }
        }
    }
}
