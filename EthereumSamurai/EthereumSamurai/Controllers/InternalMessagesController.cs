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
    public class InternalMessagesController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IInternalMessageService _internalMessageService;

        public InternalMessagesController(IInternalMessageService internalMessageService, IMapper mapper)
        {
            _internalMessageService = internalMessageService;
            _mapper = mapper;
        }

        [Route("txHash/{transactionHash}")]
        [HttpGet]
        [ProducesResponseType(typeof(FilteredInternalMessageResponse), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        [ProducesResponseType(typeof(ApiException), 500)]
        public async Task<IActionResult> GetForAddress([FromRoute] string transactionHash)
        {
            List<InternalMessageModel> messages = (await _internalMessageService.GetAsync(transactionHash)).ToList();

            return ProcessResponse(messages);
        }

        [Route("{address}")]
        [HttpGet]
        [ProducesResponseType(typeof(FilteredInternalMessageResponse), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        [ProducesResponseType(typeof(ApiException), 500)]
        public async Task<IActionResult> GetForAddress(GetAddressInternalMessageHistoryRequest request)
        {
            string address = request.address.ToLower();
            var transactionQuery = new InternalMessageQuery()
            {
                FromAddress = address,
                ToAddress = address,
                StartBlock = request.StartBlock,
                StopBlock = request.StopBlock,
                Start = request.Start,
                Count = request.Count
            };

            List<InternalMessageModel> messages = (await _internalMessageService.GetAsync(transactionQuery)).ToList();
           
            return ProcessResponse(messages);
        }

        private IActionResult ProcessResponse(List<InternalMessageModel> messages)
        {
            List<InternalMessageResponse> response = new List<InternalMessageResponse>(messages.Count);
            messages.ForEach(transaction =>
            {
                InternalMessageResponse trResponse = _mapper.Map<InternalMessageResponse>(transaction);
                response.Add(trResponse);
            });

            return new JsonResult(new FilteredInternalMessageResponse()
            {
                Messages = response
            });
        }
    }
}
