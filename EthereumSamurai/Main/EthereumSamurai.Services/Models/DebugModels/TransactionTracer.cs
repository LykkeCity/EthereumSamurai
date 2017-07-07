using EthereumSamurai.Core;
using EthereumSamurai.Models.DebugModels;
using Nethereum.Hex.HexTypes;
using Nethereum.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EthereumSamurai.Services.Models.DebugModels
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

        public string FromAddress { get; private set; }
        public string ToAddress { get; private set; }
        public string TransactionHash { get; private set; }
        public string Error { get; private set; }
        public TransferValueType Type { get; private set; }
        public List<CallStackFrame> Stack { get; private set; }

        public TransactionTracer(string from, string transactionHash, string toAddress, BigInteger value)
        {
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

            List<TransferValue> transfers = new List<TransferValue>();
            if (value != 0)
            {
                transfers.Add(new TransferValue()
                {
                    Depth = 0,
                    TransactionHash = transactionHash,
                    FromAddress = from,
                    ToAddress = toAddress,
                    Value = value,
                    Type = this.Type
                });
            }

            Stack.Add(new CallStackFrame()
            {
                Address = ToAddress,
                Transfers = transfers.ToArray()
            });
        }

        public async Task ProcessLog(StructLogItem structLog)
        {
            if (structLog.Error != null)
            {
                this.Stack.RemoveAt(this.Stack.Count - 1);
                if (this.Stack.Count == 0)
                {
                    this.Error = structLog.Error?.ToString();
                }
                return;
            }

            if (structLog.Depth == this.Stack.Count - 1)
            {
                var returnFrame = this.Stack.Last();
                this.Stack.RemoveAt(this.Stack.Count - 1);
                var topFrame = this.Stack.Last();

                if (topFrame.OpCode == OpCodes.CREATE)
                {
                    // Now we know our new address, fill it in everywhere.
                    topFrame.Address = structLog.Stack.Last();
                    await Fixup(returnFrame.Transfers, topFrame.Address);
                }

                var allTransfers = new TransferValue[topFrame.Transfers.Length + returnFrame.Transfers.Length];
                topFrame.Transfers.CopyTo(allTransfers, 0);
                returnFrame.Transfers.CopyTo(allTransfers, topFrame.Transfers.Length);
                // Our call succeded, so add any transfers that happened to the current stack frame
                topFrame.Transfers = allTransfers;
            }
            else if (structLog.Depth != this.Stack.Count)
            {
                throw new Exception($"Unexpected stack transition: was {this.Stack.Count}, now {structLog.Depth}");
            }

            switch (structLog.Opcode)
            {
                case OpCodes.CREATE:
                    {
                        // CREATE adds a frame to the stack, but we don't know their address yet - we'll fill it in
                        // when the call returns.
                        var value = new HexBigInteger(structLog.Stack.Last());
                        var src = this.Stack.Last().Address;

                        TransferValue[] transfers = null;
                        if (value.Value != 0)
                        {
                            transfers = new TransferValue[]{ new TransferValue(){
                            Depth = this.Stack.Count,
                            ToAddress = Constants.EmptyAddress,
                            Value = value.Value,
                            FromAddress = FromAddress,
                            TransactionHash = TransactionHash,
                            Type = TransferValueType.CREATION
                        }};
                        }

                        var frame = new CallStackFrame()
                        {
                            Address = Constants.EmptyAddress,
                            OpCode = structLog.Opcode,
                            Transfers = transfers ?? new TransferValue[] { }
                        };

                        this.Stack.Add(frame);
                    }
                    break;

                case OpCodes.CALL:
                    {
                        // CALL adds a frame to the stack with the target address and value
                        var value = new HexBigInteger(structLog.Stack[structLog.Stack.Count - 3]);
                        var dest = _addressUtil.ConvertToChecksumAddress(structLog.Stack[structLog.Stack.Count - 2].TrimStart(new char[] { '0' }));

                        TransferValue[] transfers = null;
                        if (value.Value != 0)
                        {
                            var src = this.Stack.Last().Address;
                            transfers = new TransferValue[]{ new TransferValue(){
                            Depth = this.Stack.Count,
                            ToAddress = dest,
                            Value = value.Value,
                            FromAddress = src,
                            TransactionHash = TransactionHash,
                            Type = TransferValueType.TRANSFER
                        }};
                        }

                        var frame = new CallStackFrame()
                        {
                            Address = dest,
                            OpCode = structLog.Opcode,
                            Transfers = transfers ?? new TransferValue[] { }
                        };

                        this.Stack.Add(frame);
                    }
                    break;

                case OpCodes.CALLCODE:
                case OpCodes.DELEGATECALL:
                    // CALLCODE and DELEGATECALL don't transfer value or change the from address, but do create
                    // a separate failure domain.
                    {
                        var frame = new CallStackFrame()
                        {
                            Address = this.Stack.Last().Address,
                            OpCode = structLog.Opcode,
                        };

                        this.Stack.Add(frame);
                    }
                    break;

                case OpCodes.SUICIDE:
                    // SUICIDE results in a transfer back to the calling address.
                    {
                    }
                    break;

                default:
                    break;
            }
        }

        public TraceResult BuildResult()
        {
            var stackResult = Stack.FirstOrDefault();

            return new TraceResult()
            {
                HasError = !string.IsNullOrEmpty(Error),
                Transfers = stackResult?.Transfers
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
    }
}
