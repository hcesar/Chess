using Chess.App.Tests;
using Chess.App.Tests.Volke;
using Chess.IO;
using Chess.IO.VolkeTest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Processing.Commands
{
    [Command("summary", "Export Summary", "Exports the summary of proficiency test.")]
    public class ExportSummaryCommand : Command<ExportSummaryCommand.Arguments>
    {
        protected override void ExecuteCore(Arguments args)
        {
            if ((args.ExportsDurations || args.ExportsQualities) && string.IsNullOrEmpty(args.TestFilesPath))
                throw new InvalidOperationException("The argument '-tpath' is required when '-d' or '-q' is selected.");

            Log.Information("Exporting summary...");
            var participants = Participant.Load(args.InputFile);

            participants = participants
                                    .Where(i => args.Participants.Count == 0 || args.Participants.Contains(i.Id))
                                    .Where(i => args.ParticipantsExcluded.Count == 0 || !args.ParticipantsExcluded.Contains(i.Id))
                                    .ToList();

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
            sb.Append("Id;Name;Age;Laterality;Chess Level;ELO;Correct Answers; Wrong Answers; Elapsed (seconds); Elapsed (minutes);Score");

            if (args.ExportsQualities)
                for (int i = 0; i < N; i++)
                    sb.Append(";Quality T").Append((i % N) + 1);

            if (args.ExportsDurations)
                for (int i = 0; i < N; i++)
                    sb.Append(";Dur. T").Append((i % N) + 1);

            sb.AppendLine();


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
                    .AppendFormat("{0:0.00};", getScore(entry.Test));


                if (!string.IsNullOrEmpty(args.TestFilesPath))
                {
                    string testFile = Path.Combine(args.TestFilesPath, entry.Participant.Id + ".volke");
                    var testSummary = new VolkeTestReader(testFile).Using(i => i.ReadSummary());

                    if (args.ExportsQualities)
                        foreach (var testItem in testSummary.TestItems)
                            sb.AppendFormat("{0:0};", GetQuality(testItem));

                    if (args.ExportsDurations)
                    {
                        foreach (var testItem in testSummary.TestItems)
                        {
                            var time = testItem.AnswerEyeMovements.Select(i => i.Timestamp).LastOrDefault() / 1000f;
                            sb.AppendFormat("{0:0.0}s;", time);
                        }
                    }
                }

                sb.AppendLine();
            }
            if (error)
                return;

            if (!Directory.Exists(ProcessingSettings.OutputDirectory))
                Directory.CreateDirectory(ProcessingSettings.OutputDirectory);

            var output = Path.Combine(ProcessingSettings.OutputDirectory, args.OutputFile ?? Path.GetFileNameWithoutExtension(args.InputFile) + ".csv");
            File.WriteAllText(output, sb.ToString(), Encoding.UTF8);
            Log.Information("Done!");
        }

        private decimal GetQuality(VolkeTestItem testItem)
        {
            if (testItem.AnswerEyeMovements.Count == 0)
                return 0L;

            return Math.Round(100 * (1 - testItem.AnswerEyeMovements.Count(i => i.X == 0 && i.Y == 0) / (decimal)testItem.AnswerEyeMovements.Count), 0);
        }

        public class Arguments
        {
            [PositionalArgument(0, "input"), Required, ArgumentDescription("Defines the input xml file")]
            public string InputFile { get; set; }

            [PositionalArgument(1, "output"), ArgumentDescription("Defines the output csv file")]
            public string OutputFile { get; set; }

            [Argument("tpath"), ArgumentDescription("Test files path")]
            public string TestFilesPath { get; set; }

            [Argument("p"), ArgumentDescription("Participants included")]
            public IList<int> Participants { get; set; }

            [Argument("xp"), ArgumentDescription("Participants not included")]
            public IList<int> ParticipantsExcluded { get; set; }

            [Argument("q"), ArgumentDescription("Exports test gaze quality (required tpath flags)")]
            public bool ExportsQualities { get; set; }

            [Argument("d"), ArgumentDescription("Exports test gaze durations (required tpath flags)")]
            public bool ExportsDurations { get; set; }

            public Arguments()
            {
                this.Participants = new int[0];
                this.ParticipantsExcluded = new int[0];
            }
        }
    }
}
