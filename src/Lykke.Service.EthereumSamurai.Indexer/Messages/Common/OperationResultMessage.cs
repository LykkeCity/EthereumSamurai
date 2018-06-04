using System.ComponentModel;

namespace Lykke.Job.EthereumSamurai.Messages.Common
{
    [ImmutableObject(true)]
    public sealed class OperationResultMessage
    {
        public OperationResultMessage(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public bool IsSuccess { get; private set; }
    }
}
