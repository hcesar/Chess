using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Processing.Commands
{
    public class CommandAttribute : Attribute
    {
        public string Command { get; private set; }
        public string Description { get; private set; }
        public string Title { get; private set; }

        public CommandAttribute(string command, string title, string description)
        {
            this.Command = command;
            this.Title = title;
            this.Description = description;
        }
    }
    public class ArgumentAttribute : Attribute
    {
    }

    public class NamedArgumentAttribute : ArgumentAttribute
    {
        public string Argument { get; private set; }

        public NamedArgumentAttribute(string argument)
        {
            this.Argument = argument;
        }
    }

    public class PositionalArgumentAttribute : ArgumentAttribute
    {
        public int Position { get; private set; }

        public PositionalArgumentAttribute(int position)
        {
            this.Position = position;
        }
    }
}
