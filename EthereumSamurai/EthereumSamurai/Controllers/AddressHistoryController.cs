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
    public class AddressHistoryController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IAddressHistoryService _addressHistoryService;

        public AddressHistoryController(IAddressHistoryService addressHistoryService, IMapper mapper)
        {
            _addressHistoryService = addressHistoryService;
            _mapper = mapper;
        }

        [Route("{address}")]
        [HttpGet]
        [ProducesResponseType(typeof(FilteredAddressHistoryResponse), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        [ProducesResponseType(typeof(ApiException), 500)]
        public async Task<IActionResult> GetForAddress(GetAddressInternalMessageHistoryRequest request)
        {
            string address = request.address.ToLower();
            var transactionQuery = new AddressHistoryQuery()
            {
                FromAddress = address,
                ToAddress = address,
                StartBlock = request.StartBlock,
                StopBlock = request.StopBlock,
                Start = request.Start,
                Count = request.Count
            };

            List<AddressHistoryModel> messages = (await _addressHistoryService.GetAsync(transactionQuery)).ToList();
           
            return ProcessResponse(messages);
        }

        private IActionResult ProcessResponse(List<AddressHistoryModel> messages)
        {
            List<AddressHistoryResponse> response = new List<AddressHistoryResponse>(messages.Count);
            messages.ForEach(transaction =>
            {
                AddressHistoryResponse trResponse = _mapper.Map<AddressHistoryResponse>(transaction);
                response.Add(trResponse);
            });

            return new JsonResult(new FilteredAddressHistoryResponse()
            {
                History = response
            });
        }
    }
}
