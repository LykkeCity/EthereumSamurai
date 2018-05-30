using System; 
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.EthereumSamurai.Core.Exceptions;

namespace Lykke.Job.EthereumSamurai.Utils
{
    public static class TimeoutPolicy
    {
        /// <summary>
        /// Retry policy with exponential waiting before retries
        /// </summary>
        /// <returns></returns>
        public static async Task ExecuteAsync(Func<Task> func, TimeSpan timeouTimeSpan)
        {
            var task = func();
            if (await Task.WhenAny(task, Task.Delay(timeouTimeSpan)) == task)
            {
                await task;
            }
            else
            {
                // timeout/cancellation logic
                throw new OperationTimedOutException("Operation timed out");
            }
        }
    }
}
