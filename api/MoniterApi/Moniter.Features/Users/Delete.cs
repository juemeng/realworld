using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Moniter.Infrastructure;
using Moniter.Infrastructure.Errors;

namespace Moniter.Features.Users
{
    public class Delete
    {
        public class Command : IRequest
        {
            public Command(Guid id)
            {
                Id = id;
            }

            public Guid Id { get; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Id).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly MoniterContext _context;

            public Handler(MoniterContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(Command message, CancellationToken cancellationToken)
            {
                var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == message.Id, cancellationToken);

                if (user == null)
                {
                    throw new RestException(HttpStatusCode.NotFound, new { User = ErrorMessages.UserNotFound });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
    
    public class List
    {
        public class Query : IRequest<UsersEnvelope>
        {
            public Func<Models.User, bool> Filter { get; set; } = user => true;
        }

        public class QueryHandler : IRequestHandler<Query, UsersEnvelope>
        {
            private readonly MoniterContext _context;
            private readonly IMapper _mapper;

            public QueryHandler(MoniterContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public Task<UsersEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                var usersQuery = _context.Users.Where(message.Filter);
                var users = usersQuery.Select(u => _mapper.Map<Models.User, User>(u));
                return Task.FromResult(new UsersEnvelope(users.ToList()));
            }
        }
    }
}