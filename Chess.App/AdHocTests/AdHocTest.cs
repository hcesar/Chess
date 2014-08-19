using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Chess.App
{
    public class AdHocTest
    {
        public string Name { get; set; }

        public string Instruction { get; set; }

        public string StartingFEN { get; set; }

        public int PlayerMoves { get; set; }

        public AILevel AILevel { get; set; }

        public TimeSpan WaitBeforeMove { get; set; }

        public PlayerColor PlayerColor { get; set; }

        public Player GetPlayer(PlayerColor color)
        {
            if (this.PlayerColor == color)
                return new HumanPlayer();
            //return new ComputerPlayer(AILevel.Hard);

            return new ComputerPlayer(this.AILevel < AILevel.Easy ? AILevel.Medium : this.AILevel);
        }

        public AdHocTest()
        {
        }

        public AdHocTest(Stream testFile)
        {
            var xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(testFile);

            var nodes = xmlDocument.SelectNodes("/test/*").Cast<System.Xml.XmlNode>().ToList().ToList();
            foreach (var prop in this.GetType().GetProperties())
            {
                var node = nodes.FirstOrDefault(i => i.Name.Equals(prop.Name, StringComparison.InvariantCultureIgnoreCase));
                if (node == null)
                    continue;

                string value = node.InnerText;

                if (value.Length == 0)
                    continue;

                var converter = TypeDescriptor.GetConverter(prop.PropertyType);
                try
                {
                    prop.SetValue(this, converter.ConvertFromString(value));
                }
                catch
                {
                    throw new InvalidOperationException(string.Format("Não foi possível converter '{0}' em '{1}'.", value, prop.Name));
                }
            }
        }
    }
}