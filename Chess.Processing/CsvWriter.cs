using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Processing
{
    public class CsvWriter
    {
        public static void Write<T>(Stream stream, IEnumerable<T> collection)
        {
            var writer = new CsvWriter<T>(stream);
            writer.Write(collection);
        }
    }

    public class CsvWriter<T>
    {
        private StreamWriter writer;
        IList<PropertyInfo> columns = typeof(T).GetProperties();

        public CsvWriter(Stream stream)
        {
            this.writer = new StreamWriter(stream);
        }

        public void Write(IEnumerable<T> collection)
        {
            WriteLine((col) => col.Name);
            foreach (var item in collection)
                WriteLine((col) => col.GetValue(item));
            this.writer.Close();
        }

        private void WriteLine(Func<PropertyInfo, object> columnValueGetter)
        {
            bool first = true;
            foreach (var col in columns)
            {
                if (!first)
                    writer.Write(';');

                var value = columnValueGetter(col);
                writer.Write(ParseString(value));
                first = false;
            }

            writer.WriteLine();
        }

        private string ParseString(object value)
        {
            return Convert.ChangeType(value, typeof(string)) as string;
        }
    }
}
