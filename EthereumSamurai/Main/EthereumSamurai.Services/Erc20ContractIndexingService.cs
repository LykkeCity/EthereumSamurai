﻿using System;
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
            IWeb3 web3)
        {
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

                if (TryCallFunction(ethereumContract, "name", out string name))
                {
                    erc20Contract.TokenName = name;
                }

                if (TryCallFunction(ethereumContract, "symbol", out string symbol))
                {
                    erc20Contract.TokenSymbol = symbol;
                }

                if (TryCallFunction(ethereumContract, "decimals", out uint decimals))
                {
                    erc20Contract.TokenDecimals = decimals;
                }

                if (TryCallFunction(ethereumContract, "totalSupply", out BigInteger totalSupply))
                {
                    erc20Contract.TokenTotalSupply = totalSupply;
                }

                await _erc20ContractRepository.SaveAsync(erc20Contract);
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