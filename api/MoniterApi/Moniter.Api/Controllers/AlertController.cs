using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moniter.Api.Hub;
using Moniter.Features.Alert;
using Moniter.Features.Parser;
using Moniter.Features.Users;
using Moniter.Infrastructure.Security;
using Moniter.Models;
using Newtonsoft.Json;
using Alert = Moniter.Features.Alert.Alert;

namespace Moniter.Api.Controllers
{
    [Route("alerts")]
    [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
    public class AlertController : Controller
    {
        private readonly IMediator _mediator;

        public AlertController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<List<Alert>> Alerts()
        {
            var alerts = await _mediator.Send(new FetchAlert.Command());
            if (!alerts.Any())
            {
                alerts = new List<Alert>();
            }

            return alerts;
        }
    }
}