using Nethereum.Hex.HexTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;

namespace Lykke.Service.EthereumSamurai.Models.DebugModels
{
    public class CustomConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(T) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //explicitly specify the concrete type we want to create
            return serializer.Deserialize<T>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }
    }

    #region GETH

    [DataContract]
    public class TransactionTrace
    {
        [DataMember(Name = "gas")]
        [JsonConverter(typeof(CustomConverter<BigInteger>))]
        public BigInteger Gas { get; set; }

        [DataMember(Name = "returnValue")]
        public string ReturnValue { get; set; }

        [DataMember(Name = "structLogs")]
        public IEnumerable<StructLogItem> StructLogs { get; set; }
    }

    [DataContract]
    public class TransactionTraceResponse
    {
        [DataMember(Name = "result")]
        public TransactionTrace TransactionTrace { get; set; }

    }

    [DataContract]
    public class StructLogItem
    {
        [DataMember(Name = "op")]
        public string Opcode { get; set; }
        [DataMember(Name = "gas")]
        [JsonConverter(typeof(CustomConverter<BigInteger>))]
        public BigInteger Gas { get; set; }
        [DataMember(Name = "gasCost")]
        [JsonConverter(typeof(CustomConverter<BigInteger>))]
        public BigInteger GasCost { get; set; }
        [DataMember(Name = "depth")]
        public int Depth { get; set; }
        [DataMember(Name = "error")]
        public object Error { get; set; }
        [DataMember(Name = "stack")]
        public List<string> Stack { get; set; }
        //[DataMember(Name = "memory")]
        //public IEnumerable<string>  Memory { get; set; }
        //[DataMember(Name = "storage")]
        //public Dictionary<string, string> Storage { get; set; }
    }

    #endregion

    #region PARITY

    /*
     {
       "id": 1,
       "jsonrpc": "2.0",
       "result": [
         {
           "action": {
             "callType": "call",
             "from": "0xaa7b131dc60b80d3cf5e59b5a21a666aa039c951",
             "gas": "0x0",
             "input": "0x",
             "to": "0xd40aba8166a212d6892125f079c33e6f5ca19814",
             "value": "0x4768d7effc3fbe"
           },
           "blockHash": "0x7eb25504e4c202cf3d62fd585d3e238f592c780cca82dacb2ed3cb5b38883add",
           "blockNumber": 3068185,
           "result": {
             "gasUsed": "0x0",
             "output": "0x"
           },
           "subtraces": 0,
           "traceAddress": [],
           "transactionHash": "0x07da28d752aba3b9dd7060005e554719c6205c8a3aea358599fc9b245c52f1f6",
           "transactionPosition": 0,
           "type": "call"
         },
         ...
       ]
     }

        {
	"jsonrpc": "2.0",
	"result": [
		{
			"action": {
				"callType": "call",
				"from": "0x58e56b022760ae027e9e72bd892e72b42bda500a",
				"gas": "0x6a7db8",
				"input": "0x",
				"to": "0xb71f086762ed05481acb28697a65ef9c7b8f0235",
				"value": "0x6f05b59d3b20000"
			},
			"blockHash": "0x0f9a81549b08f96c8fc5941b6c58bee1494edea7e2ae1f061e257f931797e147",
			"blockNumber": 1800021,
			"result": {
				"gasUsed": "0x14c3c",
				"output": "0x"
			},
			"subtraces": 2,
			"traceAddress": [],
			"transactionHash": "0x1f8d164fef4efb88160d55d59eaab311e0e61b47eaf7364b7dc3108aceb6aa30",
			"transactionPosition": 0,
			"type": "call"
		},
		{
			"action": {
				"callType": "call",
				"from": "0xb71f086762ed05481acb28697a65ef9c7b8f0235",
				"gas": "0x68cbcc",
				"input": "0x59f1286d000000000000000000000000b71f086762ed05481acb28697a65ef9c7b8f0235",
				"to": "0xa67f1ab8d8dadb02d92fcb1332bf5317d20b8352",
				"value": "0x0"
			},
			"blockHash": "0x0f9a81549b08f96c8fc5941b6c58bee1494edea7e2ae1f061e257f931797e147",
			"blockNumber": 1800021,
			"result": {
				"gasUsed": "0x23a",
				"output": "0x00000000000000000000000000000000000000000000000006f05b59d3b20000"
			},
			"subtraces": 0,
			"traceAddress": [
				0
			],
			"transactionHash": "0x1f8d164fef4efb88160d55d59eaab311e0e61b47eaf7364b7dc3108aceb6aa30",
			"transactionPosition": 0,
			"type": "call"
		},
		{
			"action": {
				"callType": "call",
				"from": "0xb71f086762ed05481acb28697a65ef9c7b8f0235",
				"gas": "0x68c39a",
				"input": "0x2e1a7d4d00000000000000000000000000000000000000000000000006f05b59d3b20000",
				"to": "0xa67f1ab8d8dadb02d92fcb1332bf5317d20b8352",
				"value": "0x0"
			},
			"blockHash": "0x0f9a81549b08f96c8fc5941b6c58bee1494edea7e2ae1f061e257f931797e147",
			"blockNumber": 1800021,
			"result": {
				"gasUsed": "0x13bb9",
				"output": "0x"
			},
			"subtraces": 1,
			"traceAddress": [
				1
			],
			"transactionHash": "0x1f8d164fef4efb88160d55d59eaab311e0e61b47eaf7364b7dc3108aceb6aa30",
			"transactionPosition": 0,
			"type": "call"
		},
		{
			"action": {
				"callType": "call",
				"from": "0xa67f1ab8d8dadb02d92fcb1332bf5317d20b8352",
				"gas": "0x670227",
				"input": "0x",
				"to": "0xb71f086762ed05481acb28697a65ef9c7b8f0235",
				"value": "0x6f05b59d3b20000"
			},
			"blockHash": "0x0f9a81549b08f96c8fc5941b6c58bee1494edea7e2ae1f061e257f931797e147",
			"blockNumber": 1800021,
			"result": {
				"gasUsed": "0x107d3",
				"output": "0x"
			},
			"subtraces": 2,
			"traceAddress": [
				1,
				0
			],
			"transactionHash": "0x1f8d164fef4efb88160d55d59eaab311e0e61b47eaf7364b7dc3108aceb6aa30",
			"transactionPosition": 0,
			"type": "call"
		},
		{
			"action": {
				"callType": "call",
				"from": "0xb71f086762ed05481acb28697a65ef9c7b8f0235",
				"gas": "0x655e29",
				"input": "0x59f1286d000000000000000000000000b71f086762ed05481acb28697a65ef9c7b8f0235",
				"to": "0xa67f1ab8d8dadb02d92fcb1332bf5317d20b8352",
				"value": "0x0"
			},
			"blockHash": "0x0f9a81549b08f96c8fc5941b6c58bee1494edea7e2ae1f061e257f931797e147",
			"blockNumber": 1800021,
			"result": {
				"gasUsed": "0x23a",
				"output": "0x00000000000000000000000000000000000000000000000006f05b59d3b20000"
			},
			"subtraces": 0,
			"traceAddress": [
				1,
				0,
				0
			],
			"transactionHash": "0x1f8d164fef4efb88160d55d59eaab311e0e61b47eaf7364b7dc3108aceb6aa30",
			"transactionPosition": 0,
			"type": "call"
		},
		{
			"action": {
				"callType": "call",
				"from": "0xb71f086762ed05481acb28697a65ef9c7b8f0235",
				"gas": "0x6555f8",
				"input": "0x2e1a7d4d00000000000000000000000000000000000000000000000006f05b59d3b20000",
				"to": "0xa67f1ab8d8dadb02d92fcb1332bf5317d20b8352",
				"value": "0x0"
			},
			"blockHash": "0x0f9a81549b08f96c8fc5941b6c58bee1494edea7e2ae1f061e257f931797e147",
			"blockNumber": 1800021,
			"result": {
				"gasUsed": "0xf750",
				"output": "0x"
			},
			"subtraces": 1,
			"traceAddress": [
				1,
				0,
				1
			],
			"transactionHash": "0x1f8d164fef4efb88160d55d59eaab311e0e61b47eaf7364b7dc3108aceb6aa30",
			"transactionPosition": 0,
			"type": "call"
		},
		{
			"action": {
				"callType": "call",
				"from": "0xa67f1ab8d8dadb02d92fcb1332bf5317d20b8352",
				"gas": "0x63a23c",
				"input": "0x",
				"to": "0xb71f086762ed05481acb28697a65ef9c7b8f0235",
				"value": "0x6f05b59d3b20000"
			},
			"blockHash": "0x0f9a81549b08f96c8fc5941b6c58bee1494edea7e2ae1f061e257f931797e147",
			"blockNumber": 1800021,
			"result": {
				"gasUsed": "0xc36a",
				"output": "0x"
			},
			"subtraces": 2,
			"traceAddress": [
				1,
				0,
				1,
				0
			],
			"transactionHash": "0x1f8d164fef4efb88160d55d59eaab311e0e61b47eaf7364b7dc3108aceb6aa30",
			"transactionPosition": 0,
			"type": "call"
		},
		{
			"action": {
				"callType": "call",
				"from": "0xb71f086762ed05481acb28697a65ef9c7b8f0235",
				"gas": "0x620bbe",
				"input": "0x59f1286d000000000000000000000000b71f086762ed05481acb28697a65ef9c7b8f0235",
				"to": "0xa67f1ab8d8dadb02d92fcb1332bf5317d20b8352",
				"value": "0x0"
			},
			"blockHash": "0x0f9a81549b08f96c8fc5941b6c58bee1494edea7e2ae1f061e257f931797e147",
			"blockNumber": 1800021,
			"result": {
				"gasUsed": "0x23a",
				"output": "0x00000000000000000000000000000000000000000000000006f05b59d3b20000"
			},
			"subtraces": 0,
			"traceAddress": [
				1,
				0,
				1,
				0,
				0
			],
			"transactionHash": "0x1f8d164fef4efb88160d55d59eaab311e0e61b47eaf7364b7dc3108aceb6aa30",
			"transactionPosition": 0,
			"type": "call"
		},
		{
			"action": {
				"callType": "call",
				"from": "0xb71f086762ed05481acb28697a65ef9c7b8f0235",
				"gas": "0x62038c",
				"input": "0x2e1a7d4d00000000000000000000000000000000000000000000000006f05b59d3b20000",
				"to": "0xa67f1ab8d8dadb02d92fcb1332bf5317d20b8352",
				"value": "0x0"
			},
			"blockHash": "0x0f9a81549b08f96c8fc5941b6c58bee1494edea7e2ae1f061e257f931797e147",
			"blockNumber": 1800021,
			"result": {
				"gasUsed": "0xb2e7",
				"output": "0x"
			},
			"subtraces": 1,
			"traceAddress": [
				1,
				0,
				1,
				0,
				1
			],
			"transactionHash": "0x1f8d164fef4efb88160d55d59eaab311e0e61b47eaf7364b7dc3108aceb6aa30",
			"transactionPosition": 0,
			"type": "call"
		},
		{
			"action": {
				"callType": "call",
				"from": "0xa67f1ab8d8dadb02d92fcb1332bf5317d20b8352",
				"gas": "0x605d19",
				"input": "0x",
				"to": "0xb71f086762ed05481acb28697a65ef9c7b8f0235",
				"value": "0x6f05b59d3b20000"
			},
			"blockHash": "0x0f9a81549b08f96c8fc5941b6c58bee1494edea7e2ae1f061e257f931797e147",
			"blockNumber": 1800021,
			"result": {
				"gasUsed": "0x4469",
				"output": "0x"
			},
			"subtraces": 2,
			"traceAddress": [
				1,
				0,
				1,
				0,
				1,
				0
			],
			"transactionHash": "0x1f8d164fef4efb88160d55d59eaab311e0e61b47eaf7364b7dc3108aceb6aa30",
			"transactionPosition": 0,
			"type": "call"
		},
		{
			"action": {
				"callType": "call",
				"from": "0xb71f086762ed05481acb28697a65ef9c7b8f0235",
				"gas": "0x5ed3af",
				"input": "0x59f1286d000000000000000000000000b71f086762ed05481acb28697a65ef9c7b8f0235",
				"to": "0xa67f1ab8d8dadb02d92fcb1332bf5317d20b8352",
				"value": "0x0"
			},
			"blockHash": "0x0f9a81549b08f96c8fc5941b6c58bee1494edea7e2ae1f061e257f931797e147",
			"blockNumber": 1800021,
			"result": {
				"gasUsed": "0x23a",
				"output": "0x00000000000000000000000000000000000000000000000006f05b59d3b20000"
			},
			"subtraces": 0,
			"traceAddress": [
				1,
				0,
				1,
				0,
				1,
				0,
				0
			],
			"transactionHash": "0x1f8d164fef4efb88160d55d59eaab311e0e61b47eaf7364b7dc3108aceb6aa30",
			"transactionPosition": 0,
			"type": "call"
		},
		{
			"action": {
				"callType": "call",
				"from": "0xb71f086762ed05481acb28697a65ef9c7b8f0235",
				"gas": "0x5ecb7e",
				"input": "0x2e1a7d4d00000000000000000000000000000000000000000000000006f05b59d3b20000",
				"to": "0xa67f1ab8d8dadb02d92fcb1332bf5317d20b8352",
				"value": "0x0"
			},
			"blockHash": "0x0f9a81549b08f96c8fc5941b6c58bee1494edea7e2ae1f061e257f931797e147",
			"blockNumber": 1800021,
			"result": {
				"gasUsed": "0x33e6",
				"output": "0x"
			},
			"subtraces": 0,
			"traceAddress": [
				1,
				0,
				1,
				0,
				1,
				0,
				1
			],
			"transactionHash": "0x1f8d164fef4efb88160d55d59eaab311e0e61b47eaf7364b7dc3108aceb6aa30",
			"transactionPosition": 0,
			"type": "call"
		}
	],
	"id": 1
}
    */


    //	"action": {
    //		"callType": "call",
    //		"from": "0x58e56b022760ae027e9e72bd892e72b42bda500a",
    //		"gas": "0x6a7db8",
    //		"input": "0x",
    //		"to": "0xb71f086762ed05481acb28697a65ef9c7b8f0235",
    //		"value": "0x6f05b59d3b20000"
    //	},
    //	"blockHash": "0x0f9a81549b08f96c8fc5941b6c58bee1494edea7e2ae1f061e257f931797e147",
    //	"blockNumber": 1800021,
    //	"result": {
    //		"gasUsed": "0x14c3c",
    //		"output": "0x"
    //	},
    //	"subtraces": 2,
    //	"traceAddress": [],
    //	"transactionHash": "0x1f8d164fef4efb88160d55d59eaab311e0e61b47eaf7364b7dc3108aceb6aa30",
    //	"transactionPosition": 0,
    //	"type": "call"
    //},
    [DataContract]
    public class ParityTransactionTrace
    {
        [DataMember(Name = "action")]
        public ParityTransactionAction Action { get; set; }

        [DataMember(Name = "subtraces")]
        public int Subtraces { get; set; }

        [DataMember(Name = "transactionHash")]
        public string TransactionHash { get; set; }

        [DataMember(Name = "transactionPosition")]
        public int TransactionPosition { get; set; }

        [DataMember(Name = "traceAddresses")]
        public int[] TraceAddresses { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "error")]
        public string Error { get; set; }

        [DataMember(Name = "blockNumber")]
        public ulong BlockNumber { get; set; }

        [DataMember(Name = "blockHash")]
        public string BlockHash { get; set; }

        [DataMember(Name = "result")]
        public ParityTransactionResult Result { get; set; }
    }

    [DataContract]
    public class ParityTransactionAction
    {
        [DataMember(Name = "callType")]
        public string CallType { get; set; }

        [DataMember(Name = "from")]
        public string From { get; set; }

        //[DataMember(Name = "input")]
        //public string Input { get; set; }

        [DataMember(Name = "to")]
        public string To { get; set; }

        [JsonConverter(typeof(CustomConverter<HexBigInteger>))]
        [DataMember(Name = "gas")]
        public HexBigInteger Gas { get; set; }

        [JsonConverter(typeof(CustomConverter<HexBigInteger>))]
        [DataMember(Name = "value")]
        public HexBigInteger Value { get; set; }
    }

    [DataContract]
    public class ParityTransactionResult
    {
        [JsonConverter(typeof(CustomConverter<HexBigInteger>))]
        [DataMember(Name = "gasUsed")]
        public HexBigInteger GasUsed { get; set; }

        [DataMember(Name = "output")]
        public string Output { get; set; }

        [DataMember(Name = "address")]
        public string Address { get; set; }
    }

    [DataContract]
    public class ParityTransactionTraceResponse
    {
        [DataMember(Name = "result")]
        public IEnumerable<ParityTransactionTrace> TransactionTrace { get; set; }

    }

    #endregion
    /*
    {
  "gas": 173026,
  "returnValue": "",
  "structLogs": [
    {
      "pc": 0,
      "op": "PUSH1",
      "gas": 4672133,
      "gasCost": 3,
      "depth": 1,
      "error": null,
      "stack": [],
      "memory": null,
      "storage": {}
    },
    */
}
