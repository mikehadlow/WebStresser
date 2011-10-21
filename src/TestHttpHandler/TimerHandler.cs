using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace TestHttpHandler
{
    public class TimerHandler : IHttpAsyncHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            throw new InvalidOperationException("This handler cannot be called synchronously");
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback callback, object state)
        {
            var taskCompletionSouce = new TaskCompletionSource<bool>(state);
            var task = taskCompletionSouce.Task;

            var timer = new System.Threading.Timer(timerState =>
            {
                context.Response.Write("OK");
                callback(task);
                taskCompletionSouce.SetResult(true);
            });
            timer.Change(1000, Timeout.Infinite);

            return task;
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            // nothing to do
        }
    }
}