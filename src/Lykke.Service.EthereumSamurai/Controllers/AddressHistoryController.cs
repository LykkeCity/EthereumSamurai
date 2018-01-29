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
    public class AddressHistoryController : Controller
    {
        private readonly IAddressHistoryService _addressHistoryService;
        private readonly IMapper _mapper;

        public AddressHistoryController(
            IAddressHistoryService addressHistoryService,
            IMapper mapper)
        {
            _addressHistoryService = addressHistoryService;
            _mapper                = mapper;
        }

        [Route("{address}")]
        [HttpGet]
        [ProducesResponseType(typeof(FilteredAddressHistoryResponse), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        [ProducesResponseType(typeof(ApiException), 500)]
        public async Task<IActionResult> GetForAddress(GetAddressInternalMessageHistoryRequest request)
        {
            var address          = request.Address.ToLowerInvariant();
            var transactionQuery = new AddressHistoryQuery
            {
                FromAddress = address,
                ToAddress   = address,
                StartBlock  = request.StartBlock,
                StopBlock   = request.StopBlock,
                Start       = request.Start,
                Count       = request.Count
            };

            var messages = (await _addressHistoryService.GetAsync(transactionQuery)).ToList();

            return ProcessResponse(messages);
        }

        private IActionResult ProcessResponse(IEnumerable<AddressHistoryModel> messages)
        {
            return new JsonResult(new FilteredAddressHistoryResponse
            {
                History = messages.Select(_mapper.Map<AddressHistoryResponse>).ToList()
            });
        }
    }
}