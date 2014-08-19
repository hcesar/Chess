using System;
using System.ServiceModel;

namespace Chess.IO
{
    [ServiceContract(Namespace = "chess.services", CallbackContract = typeof(IChessClientCallback), SessionMode = SessionMode.Required)]
    internal interface IChessServerService
    {
        [OperationContract(IsOneWay = true)]
        void ReceiveMove(Square from, Square target, Type promotePawnTo);

        [OperationContract]
        void Connect();

        [OperationContract]
        string GetStartingFEN();
    }
}