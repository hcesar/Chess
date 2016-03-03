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


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ArgumentAttribute : Attribute
    {
        public string Argument { get; private set; }

        public ArgumentAttribute(string argument)
        {
            this.Argument = argument;
        }

        public virtual string GetArgumentName()
        {
            return "-" + this.Argument.Replace(",", ", -");
        }

        public virtual string GetArgumentUsage(PropertyInfo prop, int maxNameSize)
        {
            string name = this.GetArgumentName();
            var attrDescription = prop.GetCustomAttribute<ArgumentDescriptionAttribute>();
            string description = attrDescription != null ? attrDescription.Description : "";

            StringBuilder sb = new StringBuilder();
            bool first = true;
            string defaultSpace = new string(' ', maxNameSize + 3);
            foreach (var descriptionPart in SplitBySize(description, Console.WindowWidth - (defaultSpace.Length + 1)))
            {
                if (first)
                    sb.Append("  " + name)
                      .Append(new string(' ', defaultSpace.Length - 2 - name.Length));
                else
                    sb.AppendLine().Append(defaultSpace);
                sb.Append(descriptionPart);

                first = false;
            }

            return sb.ToString();
        }

        private IEnumerable<string> SplitBySize(string text, int size)
        {
            if (text.Contains('\n'))
            {
                foreach (var line in text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                    foreach (var desc in SplitBySize(line, size))
                        yield return desc;

                yield break;
            }

            if (text.Length < size)
            {
                yield return text;
                yield break;
            }

            string remaining = text;
            while (remaining.Length > size)
            {
                var idx = remaining.LastIndexOf(' ', Math.Min(remaining.Length, size));
                if (idx < 0) idx = size;

                yield return remaining.Substring(0, idx);
                remaining = remaining.Remove(0, idx).Trim();
            }

            if (remaining.Length > 0)
                yield return remaining;
        }
    }

    public class PositionalArgumentAttribute : ArgumentAttribute
    {
        public int Position { get; private set; }

        public PositionalArgumentAttribute(int position, string name)
            : base(name)
        {
            this.Position = position;
        }

        public override string GetArgumentName()
        {
            return "<" + this.Argument + ">";
        }

    }

    public class ArgumentDescriptionAttribute : Attribute
    {
        public string Description { get; private set; }

        public ArgumentDescriptionAttribute(string description)
        {
            this.Description = description;
        }
    }

    public class RequiredAttribute : Attribute
    {
    }

    public class DefaultValueAttribute : Attribute
    {
        public string Value { get; private set; }

        public DefaultValueAttribute(string value)
        {
            this.Value = value;
        }
    }


}
