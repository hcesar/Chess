using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Chess.App.Tests
{
    public abstract class Test
    {
        public string Name { get; set; }

        public string Instruction { get; set; }

        public abstract TestOrchestrator GetOrchestrator(BoardControl boardControl);


        public void OnPreInit(BoardControl boardControl)
        {

            this.OnInit();
        }

        protected virtual void OnInit()
        {
        }


        internal static IList<Test> LoadAll()
        {
            var file = new FileInfo("tests.xml");

            if (!file.Exists)
                return new Test[0];

            XmlDocument xml = new XmlDocument();
            using (var fs = file.OpenRead())
                xml.Load(fs);

            var ret = new List<Test>();
            foreach (XmlNode node in xml.SelectNodes("/Tests/*"))
            {
                var n = node.SelectSingleNode("./Type");

                if (n == null)
                    throw new InvalidOperationException("Test Type must be informed.");

                var typeName = n.InnerText;
                var type = FindType(typeName);

                if (type == null)
                    throw new InvalidOperationException(string.Format("'{0}' is not a valid Test.", typeName));

                var obj = (Test)Activator.CreateInstance(type);
                node.ToEntity(obj);

                ret.Add(obj);
            }

            return ret;
        }

        private static Type FindType(string testType)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(i => i.GetTypes().Where(t => typeof(Test).IsAssignableFrom(t))).ToList();
            var type = types.FirstOrDefault(i => i.Name.Equals(testType, StringComparison.InvariantCultureIgnoreCase));

            if (type != null)
                return type;

            testType += "Test";
            return types.FirstOrDefault(i => i.Name.Equals(testType, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}