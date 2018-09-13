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

namespace Moniter.Features.Users
{
    public class Login
    {
        public class LoginData
        {
            public string UserName { get; set; }

            public string Password { get; set; }
        }
    
        public class LoginDataValidator : AbstractValidator<LoginData>
        {
            public LoginDataValidator()
            {
                RuleFor(x => x.UserName).NotNull().NotEmpty();
                RuleFor(x => x.Password).NotNull().NotEmpty();
            }
        }
        
        public class Command : IRequest<UserEnvelope>
        {
            public LoginData User { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.User).NotNull().SetValidator(new LoginDataValidator());
            }
        }

        public class Handler : IRequestHandler<Command, UserEnvelope>
        {
            private readonly MoniterContext _context;
            private readonly IPasswordHasher _passwordHasher;
            private readonly IJwtTokenGenerator _jwtTokenGenerator;
            private readonly IMapper _mapper;

            public Handler(MoniterContext context, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator, IMapper mapper)
            {
                _context = context;
                _passwordHasher = passwordHasher;
                _jwtTokenGenerator = jwtTokenGenerator;
                _mapper = mapper;
            }

            public async Task<UserEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                var user = await _context.Users.Where(x => x.Username == message.User.UserName).SingleOrDefaultAsync(cancellationToken);
                if (user == null)
                {
                    throw new RestException(HttpStatusCode.Unauthorized, new { Error = "Invalid username / password." });
                }

                if (!user.Hash.SequenceEqual(_passwordHasher.Hash(message.User.Password, user.Salt)))
                {
                    throw new RestException(HttpStatusCode.Unauthorized, new { Error = "Invalid email / password." });
                }
             
                var newUser  = _mapper.Map<Domain.User, User>(user);
                newUser.Token = await _jwtTokenGenerator.CreateToken(user.Username);
                return new UserEnvelope(newUser);
            }
        }
    }
}