using System;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moniter.Api.Hub;
using Moniter.Features.Alert;
using Moniter.Features.Parser;
using Moniter.Models;
using Newtonsoft.Json;

namespace Moniter.Api.Controllers
{
    [Route("devices")]
    public class DeviceController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IAlertMessageParser _messageParser;
        private readonly IMapper _mapper;
        private readonly IHubContext<DeviceHub> _hubContext;

        public DeviceController(IMediator mediator,IAlertMessageParser messageParser,IMapper mapper,IHubContext<DeviceHub> hubContext)
        {
            _mediator = mediator;
            _messageParser = messageParser;
            _mapper = mapper;
            _hubContext = hubContext;
        }
        
        [HttpPost("report")]
        public async Task<SaveResult> Create([FromBody] BufferData message)
        {
            var alertInfo = _messageParser.Parse(message);
            var command = _mapper.Map<AlertInfo, Save.Command>(alertInfo);
            
            var resultEnvelope = await _mediator.Send(command);
            if (resultEnvelope.Skipped)
            {
                return new SaveResult
                {
                    Status = SaveAlertStatus.Skipped,
                    Message = @"已忽略"
                };
            }
            if (resultEnvelope.AlertViewModel.Id != Guid.Empty)
            {
                await _hubContext.Clients.All.SendAsync("addNotice",JsonConvert.SerializeObject(resultEnvelope.AlertViewModel));
            }

            return resultEnvelope.Result;
        }
    }
}