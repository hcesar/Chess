using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Chess.Processing
{
    public static class Question
    {
        private static List<string> Questions;
        static Question()
        {
            if (File.Exists("tests.volke.xml"))
            {
                var doc = new XmlDocument();
                doc.Load("tests.volke.xml");
                Questions = doc.SelectNodes("/Tests/Test/Question").Cast<XmlNode>().Select(i => i.InnerText).ToList(); ;
            }
        }

        public static string GetQuestion(int questionIndex)
        {
            if (questionIndex >= Questions.Count)
                return "";

            return Questions[questionIndex];
        }

    }
}
