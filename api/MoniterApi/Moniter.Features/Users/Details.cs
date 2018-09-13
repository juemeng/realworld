using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moniter.Infrastructure;
using Moniter.Infrastructure.Errors;
using Moniter.Infrastructure.Security;

namespace Moniter.Features.Users
{
    public class Details
    {
        public class Query : IRequest<UserEnvelope>
        {
            public string Username { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.Username).NotNull().NotEmpty();
            }
        }

        public class QueryHandler : IRequestHandler<Query, UserEnvelope>
        {
            private readonly MoniterContext _context;
            private readonly IJwtTokenGenerator _jwtTokenGenerator;
            private readonly IMapper _mapper;

            public QueryHandler(MoniterContext context, IJwtTokenGenerator jwtTokenGenerator, IMapper mapper)
            {
                _context = context;
                _jwtTokenGenerator = jwtTokenGenerator;
                _mapper = mapper;
            }

            public async Task<UserEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Username == message.Username, cancellationToken);
                if (user == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { User = ErrorMessages.UserNotFound});
                }
                var newUser = _mapper.Map<Domain.User, User>(user);
                newUser.Token = await _jwtTokenGenerator.CreateToken(user.Username);
                return new UserEnvelope(newUser);
            }
        }
    }
}