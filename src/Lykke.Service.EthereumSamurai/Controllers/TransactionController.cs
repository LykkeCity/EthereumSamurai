using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.EthereumSamurai.Core.Services;
using Lykke.Service.EthereumSamurai.Filters;
using Lykke.Service.EthereumSamurai.Models.Blockchain;
using Lykke.Service.EthereumSamurai.Models.Query;
using Lykke.Service.EthereumSamurai.Requests;
using Lykke.Service.EthereumSamurai.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EthereumSamurai.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TransactionController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ITransactionService _transactionService;

        public TransactionController(
            ITransactionService transactionService,
            IMapper mapper)
        {
            _transactionService = transactionService;
            _mapper = mapper;
        }

        [Route("txHash/{transactionHash}")]
        [HttpGet]
        [ProducesResponseType(typeof(TransactionFullInfoResponse), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        [ProducesResponseType(typeof(ApiException), 500)]
        public async Task<IActionResult> GetForAddress([FromRoute] string transactionHash)
        {
            var transaction = await _transactionService.GetFullInfoAsync(transactionHash);

            var trResponse = new TransactionFullInfoResponse()
            {
                Erc20Transfers = transaction?.Erc20Transfers.Select(x => _mapper.Map<Erc20TransferHistoryResponse>(x)),
                Transaction = _mapper.Map<TransactionResponse>(transaction.TransactionModel)
            };

            return new JsonResult(trResponse);
        }

        [Route("{address}")]
        [HttpGet]
        [ProducesResponseType(typeof(FilteredTransactionsResponse), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        [ProducesResponseType(typeof(ApiException), 500)]
        public async Task<IActionResult> GetForAddress(GetAddressHistoryRequest request)
        {
            var address = request.Address.ToLower();
            var transactionQuery = new TransactionQuery
            {
                FromAddress = address,
                ToAddress = address,
                Start = request.Start,
                Count = request.Count
            };

            var transactions = (await _transactionService.GetAsync(transactionQuery)).ToList();

            return ProcessResponse(transactions);
        }

        [Route("block/number/{blockNumber}")]
        [HttpGet]
        [ProducesResponseType(typeof(FilteredTransactionsResponse), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        [ProducesResponseType(typeof(ApiException), 500)]
        public async Task<IActionResult> GetForBlockNumber([FromRoute] ulong blockNumber)
        {
            var transactions = (await _transactionService.GetForBlockNumberAsync(blockNumber)).ToList();

            return ProcessResponse(transactions);
        }

        [Route("block/hash/{blockHash}")]
        [HttpGet]
        [ProducesResponseType(typeof(FilteredTransactionsResponse), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        [ProducesResponseType(typeof(ApiException), 500)]
        public async Task<IActionResult> GetForBlockNumber([FromRoute] string blockHash)
        {
            var transactions = (await _transactionService.GetForBlockHashAsync(blockHash)).ToList();

            return ProcessResponse(transactions);
        }

        private IActionResult ProcessResponse(IEnumerable<TransactionModel> transactions)
        {
            return new JsonResult(new FilteredTransactionsResponse
            {
                Transactions = transactions.Select(_mapper.Map<TransactionResponse>).ToList()
            });
        }
    }
}