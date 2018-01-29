using Lykke.Service.EthereumSamurai.Core;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Core.Services.Erc20;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.StandardTokenEIP20;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumSamurai.Services.Erc20
{
    /*
    contract ERC20Interface {
        event Transfer(address indexed from, address indexed to, uint256 value);
        event Approval(address indexed from, address indexed spender, uint256 value);

        function totalSupply() constant returns (uint256 supply);
        function balanceOf(address _owner) constant returns (uint256 balance);
        function transfer(address _to, uint256 _value) returns (bool success);
        function transferFrom(address _from, address _to, uint256 _value) returns (bool success);
        function approve(address _spender, uint256 _value) returns (bool success);
        function allowance(address _owner, address _spender) constant returns (uint256 remaining);
    }
    */

    public class Erc20Detector : IErc20Detector
    {
        private const string _notCompatibleContractResponse = "0x";
        private readonly Web3 _web3;

        public Erc20Detector(Web3 web3)
        {
            _web3 = web3;
        }

        public async Task<bool> IsContractErc20Compatible(string contractAddress)
        {
            bool isContractErc20Compatible = false;
            Contract contract = _web3.Eth.GetContract(Constants.Erc20ABI, contractAddress);
            //contract.Eth.Transactions.Call.SendRequestAsync(,);

            Function totalSupplyFunc = contract.GetFunction("totalSupply");
            Function balanceOfFunc = contract.GetFunction("balanceOf");
            Function transferFunc = contract.GetFunction("transfer");
            Function transferFromFunc = contract.GetFunction("transferFrom");
            Function approveFunc = contract.GetFunction("approve");
            Function allowanceFunc = contract.GetFunction("allowance");

            try
            {
                await CheckContractHasFunction(contract, totalSupplyFunc);
                await CheckContractHasFunction(contract, balanceOfFunc, contractAddress);
                await CheckContractHasFunction(contract, transferFunc, Constants.EmptyAddress, new BigInteger(1000));
                await CheckContractHasFunction(contract, transferFromFunc, Constants.EmptyAddress, Constants.EmptyAddress, new BigInteger(1000));
                await CheckContractHasFunction(contract, approveFunc, Constants.EmptyAddress, new BigInteger(1000));
                await CheckContractHasFunction(contract, allowanceFunc, Constants.EmptyAddress, Constants.EmptyAddress);

                isContractErc20Compatible = true;
            }
            catch (Exception e)
            {
            }

            return isContractErc20Compatible;
        }

        private async Task CheckContractHasFunction(Contract contract, Function function, params object[] parameters)
        {
            var callInput = function.CreateCallInput(parameters);
            var response = await contract.Eth.Transactions.Call.SendRequestAsync(callInput);

            ThrowOnNotCompatibleResponse(response);
        }

        private void ThrowOnNotCompatibleResponse(string response)
        {
            if (response == _notCompatibleContractResponse)
            {
                throw new Exception("Contract is not compatible with erc20 standard");
            }
        }
    }
}

