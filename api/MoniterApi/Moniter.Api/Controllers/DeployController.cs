using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Moniter.Features.Alert;
using Moniter.Features.Deploy;
using Moniter.Infrastructure.Security;

namespace Moniter.Api.Controllers
{
    [Route("deploy")]
//    [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
    public class DeployController : Controller
    {
        private readonly IMediator _mediator;

        public DeployController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<DeployResult> Deploy([FromBody] Deploy.Command command)
        {
            command.DeployInfo.MasterId = Guid.NewGuid();
            return await _mediator.Send(command);
        }

        [HttpGet("bindings")]
        public async Task<List<Bindings>> Bindings()
        {
            return await _mediator.Send(new FetchBindings.Command());
        }
    }
}