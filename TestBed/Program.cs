using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sm4shTourneyLib;
using Sm4shTourneyLib.CharacterList;

namespace TestBed
{
    class Program
    {
        static readonly string imageDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "images");

        static void Main(string[] args)
        {
            Console.WriteLine("Getting Characters...\n");
            CharacterUpdater updater = new CharacterUpdater();
            List<Character> characters = updater.GetOrderedCharacters();

            if (!Directory.Exists(imageDirectory))
                Directory.CreateDirectory(imageDirectory);
            WebClient client = new WebClient();
            int counter = 1;
            foreach(Character c in characters)
            {
                Console.WriteLine(counter + ": " + c.Name);

                Console.Write("image... ");
                try
                {
                    client.DownloadFile(c.ImageLocation, Path.Combine(imageDirectory, c.GetImageFileName()));
                    WriteWithColor("DONE!", ConsoleColor.Green);
                }
                catch (Exception e)
                {
                    WriteWithColor("FAIL!", ConsoleColor.Red);
                    WriteWithColor(e.ToString(), ConsoleColor.Red);
                }
                Console.WriteLine(c.GetImageFileName());

                Console.WriteLine();
                counter++;
            }

        }

        private static void WriteWithColor(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
