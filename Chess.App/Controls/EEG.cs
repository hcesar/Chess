using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.App.Controls
{
    public static class EEG
    {
        private static String strFooter = "\r\nNOME=DDD\r\nSOBRENOME=\r\nSEXO=\r\nDOUTOR=Enscer\r\nIDADE=";
        private static String[] FILEPATH = { "controle.txt", "\\\\lapeletro\\iCelera\\controle.txt" };
        private static String[] COMMANDS = { "[GRAVACAO]\r\nCOMANDO=INICIALIZA\r\nNOME_ARQUIVO=", 
                                               "[GRAVACAO]\r\nCOMANDO=FINALIZA", 
                                               "[GRAVACAO]\r\nCOMANDO=GRAVAR\r\nNOME_ARQUIVO=", 
                                               "[GRAVACAO]\r\nCOMANDO=PARA" };
        public static Boolean Connected = false;

        public static void SendCommand(int eegCommand, String id="") {

            if (!Connected)
                return;

            String command = COMMANDS[eegCommand];
            String filePath = Connected ? FILEPATH[1] : FILEPATH[0];
            
            if(eegCommand == EEGCommand.ENABLE || eegCommand == EEGCommand.START)
            {
                if (id == "")
                    id = "999";

                command += id + strFooter;
            }

            System.IO.StreamWriter file = new System.IO.StreamWriter(filePath);
            file.WriteLine(command);
            file.Close();
        }
    }

    public static class EEGCommand {
        public static int ENABLE = 0;
        public static int DISABLE = 1;
        public static int START = 2;
        public static int STOP = 3;
    }
}
