//using System;
//using System.Collections.Generic;
//using System.Numerics;
//using System.Runtime.Serialization;
//using System.Text;

//namespace Lykke.Service.EthereumSamurai.Models.DebugModels
//{
//    [DataContract]
//    public class TransactionTrace
//    {
//        [DataMember(Name = "gas")]
//        public BigInteger Gas { get; set; }

//        [DataMember(Name = "returnValue")]
//        public string ReturnValue { get; set; }

//        [DataMember(Name = "structLogs")]
//        public IEnumerable<StructLogItem> StructLogs { get; set; }
//    }

//    [DataContract]
//    public class StructLogItem
//    {
//        [DataMember(Name = "op")]
//        public string Opcode { get; set; }
//        [DataMember(Name = "gas")]
//        public BigInteger Gas { get; set; }
//        [DataMember(Name = "gasCost")]
//        public BigInteger GasCost { get; set; }
//        [DataMember(Name = "depth")]
//        public ulong Depth { get; set; }
//        [DataMember(Name = "error")]
//        public string Error { get; set; }
//        [DataMember(Name = "stack")]
//        public object Stack { get; set; }
//        [DataMember(Name = "memory")]
//        public object Memory { get; set; }
//        [DataMember(Name = "storage")]
//        public object Storage { get; set; }
//    }
//    /*
//    {
//  "gas": 173026,
//  "returnValue": "",
//  "structLogs": [
//    {
//      "pc": 0,
//      "op": "PUSH1",
//      "gas": 4672133,
//      "gasCost": 3,
//      "depth": 1,
//      "error": null,
//      "stack": [],
//      "memory": null,
//      "storage": {}
//    },
//    */
//}
