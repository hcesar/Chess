using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Chess.App
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            //AllocConsole();
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            Application.SetCompatibleTextRenderingDefault(false);

            var sidePlayer = new SidePlayer();
            var chessForm = new ChessForm(sidePlayer);
            sidePlayer.ChessForm = chessForm;

            new Thread(() =>
            {
                Application.EnableVisualStyles();
                Application.Run(sidePlayer);
            }).Start();

            Application.EnableVisualStyles();
            Application.Run(chessForm);          
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject);
           // Console.ReadLine();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();
    }
}