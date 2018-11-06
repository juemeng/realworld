using System;
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
    public class Create
    {
        public class UserData
        {
            public string Username { get; set; }

            public string Email { get; set; }

            public string Password { get; set; }
            
            public string ConfirmPassword { get; set; }
        }

        public class UserDataValidator : AbstractValidator<UserData>
        {
            public UserDataValidator()
            {
                RuleFor(x => x.Username).NotNull().WithMessage(ErrorMessages.UserNameCannotBeNull);
                RuleFor(x => x.Email).NotNull().WithMessage(ErrorMessages.EmailCannotBeNull).EmailAddress().WithMessage(ErrorMessages.EmailFormatError);
                RuleFor(x => x.Password).NotNull().WithMessage(ErrorMessages.PasswordCannotBeNull).MinimumLength(6)
                    .WithMessage(ErrorMessages.PasswordLengthGreaterThan).MaximumLength(16).WithMessage(ErrorMessages.PasswordLengthLessThan);
                RuleFor(x => x.ConfirmPassword).NotNull().NotEmpty().MinimumLength(6).Must((x,y)=>x.Password == y);
            }
        }

        public class Command : IRequest<UserEnvelope>
        {
            public UserData User { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.User).NotNull().WithMessage(ErrorMessages.UserCastFailure).SetValidator(new UserDataValidator());
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
                if (await _context.Users.Where(x => x.Username == message.User.Username).AnyAsync(cancellationToken))
                {
                    throw new RestException(HttpStatusCode.BadRequest, new { Username = ErrorMessages.UserNameInUse});
                }

                if (await _context.Users.Where(x => x.Email == message.User.Email).AnyAsync(cancellationToken))
                {
                    throw new RestException(HttpStatusCode.BadRequest, new { Email = ErrorMessages.EmailInUse });
                }

                var salt = Guid.NewGuid().ToByteArray();
                var newUser = new Models.User
                {
                    Username = message.User.Username,
                    Email = message.User.Email,
                    Role = UserRole.User,
                    Hash = _passwordHasher.Hash(message.User.Password, salt),
                    Salt = salt
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync(cancellationToken);
                var user = _mapper.Map<Models.User, User>(newUser);
                user.Token = await _jwtTokenGenerator.CreateToken(newUser.Username);
                return new UserEnvelope(user);
            }
        }
    }
}