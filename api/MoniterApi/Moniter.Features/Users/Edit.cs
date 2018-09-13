using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moniter.Infrastructure;
using Moniter.Infrastructure.Security;

namespace Moniter.Features.Users
{
    public class Edit
    {
        public class UserData
        {
            public string Username { get; set; }

            public string Email { get; set; }

            public string Password { get; set; }

            public string Bio { get; set; }

            public string Image { get; set; }
        }

        public class Command : IRequest<UserEnvelope>
        {
            public UserData User { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.User).NotNull();
            }
        }

        public class Handler : IRequestHandler<Command, UserEnvelope>
        {
            private readonly MoniterContext _context;
            private readonly IPasswordHasher _passwordHasher;
            private readonly ICurrentUserAccessor _currentUserAccessor;
            private readonly IMapper _mapper;

            public Handler(MoniterContext context, IPasswordHasher passwordHasher, 
                ICurrentUserAccessor currentUserAccessor, IMapper mapper)
            {
                _context = context;
                _passwordHasher = passwordHasher;
                _currentUserAccessor = currentUserAccessor;
                _mapper = mapper;
            }

            public async Task<UserEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                var currentUsername = _currentUserAccessor.GetCurrentUsername();
                var user = await _context.Users.Where(x => x.Username == currentUsername).FirstOrDefaultAsync(cancellationToken);

                user.Username = message.User.Username ?? user.Username;
                user.Email = message.User.Email ?? user.Email;
                user.Bio = message.User.Bio ?? user.Bio;
                user.Image = message.User.Image ?? user.Image;

                if (!string.IsNullOrWhiteSpace(message.User.Password))
                {
                    var salt = Guid.NewGuid().ToByteArray();
                    user.Hash = _passwordHasher.Hash(message.User.Password, salt);
                    user.Salt = salt;
                }
                
                await _context.SaveChangesAsync(cancellationToken);

                return new UserEnvelope(_mapper.Map<Domain.User, User>(user));
            }
        }
    }
}