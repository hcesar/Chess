using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Chess.App.Tests
{
    [XmlInclude(typeof(AdHoc.AdHocTestResult))]
    [XmlInclude(typeof(Volke.VolkeTestResult))]
    public class TestResult
    {
        public string Id { get; set; }
        public string RecordFile { get; set; }
        public string Result { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime DateQuestion { get; set; }
        public DateTime DateAnswer { get; set; }
        public long TimestampQuestion { get; set; }
        public long TimestampAnswer {get; set;}
        public long ElapsedQuestion { get; set; }
        public long ElapsedAnswer { get; set; }
    }
}
