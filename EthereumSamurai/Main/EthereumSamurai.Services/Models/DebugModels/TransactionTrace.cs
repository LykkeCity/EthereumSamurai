using Nethereum.Hex.HexTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;

namespace EthereumSamurai.Models.DebugModels
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
