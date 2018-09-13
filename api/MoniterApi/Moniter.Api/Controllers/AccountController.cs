using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moniter.Features.Users;

namespace Moniter.Api.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<UserEnvelope> Create([FromBody] Create.Command command)
        {
            return await _mediator.Send(command);
        }


        [HttpPost("login")]
        public async Task<UserEnvelope> Login([FromBody] Login.Command command)
        {
            return await _mediator.Send(command);
        }

        public string Test()
        {
            return "aaa";
        }
    }
}