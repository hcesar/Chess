using Chess.Processing.Commands;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Processing
{
    public abstract class Command : Command<List<string>>
    {
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

    public abstract class Command<TArgs> : ICommand
        where TArgs : class, new()
    {
        public CommandAttribute Attribute
        {
            get
            {
                return this.GetType().GetCustomAttributes(typeof(CommandAttribute), true).Cast<CommandAttribute>().First();
            }
        }
        protected abstract void ExecuteCore(TArgs args);
        public abstract string GetUsageMessage(string executable);

        public static TArgs ParseArguments(List<string> args)
        {
            if (typeof(TArgs) == typeof(List<string>))
                return args as TArgs;

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
                    var propArgs = properties.FirstOrDefault(p => p.Argument is NamedArgumentAttribute && ContainsArgument(((NamedArgumentAttribute)p.Argument).Argument, cmdArgs[i].Argument));

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

                object value = cmdArgs[i].Value;
                if (propType.IsEnum)
                    value = Enum.Parse(propType, value.ToString(), true);
                else
                    value = Convert.ChangeType(value, propType);

                prop.SetValue(ret, value);
            }

            return ret;
        }

        private static bool ContainsArgument(string argumentList, string argument)
        {
            if (argumentList.IndexOf(',') < 0)
                return argumentList.Equals(argument, StringComparison.InvariantCultureIgnoreCase);

            return argumentList.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Any(i => ContainsArgument(i, argument));
        }

        public void Execute(List<string> args)
        {
            var arguments = Log.TryExecute(() => ParseArguments(args), "Parsing arguments");
            this.ExecuteCore(arguments);
        }


    }
    public interface ICommand
    {
        CommandAttribute Attribute { get; }

        void Execute(List<string> args);
        string GetUsageMessage(string executable);
    }
}
