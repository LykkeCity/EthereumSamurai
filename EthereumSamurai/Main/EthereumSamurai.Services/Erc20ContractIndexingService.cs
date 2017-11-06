using System;
using System.Numerics;
using System.Threading.Tasks;
using EthereumSamurai.Core.Repositories;
using EthereumSamurai.Core.Services;
using EthereumSamurai.Core.Services.Erc20;
using EthereumSamurai.Models.Blockchain;
using Nethereum.Contracts;

namespace EthereumSamurai.Services
{
    public class Erc20ContractIndexingService : IErc20ContractIndexingService
    {
        private const string MetadataAbi = @"[{""constant"":true,""inputs"":[],""name"":""name"",""outputs"":[{""name"":"""",""type"":""string""}],""payable"":false,""type"":""function""},{""constant"":true,""inputs"":[],""name"":""totalSupply"",""outputs"":[{""name"":""totalSupply"",""type"":""uint256""}],""payable"":false,""type"":""function""},{""constant"":true,""inputs"":[],""name"":""decimals"",""outputs"":[{""name"":"""",""type"":""uint8""}],""payable"":false,""type"":""function""},{""constant"":true,""inputs"":[],""name"":""symbol"",""outputs"":[{""name"":"""",""type"":""string""}],""payable"":false,""type"":""function""}]";
        
        private readonly IIndexingRabbitNotifier     _rabbitQueuePublisher;
        private readonly IErc20ContractRepository    _erc20ContractRepository;
        private readonly IErc20Detector              _erc20Detector;
        private readonly IErc20ContractIndexingQueue _indexingQueue;
        private readonly ITransactionRepository      _transactionRepository;
        private readonly IWeb3                       _web3;

        public Erc20ContractIndexingService(
            IErc20ContractRepository erc20ContractRepository,
            IErc20Detector erc20Detector,
            IErc20ContractIndexingQueue indexingQueue,
            ITransactionRepository transactionRepository,
            IWeb3 web3,
            IIndexingRabbitNotifier rabbitQueuePublisher)
        {
            _rabbitQueuePublisher    = rabbitQueuePublisher;
            _erc20ContractRepository = erc20ContractRepository;
            _erc20Detector           = erc20Detector;
            _indexingQueue           = indexingQueue;
            _transactionRepository   = transactionRepository;
            _web3                    = web3;
        }

        public async Task<DeployedContractModel> GetNextContractToIndexAsync()
        {
            do
            {
                var deployedContract = _indexingQueue.Dequeue();

                if (deployedContract != null)
                {
                    return deployedContract;
                }

                await Task.Delay(10000);

            } while (true);
        }

        public async Task IndexContractAsync(DeployedContractModel deployedContract)
        {
            if (await TransactionExists(deployedContract.TransactionHash) &&
                await _erc20Detector.IsContractErc20Compatible(deployedContract.Address))
            {
                var ethereumContract = _web3.Eth.GetContract(MetadataAbi, deployedContract.Address);
                var erc20Contract    = new Erc20ContractModel
                {
                    Address         = deployedContract.Address,
                    BlockHash       = deployedContract.BlockHash,
                    BlockNumber     = BigInteger.Parse(deployedContract.BlockNumber),
                    BlockTimestamp  = BigInteger.Parse(deployedContract.BlockTimestamp),
                    DeployerAddress = deployedContract.DeployerAddress,
                    TransactionHash = deployedContract.TransactionHash
                };

                await Task.WhenAll
                (
                    Task.Run(() =>
                    {
                        if (TryCallFunction(ethereumContract, "name", out string name))
                        {
                            erc20Contract.TokenName = name;
                        }
                    }),
                    Task.Run(() =>
                    {
                        if (TryCallFunction(ethereumContract, "symbol", out string symbol))
                        {
                            erc20Contract.TokenSymbol = symbol;
                        }
                    }),
                    Task.Run(() =>
                    {
                        if (TryCallFunction(ethereumContract, "decimals", out uint decimals))
                        {
                            erc20Contract.TokenDecimals = decimals;
                        }
                    }),
                    Task.Run(() =>
                    {
                        if (TryCallFunction(ethereumContract, "totalSupply", out BigInteger totalSupply))
                        {
                            erc20Contract.TokenTotalSupply = totalSupply;
                        }
                    })
                );
                
                await _erc20ContractRepository.SaveAsync(erc20Contract);

                await _rabbitQueuePublisher.NotifyAsync(new EthereumSamurai.Models.Messages.RabbitContractIndexingMessage()
                {
                    Address = erc20Contract.Address,
                    BlockHash = erc20Contract.BlockHash,
                    BlockNumber = (ulong)erc20Contract.BlockNumber,
                    BlockTimestamp = (int)erc20Contract.BlockTimestamp,
                    DeployerAddress = erc20Contract.DeployerAddress,
                    IndexingMessageType = EthereumSamurai.Models.Messages.IndexingMessageType.ErcContract,
                    TokenDecimals = erc20Contract.TokenDecimals,
                    TokenName = erc20Contract.TokenName,
                    TokenSymbol = erc20Contract.TokenSymbol,
                    TokenTotalSupply = erc20Contract.TokenTotalSupply.ToString(),
                    TransactionHash = erc20Contract.TransactionHash
                });
            }
        }

        private async Task<bool> TransactionExists(string transactionHash)
        {
            return await _transactionRepository.GetAsync(transactionHash) != null;
        }

        private static bool TryCallFunction<T>(Contract contract, string name, out T result)
        {
            result = default(T);

            try
            {
                result = contract.GetFunction(name).CallAsync<T>().Result;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}