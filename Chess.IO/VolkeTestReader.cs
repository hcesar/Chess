using Chess.IO.VolkeTest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.IO
{
    public class VolkeTestReader : IDisposable
    {
        private BinaryReader reader;

        public VolkeTestReader(string fileName)
            : this(File.OpenRead(fileName))
        {
        }

        public VolkeTestReader(Stream stream)
        {
            reader = new BinaryReader(stream);
        }

        public IEnumerable<VolkeMessage> ReadAll()
        {
            var ret = new List<VolkeMessage>();

            while (true)
            {
                if (reader.BaseStream.Length == reader.BaseStream.Position)
                    break;

                long timeFrame = reader.ReadInt64();
                var messageType = (VolkeMessageType)reader.ReadByte();

                switch (messageType)
                {
                    case VolkeMessageType.TestStart:
                        yield return new VolkeTestStartMessage(timeFrame, reader);
                        break;

                    case VolkeMessageType.MouseSensorData:
                        yield return new VolkeMouseSensor(timeFrame, reader);
                        break;

                    case VolkeMessageType.EyeSensorData:
                        yield return new VolkeEyeSensorMessage(timeFrame, reader);
                        break;

                    case VolkeMessageType.Question:
                        yield return new VolkeQuestionMessage(timeFrame, reader);
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public VolkeTestSummary ReadSummary()
        {
            var ret = new VolkeTestSummary();

            var enumerator = ReadAll().GetEnumerator();
            enumerator.MoveNext();

            bool found = enumerator.SkipWhile(i => !(i is VolkeTestStartMessage || i is VolkeQuestionMessage));
            if (!found) throw new InvalidOperationException("Invalid test file format: Instruction not found.");
            if (enumerator.Current is VolkeTestStartMessage) throw new InvalidOperationException("Invalid test file format: TestStart/Question order mismatch.");
            ret.Instruction = (enumerator.Current as VolkeQuestionMessage).Image;
            enumerator.MoveNext();

            while (true)
            {
                found = enumerator.SkipWhile(i => !(i is VolkeTestStartMessage || i is VolkeQuestionMessage));
                if (!found) break;
                if (enumerator.Current is VolkeTestStartMessage) throw new InvalidOperationException("Invalid test file format: TestStart/Question order mismatch.");

                var volkeItem = new VolkeTestItem();
                volkeItem.Question = (enumerator.Current as VolkeQuestionMessage);
                enumerator.MoveNext();

                enumerator.MoveNext();
                volkeItem.QuestionEyeMovements = enumerator.TakeWhile(i => !(i is VolkeTestStartMessage || i is VolkeQuestionMessage)).OfType<VolkeEyeSensorMessage>().ToList();
                long baseTimeFrame = volkeItem.QuestionEyeMovements.Select(i => i.Timestamp).FirstOrDefault();
                foreach (var eyeMovement in volkeItem.QuestionEyeMovements)
                    eyeMovement.Timestamp -= baseTimeFrame;

                found = enumerator.SkipWhile(i => !(i is VolkeTestStartMessage || i is VolkeQuestionMessage));
                if (!found) throw new InvalidOperationException("Invalid test file format: TestStart/Question not found.");
                if (enumerator.Current is VolkeQuestionMessage) throw new InvalidOperationException("Invalid test file format: TestStart/Question order mismatch.");
                volkeItem.BoardFEN = (enumerator.Current as VolkeTestStartMessage).FEN;

                enumerator.MoveNext();
                volkeItem.AnswerEyeMovements = enumerator.TakeWhile(i => !(i is VolkeTestStartMessage || i is VolkeQuestionMessage)).OfType<VolkeEyeSensorMessage>().ToList();

                baseTimeFrame = volkeItem.AnswerEyeMovements.Select(i => i.Timestamp).FirstOrDefault();
                foreach (var eyeMovement in volkeItem.AnswerEyeMovements)
                    eyeMovement.Timestamp -= baseTimeFrame;
                ret.TestItems.Add(volkeItem);
            }


            return ret;
        }

        public void Dispose()
        {
            this.reader.Dispose();
        }
    }

    internal enum VolkeMessageType : byte
    {
        TestStart = 1,
        MouseSensorData = 2,
        Question = 3,
        EyeSensorData = 4,
    }
}
