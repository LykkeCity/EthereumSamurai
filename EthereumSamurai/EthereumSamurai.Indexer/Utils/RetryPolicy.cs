using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EthereumSamurai.Indexer.Utils
{
    public static class RetryPolicy
    {
        public static async Task Execute(Func<Task> func, int retryCount)
        {
            bool isExecutionCompleted = false;
            int currentTry = 1;

            do
            {
                try
                {
                    await func();
                    isExecutionCompleted = true;
                }
                catch (Exception e)
                {
                    if (currentTry >= retryCount)
                    {
                        throw;
                    }

                    currentTry++;
                }

            } while (!isExecutionCompleted);
        }
    }
}
