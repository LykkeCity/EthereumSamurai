using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EthereumSamurai.Core.Services;
using EthereumSamurai.Models.Query;
using EthereumSamurai.Models.Blockchain;
using AutoMapper;
using EthereumSamurai.Responses;
using EthereumSamurai.Requests;
using EthereumSamurai.Filters;

namespace EthereumSamurai.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly IMapper _mapper;

        public TransactionController(ITransactionService transactionService, IMapper mapper)
        {
            _transactionService = transactionService;
            _mapper = mapper;
        }

        [Route("{address}")]
        [HttpGet]
        [ProducesResponseType(typeof(FilteredTransactionsResponse), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        [ProducesResponseType(typeof(ApiException), 500)]
        public async Task<IActionResult> GetForAddress(GetAddressHistoryRequest request)
        {
            var transactionQuery = new TransactionQuery()
            {
                FromAddress = request.Address,
                ToAddress = request.Address,
                Start = request.Start,
                Count = request.Count
            };

            List<TransactionModel> transactions = (await _transactionService.GetAsync(transactionQuery)).ToList();
           
            return ProcessResponse(transactions);
        }

        [Route("block/number/{blockNumber}")]
        [HttpGet]
        [ProducesResponseType(typeof(FilteredTransactionsResponse), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        [ProducesResponseType(typeof(ApiException), 500)]
        public async Task<IActionResult> GetForBlockNumber([FromRoute] ulong blockNumber)
        {
            List<TransactionModel> transactions = (await _transactionService.GetForBlockNumberAsync(blockNumber)).ToList();

            return ProcessResponse(transactions);
        }

        [Route("block/hash/{blockHash}")]
        [HttpGet]
        [ProducesResponseType(typeof(FilteredTransactionsResponse), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        [ProducesResponseType(typeof(ApiException), 500)]
        public async Task<IActionResult> GetForBlockNumber([FromRoute] string blockHash)
        {
            List<TransactionModel> transactions = (await _transactionService.GetForBlockHashAsync(blockHash)).ToList();

            return ProcessResponse(transactions);
        }

        private IActionResult ProcessResponse(List<TransactionModel> transactions)
        {
            List<TransactionResponse> response = new List<TransactionResponse>(transactions.Count);
            transactions.ForEach(transaction =>
            {
                TransactionResponse trResponse = _mapper.Map<TransactionResponse>(transaction);
                response.Add(trResponse);
            });

            return new JsonResult(new FilteredTransactionsResponse()
            {
                Transactions = response
            });
        }
    }
}
