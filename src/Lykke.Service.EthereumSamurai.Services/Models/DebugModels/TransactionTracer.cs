using Lykke.Service.EthereumSamurai.Core;
using Lykke.Service.EthereumSamurai.Models.DebugModels;
using Nethereum.Hex.HexTypes;
using Nethereum.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumSamurai.Services.Models.DebugModels
{
    public class TransferValue
    {
        public int Depth { get; set; }
        public string TransactionHash { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public BigInteger Value { get; set; }
        public TransferValueType Type { get; set; }
    }

    public class CallStackFrame
    {
        public string OpCode { get; set; }
        public string Address { get; set; }
        public TransferValue[] Transfers { get; set; }
    }

    public class TraceResult
    {
        public bool HasError { get; set; }

        public IEnumerable<TransferValue> Transfers { get; set; }
    }

    public class TransactionTracer
    {
        private AddressUtil _addressUtil;
        private TreeNode<CallStackFrame> _callTree;
        private TreeNode<CallStackFrame> _currentNode;
        private int _currentDepth = 0;
        private List<TransferValue> _allTransfers;

        public string FromAddress { get; private set; }
        public string ToAddress { get; private set; }
        public string TransactionHash { get; private set; }
        public string Error { get; private set; }
        public TransferValueType Type { get; private set; }
        public List<CallStackFrame> Stack { get; private set; }
        public string ContractAddress { get; private set; }

        public TransactionTracer(string from, string transactionHash, string toAddress, string contractAddress, BigInteger value)
        {
            _allTransfers = new List<TransferValue>();
            ContractAddress = contractAddress;
            _addressUtil = new AddressUtil();
            TransactionHash = transactionHash;
            Stack = new List<CallStackFrame>();
            FromAddress = from;
            ToAddress = Constants.EmptyAddress;
            Type = TransferValueType.CREATION;
            if (toAddress != null)
            {
                ToAddress = toAddress;
                Type = TransferValueType.TRANSACTION;
            }

            string destinationAddress = toAddress ?? ContractAddress ?? Constants.EmptyAddress;
            List<TransferValue> transfers = new List<TransferValue>();
            if (value != 0)
            {
                transfers.Add(new TransferValue()
                {
                    Depth = 0,
                    TransactionHash = transactionHash,
                    FromAddress = from,
                    ToAddress = destinationAddress,
                    Value = value,
                    Type = this.Type
                });
            }

            var initialCallStack = new CallStackFrame()
            {
                Address = destinationAddress,
                OpCode = Type == TransferValueType.CREATION ? OpCodes.CREATE : null,
                Transfers = transfers.ToArray()
            };

            Stack.Add(initialCallStack);

            _callTree = new TreeNode<CallStackFrame>(initialCallStack, 0);
            _currentNode = _callTree;
        }

        public async Task ProcessLog(StructLogItem structLog)
        {
            _currentDepth = structLog.Depth;
            CallStackFrame addedOnCurrentProcessing = null;
            if (structLog.Error != null)
            {
                this.Error = structLog.Error?.ToString();

                return;
            }

            if (structLog.Depth == _currentNode.Depth)
            {
                var returnFrame = _currentNode.Data;
                _currentNode = _currentNode.Parent;
                var topFrame = _currentNode.Data;

                if ((topFrame.OpCode == OpCodes.CREATE || returnFrame.OpCode == OpCodes.CREATE))
                {
                    string lastAddress = structLog.Stack.Last();
                    returnFrame.Address = FormatAddress(lastAddress);
                    if (returnFrame.Transfers != null && returnFrame.Transfers.Count() != 0)
                    {
                        await Fixup(returnFrame.Transfers, returnFrame.Address);
                    }
                }
            }

            switch (structLog.Opcode)
            {
                case OpCodes.CREATE:
                    {
                        var value = new HexBigInteger(structLog.Stack.Last());
                        string src = _currentDepth - 1 == _currentNode.Depth ? _currentNode.Data.Address : _currentNode.Parent?.Data.Address;

                        TransferValue[] transfers = null;
                        //if (value.Value != 0)
                        //{
                        transfers = new TransferValue[]{ new TransferValue(){
                            Depth = structLog.Depth,
                            ToAddress = Constants.EmptyAddress,
                            Value = value.Value,
                            FromAddress = src,
                            TransactionHash = TransactionHash,
                            Type = TransferValueType.CREATION
                        }};
                        //}

                        addedOnCurrentProcessing = new CallStackFrame()
                        {
                            Address = Constants.EmptyAddress,
                            OpCode = structLog.Opcode,
                            Transfers = transfers ?? new TransferValue[] { }
                        };
                    }
                    break;

                case OpCodes.CALL:
                    {
                        var value = new HexBigInteger(structLog.Stack[structLog.Stack.Count - 3]);
                        var dest = FormatAddress(structLog.Stack[structLog.Stack.Count - 2]);
                        string src = _currentNode.Data.Address;

                        TransferValue[] transfers = null;
                        if (value.Value != 0)
                        {
                            transfers = new TransferValue[]{ new TransferValue(){
                            Depth = structLog.Depth,//this.Stack.Count,
                            ToAddress = dest,
                            Value = value.Value,
                            FromAddress = src,
                            TransactionHash = TransactionHash,
                            Type = TransferValueType.TRANSFER
                        }};
                        }

                        addedOnCurrentProcessing = new CallStackFrame()
                        {
                            Address = dest,
                            OpCode = structLog.Opcode,
                            Transfers = transfers ?? new TransferValue[] { }
                        };
                    }
                    break;

                case OpCodes.CALLCODE:
                case OpCodes.DELEGATECALL:
                    {
                        string src = _currentNode.Data.Address;
                        addedOnCurrentProcessing = new CallStackFrame()
                        {
                            Address = src,
                            OpCode = structLog.Opcode,
                        };
                    }
                    break;

                case OpCodes.SUICIDE:
                    // SUICIDE results in a transfer back to the calling address.
                    //TODO: Find a way to calculate such TransferValue
                    {
                    }
                    break;

                default:
                    break;
            }

            if (addedOnCurrentProcessing != null)
            {
                this.Stack.Add(addedOnCurrentProcessing);
                TreeNode<CallStackFrame> newNode = new TreeNode<CallStackFrame>(addedOnCurrentProcessing, _currentDepth);
                if (_currentDepth == _currentNode.Depth)
                {
                    _currentNode.Parent.AddChild(newNode);
                }
                else
                {
                    _currentNode.AddChild(newNode);
                }

                _currentNode = newNode;
            }
        }

        public TraceResult BuildResult()
        {
            var transfers = new List<TransferValue>();
            _callTree.RecursivelyProcessAllNodes((data) =>
            {
                if (data?.Transfers != null)
                {
                    transfers.AddRange(data.Transfers);
                }
            });
            return new TraceResult()
            {
                HasError = !string.IsNullOrEmpty(Error),
                Transfers = transfers
            };
        }

        private async Task Fixup(TransferValue[] transfers, string address)
        {
            for (int i = 0; i < transfers.Count(); i++)
            {
                var transfer = transfers[i];
                if (transfer.FromAddress == Constants.EmptyAddress)
                {
                    transfer.FromAddress = address;
                }
                else if (transfer.ToAddress == Constants.EmptyAddress)
                {
                    transfer.ToAddress = address;
                }
            }
        }

        private string FormatAddress(string address)
        {
            string trimmed = address.TrimStart(new char[] { '0' });
            string formated = _addressUtil.ConvertToChecksumAddress(trimmed).ToLower();

            return formated;
        }
    }
}
