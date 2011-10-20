using System;
using System.ServiceModel;
using System.Threading;

namespace TestTimerService
{
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerCall,
        ConcurrencyMode = ConcurrencyMode.Multiple
    )]
    public class TimerService : ITimerService
    {
        public IAsyncResult BeginWait(string correlationId, int millisecondsToWait, AsyncCallback callback, object state)
        {
            return new TimerAsyncResult(correlationId, millisecondsToWait, state, callback);
        }

        public string EndWait(IAsyncResult asyncResult)
        {
            return ((TimerAsyncResult) asyncResult).CorrelationId;
        }
    }

    public class TimerAsyncResult : IAsyncResult
    {
        private readonly string correlationId;

        public string CorrelationId
        {
            get { return correlationId; }
        }

        public TimerAsyncResult(string correlationId, int millisecondsToWait, object asyncState, AsyncCallback asyncCallback)
        {
            this.correlationId = correlationId;

            IsCompleted = false;
            AsyncState = asyncState;

            // mimic a long running operation
            var timer = new System.Timers.Timer(millisecondsToWait);
            timer.Elapsed += (_, args) =>
            {
                IsCompleted = true;
                asyncCallback(this);
                timer.Enabled = false;
                timer.Dispose();
            };
            timer.Enabled = true;
        }

        public bool IsCompleted { get; private set; }

        // assume that WCF uses a callback rather than the AsyncWaitHandle
        public WaitHandle AsyncWaitHandle
        {
            get { return null; }
        }

        public object AsyncState { get; private set; }

        public bool CompletedSynchronously
        {
            get { return false; }
        }
    }
}