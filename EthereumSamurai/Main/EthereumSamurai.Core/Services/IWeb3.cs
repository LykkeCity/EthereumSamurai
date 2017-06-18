using Nethereum.Contracts;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC;
using Nethereum.RPC.TransactionManagers;
using Nethereum.Signer;
using Nethereum.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace EthereumSamurai.Core.Services
{
    public interface IWeb3
    {
        ShhApiService Shh { get; }
        EthApiContractService Eth { get; }
        IClient Client { get; }
        TransactionSigner OfflineTransactionSigner { get; }
        UnitConversion Convert { get; }
        ITransactionManager TransactionManager { get; set; }
        PersonalApiService Personal { get; }
        NetApiService Net { get; }

        string GetAddressFromPrivateKey(string privateKey);
        bool IsChecksumAddress(string address);
        string Sha3(string value);
        string ToChecksumAddress(string address);
        string ToValid20ByteAddress(string address);
    }
}
