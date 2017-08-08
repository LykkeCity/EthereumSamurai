using System;
using System.Threading.Tasks;

namespace EthereumSamurai.Core.Services
{
    public interface ILog
    {
        Task WriteErrorAsync(string component, string process, string context, Exception exeption,
            DateTime? dateTime = null);

        Task WriteFatalErrorAsync(string component, string process, string context, Exception exeption,
            DateTime? dateTime = null);

        Task WriteInfoAsync(string component, string process, string context, string info,
            DateTime? dateTime = null);

        Task WriteWarningAsync(string component, string process, string context, string info,
            DateTime? dateTime = null);
    }
}