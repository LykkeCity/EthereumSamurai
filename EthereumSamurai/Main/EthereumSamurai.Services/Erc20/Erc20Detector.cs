using EthereumSamurai.Core;
using EthereumSamurai.Core.Services.Erc20;
using Nethereum.Contracts;
using Nethereum.Web3;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace EthereumSamurai.Services.Erc20
{
    /*
     * Erc20 contract signature:
     * 
     *    contract ERC20Interface {
     *       event Transfer(address indexed from, address indexed to, uint256 value);
     *       event Approval(address indexed from, address indexed spender, uint256 value);
     *    
     *       function totalSupply() constant returns (uint256 supply);
     *       function balanceOf(address _owner) constant returns (uint256 balance);
     *       function transfer(address _to, uint256 _value) returns (bool success);
     *       function transferFrom(address _from, address _to, uint256 _value) returns (bool success);
     *       function approve(address _spender, uint256 _value) returns (bool success);
     *       function allowance(address _owner, address _spender) constant returns (uint256 remaining);
     *    }
     *
     */

    public class Erc20Detector : IErc20Detector
    {
        private readonly Web3 _web3;


        public Erc20Detector(Web3 web3)
        {
            _web3 = web3;
        }


        public async Task<bool> IsContractErc20Compatible(string contractAddress)
        {
            var contract = _web3.Eth.GetContract(Constants.Erc20ABI, contractAddress);

            var totalSupplyFunc = contract.GetFunction("totalSupply");
            var balanceOfFunc = contract.GetFunction("balanceOf");
            var transferFunc = contract.GetFunction("transfer");
            var transferFromFunc = contract.GetFunction("transferFrom");
            var approveFunc = contract.GetFunction("approve");
            var allowanceFunc = contract.GetFunction("allowance");

            var contractHasFunctions = await Task.WhenAll
            (
                ContractHasFunction(contract, totalSupplyFunc),
                ContractHasFunction(contract, balanceOfFunc, contractAddress),
                ContractHasFunction(contract, transferFunc, Constants.EmptyAddress, new BigInteger(1000)),
                ContractHasFunction(contract, transferFromFunc, Constants.EmptyAddress, Constants.EmptyAddress, new BigInteger(1000)),
                ContractHasFunction(contract, approveFunc, Constants.EmptyAddress, new BigInteger(1000)),
                ContractHasFunction(contract, allowanceFunc, Constants.EmptyAddress, Constants.EmptyAddress)
            );
            
            return contractHasFunctions.All(x => x);
        }

        private static async Task<bool> ContractHasFunction(Contract contract, Function function, params object[] parameters)
        {
            var callInput = function.CreateCallInput(parameters);
            var response  = await contract.Eth.Transactions.Call.SendRequestAsync(callInput);

            return response != "0x"; // response is not not compatible response
        }
    }
}

