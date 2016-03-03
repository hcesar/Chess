using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Processing.Commands
{
    class CommandArgument
    {
        public string Argument { get; set; }
        public string Value { get; set; }

        public static IList<CommandArgument> Load(IList<string> arguments)
        {
            var ret = new List<CommandArgument>();

            var enumerator = arguments.GetEnumerator();
            enumerator.MoveNext();

            while (true)
            {
                if (enumerator.Current.StartsWith("-"))
                {
                    var argument = enumerator.Current.Substring(1);
                    bool hasNext = enumerator.MoveNext();
                    string value = hasNext && !enumerator.Current.StartsWith("-") ? enumerator.Current : null;

                    ret.Add(new CommandArgument { Argument = argument, Value = value });

                    if (hasNext && enumerator.Current.StartsWith("-")) //Skip movenext
                        continue;
                }
                else
                {
                    ret.Add(new CommandArgument { Value = enumerator.Current });
                }

                if (!enumerator.MoveNext())
                    break;
            }

            return ret;
        }
    }
}
