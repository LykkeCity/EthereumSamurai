using Nethereum.Contracts;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC;
using Nethereum.RPC.TransactionManagers;
using Nethereum.Signer;
using Nethereum.Util;

namespace Lykke.Service.EthereumSamurai.Core.Services
{
    public interface IWeb3
    {
        IClient Client { get; }

        UnitConversion Convert { get; }

        EthApiContractService Eth { get; }

        NetApiService Net { get; }

        TransactionSigner OfflineTransactionSigner { get; }

        PersonalApiService Personal { get; }

        ShhApiService Shh { get; }

        ITransactionManager TransactionManager { get; set; }



        string GetAddressFromPrivateKey(string privateKey);

        bool IsChecksumAddress(string address);

        string Sha3(string value);

        string ToChecksumAddress(string address);

        string ToValid20ByteAddress(string address);
    }
}