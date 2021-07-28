using System;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC;
using Nethereum.RPC.Eth.DTOs;

namespace Lykke.Service.EthereumSamurai.Core.Eip1559
{
    public class Eip1559EthGetBlockWithTransactionsByNumber :
        RpcRequestResponseHandler<Eip1559BlockWithTransactions>
    {
        public Eip1559EthGetBlockWithTransactionsByNumber(IClient client)
            : base(client, ApiMethods.eth_getBlockByNumber.ToString())
        {
        }

        public Task<Eip1559BlockWithTransactions> SendRequestAsync(
            HexBigInteger number,
            object id = null)
        {
            return !((HexRPCType<BigInteger>)number == (HexRPCType<BigInteger>)null) ? this.SendRequestAsync(id, (object)number, (object)true) : throw new ArgumentNullException(nameof(number));
        }

        public RpcRequest BuildRequest(HexBigInteger number, object id = null) => !((HexRPCType<BigInteger>)number == (HexRPCType<BigInteger>)null) ? this.BuildRequest(id, (object)number, (object)true) : throw new ArgumentNullException(nameof(number));

        public RpcRequest BuildRequest(BlockParameter blockParameter, object id = null) => blockParameter != null ? this.BuildRequest(id, (object)blockParameter, (object)true) : throw new ArgumentNullException(nameof(blockParameter));
    }
}
