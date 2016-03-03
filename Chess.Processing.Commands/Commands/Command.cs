using Chess.Processing.Commands;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;

namespace Chess.Processing
{
    public interface ICommand
    {
        CommandAttribute Attribute { get; }

        void Execute(List<string> args);
        string GetUsageMessage(string executable);
    }

    public abstract class Command<TArgs> : ICommand
       where TArgs : class, new()
    {
        public CommandAttribute Attribute { get { return this.GetType().GetCustomAttributes(typeof(CommandAttribute), true).Cast<CommandAttribute>().First(); } }

        protected abstract void ExecuteCore(TArgs args);

        public virtual string GetUsageMessage(string executable)
        {
            var sb = new StringBuilder();
            sb
                .AppendLine(this.Attribute.Description)
                .AppendLine()
                .AppendLine("Usage:")
                .AppendFormat("{0} {1} ", executable, this.Attribute.Command);

            var argumentList = this.GetArguments()
                                    .OrderBy(i => (i.GetCustomAttribute<PositionalArgumentAttribute>() != null) ? i.GetCustomAttribute<PositionalArgumentAttribute>().Position : int.MaxValue)
                                    .ThenBy(i => i.GetCustomAttribute<RequiredAttribute>() != null ? "0" + i.Name : i.Name)
                                    .ToList();

            foreach (var prop in argumentList)
            {
                bool required = prop.GetCustomAttribute<RequiredAttribute>() != null;
                string argumentName = prop.GetArgumentName(true);
                if (prop.PropertyType != typeof(bool) && prop.GetCustomAttribute<PositionalArgumentAttribute>() == null)
                    argumentName += " <value>";

                if (!required)
                    argumentName = "[" + argumentName + "]";

                sb.Append(argumentName).Append(" ");
            }

            sb.AppendLine("\r\n");
            foreach (var prop in argumentList)
                sb
                    .AppendLine(prop.GetCustomAttribute<ArgumentAttribute>().GetArgumentUsage(prop, argumentList.Max(i => i.GetArgumentName().Length)));

            return sb.ToString();
        }

        protected IEnumerable<PropertyInfo> GetArguments(bool onlyRequireds = false)
        {
            if (onlyRequireds)
                return typeof(TArgs).GetProperties().Where(i => i.GetCustomAttribute<RequiredAttribute>() != null);

            return typeof(TArgs).GetProperties().Where(i => i.GetCustomAttribute<ArgumentAttribute>() != null);
        }

        public static TArgs ParseArguments(List<string> args)
        {
            var properties = typeof(TArgs).GetProperties().Select(i => new { Property = i, Argument = i.GetCustomAttribute<ArgumentAttribute>() }).ToList();
            var cmdArgs = CommandArgument.Load(args);
            int positionalArgs = 0;

            var ret = new TArgs();
            for (int i = 0; i < cmdArgs.Count; i++)
            {
                PropertyInfo prop;
                if (cmdArgs[i].Argument == null)
                {
                    prop = properties.FirstOrDefault(p => p.Argument is PositionalArgumentAttribute && ((PositionalArgumentAttribute)p.Argument).Position == positionalArgs).Property;

                    if (prop == null)
                        throw new InvalidOperationException(string.Format("Positional argumento '{0}' not defined for this command.", positionalArgs));

                    positionalArgs++;
                }
                else
                {
                    var propArgs = properties.FirstOrDefault(p => p.Argument is ArgumentAttribute && ContainsArgument(((ArgumentAttribute)p.Argument).Argument, cmdArgs[i].Argument));

                    if (propArgs == null)
                        propArgs = properties.FirstOrDefault(p => p.Property.Name.Equals(cmdArgs[i].Argument, StringComparison.InvariantCultureIgnoreCase));

                    if (propArgs == null)
                        throw new InvalidOperationException(string.Format("agument '{0}' not defined for this command.", cmdArgs[i].Argument));

                    prop = propArgs.Property;
                }

                if (cmdArgs[i].Value == null)
                    cmdArgs[i].Value = "true";

                var propType = prop.PropertyType;
                if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    propType = propType.GetGenericArguments().First();

                object value;
                if (propType.IsEnum)
                {
                    var enumValues = cmdArgs[i].Value.Split(',').Select(s => Enum.Parse(propType, s, true)).ToList();
                    if (enumValues.Count == 1)
                        value = enumValues.First();
                    else
                        value = GetFlagValue(enumValues, propType);
                }
                else if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    var elementType = propType.GetGenericArguments().First();
                    var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
                    foreach (var v in cmdArgs[i].Value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(v => Convert.ChangeType(v, elementType)))
                        list.Add(v);
                    value = list;
                }
                else
                {
                    value = Convert.ChangeType(cmdArgs[i].Value, propType);
                }

                prop.SetValue(ret, value);
            }

            return ret;
        }

        private static object GetFlagValue(IList<object> enumValues, Type enumType)
        {
            int value = 0;
            var intValues = enumValues.Select(i => (int)i).ToList();
            foreach (var v in intValues)
                value |= v;

            return Enum.ToObject(enumType, value);
        }

        private static bool ContainsArgument(string argumentList, string argument)
        {
            if (argumentList.IndexOf(',') < 0)
                return argumentList.Equals(argument, StringComparison.InvariantCultureIgnoreCase);

            return argumentList.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Any(i => ContainsArgument(i, argument));
        }

        public void Execute(List<string> args)
        {
            var errors = new List<PropertyInfo>();
            var cmdArgs = CommandArgument.Load(args);
            foreach (var argProp in typeof(TArgs).GetProperties().Where(p => p.GetCustomAttribute<RequiredAttribute>() != null))
            {
                var positionalAttr = argProp.GetCustomAttribute<PositionalArgumentAttribute>();
                if (positionalAttr != null && (cmdArgs.Count <= positionalAttr.Position || cmdArgs[positionalAttr.Position].Argument != null))
                {
                    new HelpCommand().Execute(new List<string> { this.Attribute.Command });
                    System.Environment.Exit(0);
                }
            }

            var arguments = Log.TryExecute(() => ParseArguments(args), "Parsing arguments");

            try
            {
                this.ExecuteCore(arguments);
            }
            catch(Exception ex)
            {
                Log.Error(ex.Message);
            }
        }


    }

    public abstract class Command
    {
        public static ICommand Load(string command)
        {
            return LoadAll().FirstOrDefault(i => i.Attribute.Command.Equals(command, StringComparison.InvariantCultureIgnoreCase));
        }
        public static IList<ICommand> LoadAll()
        {
            var ret = new List<ICommand>();
            foreach (var cmdType in typeof(Command).Assembly.GetTypes().Where(IsCommand))
            {
                var cmd = Activator.CreateInstance(cmdType) as ICommand;
                ret.Add(cmd);
            }

            return ret;
        }

        private static bool IsCommand(Type type)
        {
            return IsCommand(type, false);
        }
        private static bool IsCommand(Type type, bool canBeAbstract)
        {
            if ((!canBeAbstract && type.IsAbstract) || type == typeof(object))
                return false;

            if (type.IsSubclassOf(typeof(Command<List<string>>)))
                return true;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Command<>))
                return true;

            return IsCommand(type.BaseType, true);
        }
    }



}
