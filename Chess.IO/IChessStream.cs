using System;
using System.Collections.Generic;

namespace Chess.IO
{
    public interface IChessStream : IDisposable
    {
        event Action<ChessAction> ActionAvailable;

        IList<Type> SensorsType { get; }
    }
}