using Chess.App.Tests;
using Chess.App.Tests.Volke;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Processing.Commands
{
    [Command("summary", "Export Summary", "This command exports the summary of proficiency test.")]
    public class ExportSummaryCommand : Command
    {
        protected override void ExecuteCore(List<string> args)
        {
            Log.Information("Exporting summary...");
            var participants = Participant.Load(args.First());

            foreach (var participant in participants)
            {
                if (participant.Tests.Count > 1)
                    Log.Warning("Participant {0} has more than one test. Using last one.", participant.Id);
            }

            var entries = participants
                                .Select(p => new { Participant = p, Test = p.Tests.Cast<VolkeTestResult>().Last() })
                                .ToList();

            int N = entries[0].Test.VolkeTests.Count;
            double RTm = entries.SelectMany(i => i.Test.VolkeTests).Average(i => i.ElapsedAnswer);

            var sb = new StringBuilder();
            sb.AppendLine("Id;Name;Age;Laterality;Chess Level;ELO;Correct Answers; Wrong Answers; Elapsed (seconds); Elapsed (minutes);Score");

            Func<VolkeTestResult, double> getScore = (entry) =>
            {
                int Ncorrect = entry.CorrectAnswers;
                double RTs = entry.VolkeTests.Average(i => i.ElapsedAnswer);
                return (Ncorrect - (N / 2.0)) * (RTm / RTs);
            };


            bool error = false;
            foreach (var entry in entries.OrderByDescending(entry => getScore(entry.Test)))
            {
                if (entry.Test.VolkeTests.Count != N)
                {
                    error = true;
                    Log.Error("Participant {0} don't have the same number os answers.", entry.Participant.Name);
                }

                var test = entry.Participant.Tests.Cast<VolkeTestResult>().Last();
                sb
                    .AppendFormat("\"{0}\";", entry.Participant.Id)
                    .AppendFormat("{0};", entry.Participant.Name)
                    .AppendFormat("{0};", entry.Participant.Age)
                    .AppendFormat("{0};", entry.Participant.Laterality)
                    .AppendFormat("{0};", entry.Participant.ChessLevel)
                    .AppendFormat("{0};", entry.Participant.ELO)
                    .AppendFormat("{0};", entry.Test.CorrectAnswers)
                    .AppendFormat("{0};", entry.Test.VolkeTests.Count - entry.Test.CorrectAnswers)
                    .AppendFormat("{0};", entry.Test.ElapsedAnswer / 1000)
                    .AppendFormat("\"{0:00}m{1:00}s\";", TimeSpan.FromMilliseconds(entry.Test.ElapsedAnswer).Minutes, TimeSpan.FromMilliseconds(entry.Test.ElapsedAnswer).Seconds)
                    .AppendFormat("{0:0.00};", getScore(entry.Test))
                    .AppendLine();
                ;
            }
            if (error)
                return;

            File.WriteAllText(args[1], sb.ToString(), Encoding.UTF8);
            Log.Information("Done!");
        }

        public override string GetUsageMessage(string executable)
        {
            return executable + " summary inputfile.xml outputfile.csv";
        }
    }
}
