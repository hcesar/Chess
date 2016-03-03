using Chess.Processing.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Processing
{
    class Program
    {
        static void Main(string[] args)
        {
            args = new[] { "eyepositions", "ChessFiles\\2.volke" };

            if (!Directory.Exists(ProcessingSettings.OutputDirectory))
                Directory.CreateDirectory(ProcessingSettings.OutputDirectory);

            var commands = Command.LoadAll();
            if (args.Length == 0)
            {
                ShowUsage();
                return;
            }

            var cmd = commands.FirstOrDefault(i => args[0].Equals(i.Attribute.Command, StringComparison.InvariantCultureIgnoreCase));
            if (cmd == null)
            {
                Console.WriteLine("Command not found.\r\n");
                ShowUsage();
                return;
            }

            cmd.Execute(args.Skip(1).ToList());
        }

        private static void ShowUsage()
        {
            string executable = Path.GetFileName(typeof(Program).Assembly.Location).ToLower();
            var commands = Command.LoadAll();

            Console.WriteLine("Usage\r\n{0} [command]\r\n\r\nCommand List:\r\n\r\n", executable);
            foreach (var cmd in commands)
            {
                Console.WriteLine("   {0}", cmd.Attribute.Title);
                Console.WriteLine("   {0}\r\n", cmd.Attribute.Description);
                Console.WriteLine("   Usage:");
                Console.WriteLine("   {0}", cmd.GetUsageMessage(executable));
            }

            Console.WriteLine();
        }
    }
}
