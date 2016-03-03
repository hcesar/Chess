using Chess.App.Tests;
using Chess.App.Tests.Volke;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Processing.Commands
{
    [Command("help", "", "Shows commands help.")]
    public class HelpCommand : Command<HelpCommand.Arguments>
    {
        protected override void ExecuteCore(Arguments args)
        {
            var currentCommand = Command.Load(args.Command);

            Log.Information(currentCommand.GetUsageMessage(Common.GetExecutable()));
            Log.Information("");

            if (currentCommand is HelpCommand)
            {
                var cmds = Command.LoadAll();
                Log.Information("Command list:\r\n");
                foreach (var cmd in cmds)
                    Log.Information(cmd.Attribute.Command);
            }

        }

        public class Arguments
        {
            [PositionalArgument(0, "command"), Required, ArgumentDescription("Determines the command to show help")]
            public string Command { get; set; }
        }
    }
}
