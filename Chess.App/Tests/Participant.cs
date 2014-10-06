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
        public string Profession { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }

        [XmlElement(ElementName = "Tests")]
        public List<TestResult> Tests { get; set; }


        public static IList<Participant> Load()
        {
            return new FileInfo("tests.participants.xml").ToEntityList<Participant>("/Participants/*");
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

        private static void Update(IList<Participant> participants)
        {
            using (var fs = new FileStream("tests.participants.xml", FileMode.Create, FileAccess.Write))
                new XmlSerializer(typeof(ParticipantCollection)).Serialize(fs, new ParticipantCollection(participants));
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