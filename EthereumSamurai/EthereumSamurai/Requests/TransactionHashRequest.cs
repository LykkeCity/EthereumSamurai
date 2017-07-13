﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace EthereumSamurai.Requests
{
    [DataContract]
    public class TransactionHashRequest
    {
        [FromRoute]
        [DataMember(Name = "transactionHash")]
        public string TransactionHash { get; set; }
    }
}