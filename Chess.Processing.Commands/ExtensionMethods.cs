using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Chess.Processing.Commands;

namespace Chess.Processing
{
    public static class ExtensionMethods
    {
        public static string GetArgumentName(this PropertyInfo property, bool onlyFirst = false)
        {
            string name;

            var attr = property.GetCustomAttribute<ArgumentAttribute>();
            if (attr != null)
            {
                name = attr.GetArgumentName();
                if (onlyFirst)
                    name = name.Split(',').First();
            }
            else
            {
                name = property.Name;
            }

            return name;
        }
    }
}
