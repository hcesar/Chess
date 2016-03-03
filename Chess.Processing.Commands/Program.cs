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
            string defaultArgs = null;
            if (args.Length == 0)
            {
                //defaultArgs = "fixations ChessFiles\\2.volke -f image -eye right";
                //defaultArgs = @"aoi ChessFiles\2.volke -lpf";
                //defaultArgs = @"scanpath ChessFiles\33.volke -lpf -f image";
                //defaultArgs = @"heatmaps -i ChessFiles\2.volke,ChessFiles\3.volke,ChessFiles\4.volke,ChessFiles\5.volke,ChessFiles\10.volke -lpf -t 19 -w";
                //defaultArgs = @"heatmaps -i ChessFiles\19.volke,ChessFiles\15.volke,ChessFiles\11.volke,ChessFiles\18.volke,ChessFiles\33.volke -lpf -t 19 -w";
                //defaultArgs = "summary participants.xml result.csv -tpath .\\chessfiles";

                //defaultArgs = "summary participants.xml -d -xp 7,8,14,17,28,31 -tpath chessfiles";
                defaultArgs = "heatmap chessfiles -p 2,3,4,5,10 -w -lpf -t 1 -r 80 -sigma 4,5 -saliency";
            }
           
            if (defaultArgs != null)
                args = defaultArgs.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

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

            Console.WriteLine("Usage\r\n{0} <command>\r\n\r\nCommand List:\r\n", executable);
            foreach (var cmd in commands.OrderBy(i => i.Attribute.Command))
            {
                Console.WriteLine("  {0}{1}{2}", cmd.Attribute.Command, new string(' ', 15 - cmd.Attribute.Command.Length), cmd.Attribute.Description);
            }

            Console.WriteLine();
        }
    }
}
