using System;
using System.ServiceModel;

namespace TestTimerService
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed)]
    public interface ITimerService
    {
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginWait(string correlationId, int millisecondsToWait, AsyncCallback callback, object state);
        string EndWait(IAsyncResult asyncResult);
    }
}