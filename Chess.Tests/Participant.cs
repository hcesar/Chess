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
        public Participant()
        {
            this.Tests = new List<TestResult>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string HighestEducation { get; set; }
        public string Area { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public Laterality Laterality { get; set; }
        public ChessLevel ChessLevel { get; set; }
        public string ELO { get; set; }

        [XmlElement(ElementName = "Tests")]
        public List<TestResult> Tests { get; set; }


        public static IList<Participant> Load(string file = "tests.participants.xml")
        {
            using (var fs = File.OpenRead(file))
            {
                var list = (ParticipantCollection)new XmlSerializer(typeof(ParticipantCollection)).Deserialize(fs);
                return list.Items;
            }
        }

        public static void Insert(Participant participant)
        {
            var participants = Load();
            participant.Id = participants.Count + 1;
            participants.Add(participant);
            Update(participants);
        }

        public static void Update(Participant participant)
        {
            var participants = (List<Participant>)Load();

            int index = participants.FindIndex(i => i.Id == participant.Id);
            participants[index] = participant;

            Update(participants);
        }

        public static void Update(IList<Participant> participants, string file = "tests.participants.xml")
        {
            using (var fs = new FileStream("tests.participants.xml.new", FileMode.Create, FileAccess.Write))
                new XmlSerializer(typeof(ParticipantCollection)).Serialize(fs, new ParticipantCollection(participants));

            File.Delete(file);
            File.Move("tests.participants.xml.new", file);
        }

        #region ParticipantCollection
        [XmlRoot("Participants")]
        public class ParticipantCollection
        {
            public ParticipantCollection()
            {
            }
            public ParticipantCollection(IList<Participant> items)
            {
                this.Items = (List<Participant>)items;
            }

            [XmlElement("Participant")]
            public List<Participant> Items { get; set; }
        }
        #endregion ParticipantCollection
    }
}