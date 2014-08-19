using System;
using System.ServiceModel;

namespace Chess.IO
{
    internal interface IChessClientCallback
    {
        [OperationContract(IsOneWay = true)]
        void ReceiveMove(Square from, Square target, Type promotePawnTo);
    }
}