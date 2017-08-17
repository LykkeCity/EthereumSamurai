using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EthereumSamurai.Core.Services;
using EthereumSamurai.Filters;
using EthereumSamurai.Models.Blockchain;
using EthereumSamurai.Models.Query;
using EthereumSamurai.Requests;
using EthereumSamurai.Responses;
using Microsoft.AspNetCore.Mvc;

namespace EthereumSamurai.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class InternalMessagesController : Controller
    {
        private readonly IInternalMessageService _internalMessageService;
        private readonly IMapper                 _mapper;

        public InternalMessagesController(
            IInternalMessageService internalMessageService,
            IMapper mapper)
        {
            _internalMessageService = internalMessageService;
            _mapper                 = mapper;
        }

        [Route("txHash/{transactionHash}")]
        [HttpGet]
        [ProducesResponseType(typeof(FilteredInternalMessageResponse), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        [ProducesResponseType(typeof(ApiException), 500)]
        public async Task<IActionResult> GetForAddress([FromRoute] string transactionHash)
        {
            var messages = (await _internalMessageService.GetAsync(transactionHash)).ToList();

            return ProcessResponse(messages);
        }

        [Route("{address}")]
        [HttpGet]
        [ProducesResponseType(typeof(FilteredInternalMessageResponse), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        [ProducesResponseType(typeof(ApiException), 500)]
        public async Task<IActionResult> GetForAddress(GetAddressInternalMessageHistoryRequest request)
        {
            var address = request.Address.ToLower();
            var transactionQuery = new InternalMessageQuery
            {
                FromAddress = address,
                ToAddress   = address,
                StartBlock  = request.StartBlock,
                StopBlock   = request.StopBlock,
                Start       = request.Start,
                Count       = request.Count
            };

            var messages = (await _internalMessageService.GetAsync(transactionQuery)).ToList();

            return ProcessResponse(messages);
        }

        private IActionResult ProcessResponse(IEnumerable<InternalMessageModel> messages)
        {
            return new JsonResult(new FilteredInternalMessageResponse
            {
                Messages = messages.Select(_mapper.Map<InternalMessageResponse>).ToList()
            });
        }
    }
}