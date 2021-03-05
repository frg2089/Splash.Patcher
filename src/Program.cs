using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BootScreenUtils
{
    static class Program
    {
        const string COMMAND_UNPACK = "unpack";
        const string COMMAND_PATCH = "patch";
        //const string COMMAND_MAKE = "make";

        static async Task Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.PrintHelp();
                return;
            }
            Console.Out.PrintLogo();
            switch (args[0].ToLower())
            {
                case COMMAND_UNPACK:
                    if (args.Length > 2)
                    {
                        int bufferSize = args.Length > 3 && int.TryParse(args[3], out var tmp) ? tmp : 1024 * 1024 * 8;

                        await SplashUtils.UnpackBitmapFromImage(args[1], args[2], bufferSize).ConfigureAwait(false);
                    }
                    else
                    {
                        Console.Error.WriteLine("Argument Error");
                        Helper.PrintUnpack(Console.Error);
                    }
                    break;
                case COMMAND_PATCH:
                    if (args.Length > 2)
                    {
                        await SplashUtils.PatchImage(args[1], args[2], args.Length > 3 ? args[3] : default).ConfigureAwait(false);
                    }
                    else
                    {
                        Console.Error.WriteLine("Argument Error");
                        Helper.PrintPatch(Console.Error);
                    }
                    break;
                // case COMMAND_MAKE:
                //     if (args.Length > 3)
                //     {
                //         PackageUtils.MakePackage(args[1], args[2], args[3]);
                //     }
                //     else
                //     {
                //         Helper.PrintPatch(Console.Error);
                //     }
                //     break;
                default:
                    Console.Error.WriteLine("Command Error");
                    Console.Error.PrintHelp();
                    break;
            }
        }
        static void PrintLogo(this TextWriter textWriter)
        {
            // Logo要大
            textWriter.WriteLine("<===================================================>");
            textWriter.WriteLine();
            textWriter.WriteLine(" ██████╗  ██████╗  ██████╗ ████████╗");
            textWriter.WriteLine(" ██╔══██╗██╔═══██╗██╔═══██╗╚══██╔══╝");
            textWriter.WriteLine(" ██████╔╝██║   ██║██║   ██║   ██║");
            textWriter.WriteLine(" ██╔══██╗██║   ██║██║   ██║   ██║");
            textWriter.WriteLine(" ██████╔╝╚██████╔╝╚██████╔╝   ██║");
            textWriter.WriteLine(" ╚═════╝  ╚═════╝  ╚═════╝    ╚═╝");
            textWriter.WriteLine();
            textWriter.WriteLine(" ███████╗ ██████╗██████╗ ███████╗███████╗███╗   ██╗");
            textWriter.WriteLine(" ██╔════╝██╔════╝██╔══██╗██╔════╝██╔════╝████╗  ██║");
            textWriter.WriteLine(" ███████╗██║     ██████╔╝█████╗  █████╗  ██╔██╗ ██║");
            textWriter.WriteLine(" ╚════██║██║     ██╔══██╗██╔══╝  ██╔══╝  ██║╚██╗██║");
            textWriter.WriteLine(" ███████║╚██████╗██║  ██║███████╗███████╗██║ ╚████║");
            textWriter.WriteLine(" ╚══════╝ ╚═════╝╚═╝  ╚═╝╚══════╝╚══════╝╚═╝  ╚═══╝");
            textWriter.WriteLine();
            textWriter.WriteLine(" ██╗   ██╗████████╗██╗██╗     ███████╗");
            textWriter.WriteLine(" ██║   ██║╚══██╔══╝██║██║     ██╔════╝");
            textWriter.WriteLine(" ██║   ██║   ██║   ██║██║     ███████╗");
            textWriter.WriteLine(" ██║   ██║   ██║   ██║██║     ╚════██║");
            textWriter.WriteLine(" ╚██████╔╝   ██║   ██║███████╗███████║");
            textWriter.WriteLine("  ╚═════╝    ╚═╝   ╚═╝╚══════╝╚══════╝");
            textWriter.WriteLine();
            textWriter.WriteLine("<===================================================>");
            textWriter.WriteLine("  Version 1.0.1");
            textWriter.WriteLine("  Copyright (C) 2021 frg2089.");
            textWriter.WriteLine("  Open Source Under MIT License.");
            textWriter.WriteLine("<===================================================>");
        }

        static void PrintHelp(this TextWriter textWriter)
        {
            Helper.PrintUnpack(textWriter);
            Helper.PrintPatch(textWriter);
            //Helper.PrintMake(textWriter);
        }
        static class Helper
        {
            public static void PrintUnpack(TextWriter textWriter)
            {
                textWriter.WriteLine();
                textWriter.WriteLine($"    {COMMAND_UNPACK,-32} : Get All Bitmaps from splash.img");
                textWriter.WriteLine($"    {"       <splash.img>",-32} : splash.img file path");
                textWriter.WriteLine($"    {"       <out bitmaps folder>",-32} : Output Folder Path");
                textWriter.WriteLine($"    {"       [<buffer size>]",-32} : Buffer Size (default = 268,435,456)");
            }
            public static void PrintPatch(TextWriter textWriter)
            {
                textWriter.WriteLine();
                textWriter.WriteLine($"    {COMMAND_PATCH,-32} : Patch All Bitmaps to splash.img");
                textWriter.WriteLine($"    {"      <splash.img>",-32} : Source splash.img path");
                textWriter.WriteLine($"    {"      <bitmaps folder>",-32} : Bitmaps folder");
                textWriter.WriteLine($"    {"      [<new splash.img>] ",-32} : New splash.img path (default = <splash>.new.img)");
            }
            // public static void PrintMake(TextWriter textWriter)
            // {
            //     textWriter.WriteLine();
            //     textWriter.WriteLine($"    {COMMAND_MAKE,-32} ");
            //     textWriter.WriteLine($"    {"      <splash.img>",-32} ");
            //     textWriter.WriteLine($"    {"      <animation.zip>",-32} ");
            //     textWriter.WriteLine($"    {"      <output.zip> ",-32} ");
            // }
        }
    }
}
