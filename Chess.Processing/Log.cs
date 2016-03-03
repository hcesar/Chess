using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Processing
{
    class Log
    {
        public static bool IsVerbose { get; set; }

        public static void Information(string format, params object[] parameters)
        {
            Write(ConsoleColor.Gray, format, parameters);
        }
        public static void Warning(string format, params object[] parameters)
        {
            Write(ConsoleColor.Yellow, format, parameters);
        }
        public static void Error(string format, params object[] parameters)
        {
            Write(ConsoleColor.Red, format, parameters);
        }
        public static void Verbose(string format, params object[] parameters)
        {
            if (IsVerbose)
                Write(ConsoleColor.Gray, format, parameters);
        }

        public static TResult TryExecute<TResult>(Func<TResult> fn, string verboseInformation)
        {
            try
            {
                Log.Verbose(verboseInformation + "...");
                return fn();
            }
            catch(Exception ex)
            {
                Log.Error("An error occurred:");
                Log.Error(ex.Message);
                System.Environment.Exit(0);
                throw;
            }
        }

        private static void Write(ConsoleColor color, string format, object[] parameters)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(format, parameters);
            Console.ForegroundColor = oldColor;
        }

    }
}
