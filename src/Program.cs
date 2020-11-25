using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Splash.Generator
{
    class Program
    {
        private const string SPLASH_IMG = "splash.img";
        private const string SPLASH_NEW_IMG = "splash_new.img";

        static async Task Main(string[] args)
        {
            //Console.WriteLine("Splash Generator");
            // 换了个更大的Logo
            Console.WriteLine("=================================================");
            Console.WriteLine(" ╔═╗┌─┐┬  ┌─┐┌─┐┬ ┬  ╔═╗┌─┐┌┐┌┌─┐┬─┐┌─┐┌┬┐┌─┐┬─┐ ");
            Console.WriteLine(" ╚═╗├─┘│  ├─┤└─┐├─┤  ║ ╦├┤ │││├┤ ├┬┘├─┤ │ │ │├┬┘ ");
            Console.WriteLine(" ╚═╝┴  ┴─┘┴ ┴└─┘┴ ┴  ╚═╝└─┘┘└┘└─┘┴└─┴ ┴ ┴ └─┘┴└─ ");
            Console.WriteLine("=================================================");
            Console.WriteLine(" Version 1.0.0 ");
            Console.WriteLine(" Coder by frg2089.");
            Console.WriteLine(" Open Source Under MIT License.");
            Console.WriteLine("=================================================");

            var dir = new DirectoryInfo(// 获取工作目录
                args.Length > 0
                    ? args[0]
                    : Environment.CurrentDirectory
            );
            
            var baseFile = new FileInfo(Path.Combine(Environment.CurrentDirectory, SPLASH_IMG));
            var newFile = new FileInfo(Path.Combine(Environment.CurrentDirectory, SPLASH_NEW_IMG));
            if(!baseFile.Exists){// 找不到splash.img文件
                Console.Error.WriteLine("ERROR: Cannot find File \"{0}\"", SPLASH_IMG);
                return;// 结束程序
            }
            if(newFile.Exists) newFile.Delete();// 删除已存在文件
            baseFile.CopyTo(SPLASH_NEW_IMG);// 拷贝文件

            var files = dir.GetFiles("*.bmp");
            var enters = new Dictionary<int,FileInfo>();
            foreach (var file in files)
            {
                try
                {
                    var offset = Convert.ToInt32(file.Name.Replace(file.Extension, string.Empty), 16);
                    Console.WriteLine("Find File:{0}", file.Name.PadLeft(12));
                    enters.Add(offset, file);
                }catch(FormatException)// 无法转换为整数
                {
                    continue;// 跳过这个文件
                }
            }
            Console.WriteLine();
            Console.WriteLine("Working...");
            await using var stream = newFile.OpenWrite();
            foreach (var enter in enters)
            {
                Console.WriteLine("Write File:{1} to Offset:0x{0}", enter.Key.ToString("x").PadLeft(8, '0'), enter.Value.Name.PadLeft(12).PadRight(14));
                stream.Seek(enter.Key, SeekOrigin.Begin);
                await using var imgStream = enter.Value.OpenRead();
                await imgStream.CopyToAsync(stream);
            }
        }
    }
}
