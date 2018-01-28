using System.Threading.Tasks;

namespace Upp.Net.Trace
{
    public static class TaskTraceExtension
    {
        public static void TraceError(this Task task, ITrace trace)
        {
            task.ContinueWith(_ => trace.Exception(_.Exception), TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}