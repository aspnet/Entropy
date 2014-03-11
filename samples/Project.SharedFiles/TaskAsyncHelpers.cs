using System;
using System.Threading.Tasks;

namespace Project.SharedFiles
{
    /// <summary>
    /// This is a shared file
    /// </summary>
    internal class TaskAsyncHelpers
    {
        internal static async void ContinueWith(Task task, TaskCompletionSource<object> tcs)
        {
            try
            {
                await task;
                tcs.TrySetResult(null);
            }
            catch (TaskCanceledException)
            {
                tcs.TrySetCanceled();
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }
    }
}
