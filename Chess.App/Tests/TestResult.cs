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
        public long Elapsed { get; set; }
        public long Elapsed2 { get; set; }
    }
}
