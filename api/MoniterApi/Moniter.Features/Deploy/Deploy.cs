using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moniter.Features.Users;
using Moniter.Infrastructure;
using Moniter.Infrastructure.Errors;
using Moniter.Infrastructure.Security;
using Moniter.Models;
using User = Moniter.Features.Users.User;

namespace Moniter.Features.Deploy
{
    public class Deploy
    {
        public class DeployInfoValidator : AbstractValidator<DeployInfo>
        {
            public DeployInfoValidator()
            {
                RuleFor(x => x.Host).NotNull().WithMessage(ErrorMessages.ServerHostCannotBeNull);
                RuleFor(x => x.UserName).NotNull().WithMessage(ErrorMessages.UserNameCannotBeNull);
                RuleFor(x => x.Password).NotNull().WithMessage(ErrorMessages.PasswordCannotBeNull);
                RuleFor(x => x.FloorId).NotEmpty().WithMessage(ErrorMessages.FloorIdCannotBeNull);
            }
        }

        public class Command : IRequest<DeployResult>
        {
            public DeployInfo DeployInfo { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.DeployInfo).NotNull().WithMessage(ErrorMessages.DeployInfoCastFailure).SetValidator(new DeployInfoValidator());
            }
        }

        public class Handler : IRequestHandler<Command,DeployResult>
        {
            private readonly MoniterContext _context;
            private readonly IRemoteClient _remoteClient;

            public Handler(MoniterContext context, IRemoteClient remoteClient)
            {
                _context = context;
                _remoteClient = remoteClient;
            }


            public async Task<DeployResult> Handle(Command request, CancellationToken cancellationToken)
            {
                var oldBinding = _context.Bindings.Include(b=>b.Floor).SingleOrDefault(x => x.Host == request.DeployInfo.Host);
                if (oldBinding != null && oldBinding.Floor.Id != request.DeployInfo.FloorId)
                {
                    throw new RestException(HttpStatusCode.BadRequest, new { Username = ErrorMessages.ServerHostWasBound});
                }
                
                _remoteClient.Deploy(request.DeployInfo);
                var floor = await _context.Floors.SingleOrDefaultAsync(x => x.Id == request.DeployInfo.FloorId, cancellationToken);
                floor.MasterId = request.DeployInfo.MasterId;

                var binding = new Binding
                {
                    Host = request.DeployInfo.Host,
                    Port = request.DeployInfo.Port,
                    UserName = request.DeployInfo.UserName,
                    Password = request.DeployInfo.Password,
                    FloorId = floor.Id
                };
                await _context.Bindings.AddAsync(binding, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return new DeployResult
                {
                    MasterId = request.DeployInfo.MasterId
                };
            }
        }
    }
}