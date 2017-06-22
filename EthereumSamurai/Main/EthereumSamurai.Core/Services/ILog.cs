using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EthereumSamurai.Core.Services
{
    public interface ILog
    {
        Task WriteInfoAsync(string component, string process, string context, string info, DateTime? dateTime = null);
        Task WriteWarningAsync(string component, string process, string context, string info, DateTime? dateTime = null);
        Task WriteErrorAsync(string component, string process, string context, Exception exeption, DateTime? dateTime = null);
        Task WriteFatalErrorAsync(string component, string process, string context, Exception exeption, DateTime? dateTime = null);
    }
}
