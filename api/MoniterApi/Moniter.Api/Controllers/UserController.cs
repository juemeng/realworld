using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moniter.Features.Users;
using Moniter.Infrastructure;
using Moniter.Infrastructure.Security;

namespace Moniter.Api.Controllers
{
    [Route("user")]
    [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserAccessor _currentUserAccessor;

        public UserController(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
        {
            _mediator = mediator;
            _currentUserAccessor = currentUserAccessor;
        }

        [HttpGet]
        public async Task<UserEnvelope> GetCurrent()
        {
            return await _mediator.Send(new Details.Query()
            {
                Username = _currentUserAccessor.GetCurrentUsername()
            });
        }

        [HttpPut]
        public async Task<UserEnvelope> UpdateUser([FromBody]Edit.Command command)
        {
            return await _mediator.Send(command);
        }
    }
}