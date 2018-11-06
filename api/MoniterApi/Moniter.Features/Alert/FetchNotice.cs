using System;
using System.Collections.Generic;
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
using Moniter.Models;

namespace Moniter.Features.Alert
{   
    public class FetchNotice
    {
        public class Command : IRequest<List<Alert>>
        {
            
        }

        public class Handler : IRequestHandler<Command,List<Alert>>
        {
            private readonly MoniterContext _context;
            private readonly IMapper _mapper;

            public Handler(MoniterContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public Task<List<Alert>> Handle(Command request, CancellationToken cancellationToken)
            {
                var notices = _context.Alerts.Where(a => a.Status == AlertStatus.New);
                return notices.Select(n => n.ToViewModel()).ToListAsync(cancellationToken);
            }
        }
    }
}