using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Chess.App.Tests
{
    public class Participant
    {
        public string Name { get; set; }
        public string Profession { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }

        public static IList<Participant> Load()
        {
            return new FileInfo("tests.participants.xml").ToEntityList<Participant>("/Participants/*");
        }

        internal static void Insert(Participant participant)
        {
            var participants = Load();
            participants.Add(participant);
            using (var fs = new FileStream("tests.participants.xml", FileMode.Create, FileAccess.Write))
                new XmlSerializer(typeof(Participants)).Serialize(fs, new Participants(participants));
        }

        [XmlRoot("Participants")]
        public class Participants
        {
            public Participants()
            {
            }
            public Participants(IList<Participant> items)
            {
                this.Items = (List<Participant>)items;
            }

            [XmlElement("Participant")]
            public List<Participant> Items { get; set; }
        }
    }
}
