using System.Linq;
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
using Moniter.Models;

namespace Moniter.Features.Users
{
    public class Details
    {
        public class Query : IRequest<LoginUser>
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

        public class QueryHandler : IRequestHandler<Query, LoginUser>
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

            public async Task<LoginUser> Handle(Query message, CancellationToken cancellationToken)
            {
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Username == message.Username, cancellationToken);
                if (user == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { User = ErrorMessages.UserNotFound});
                }

                var noticeCount = await _context.Alerts.CountAsync(x => x.Status == AlertStatus.New, cancellationToken);
                
                var newUser = _mapper.Map<Models.User, LoginUser>(user);
                newUser.NoticeCount = noticeCount;
                return newUser;
            }
        }
    }
}