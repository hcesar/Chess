using Chess.IO;
using System;
using System.Collections.Generic;

namespace Chess.ZTest
{
    internal class StreamTranslator : IChessStream, IDisposable
    {
        public event Action<ChessAction> ActionAvailable;

        private StartAction start;
        private ChessStreamReader reader;
        private IList<Type> sensors;

        public StreamTranslator(string sourceFile)
        {
            this.reader = new ChessStreamReader(sourceFile);
            this.start = reader.ReadAction() as Chess.IO.StartAction;
            if (this.start == null)
                throw new InvalidOperationException("Unexpected first action.");

            this.sensors = this.start.Sensors;
        }

        public void Translate()
        {
            this.ActionAvailable(this.start);
            ChessAction action;
            do
            {
                action = reader.ReadAction();
                //System.Threading.Thread.Sleep(action.TimePosition);
                if (!(action is SensorAction))
                    Console.WriteLine(action);

                this.ActionAvailable(action);
            }
            while (!(action is EndGameAction));
        }

        void IDisposable.Dispose()
        {
        }

        IList<Type> IChessStream.SensorsType
        {
            get { return this.sensors; }
        }
    }
}