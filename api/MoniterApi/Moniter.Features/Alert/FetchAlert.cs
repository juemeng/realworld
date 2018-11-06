using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moniter.Infrastructure;
using Moniter.Models;

namespace Moniter.Features.Alert
{
    public class FetchAlert
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


            public async Task<List<Alert>> Handle(Command request, CancellationToken cancellationToken)
            {
                var alerts = await _context.Alerts.Select(x=>x.ToViewModel())
                    .ToListAsync(cancellationToken);
                return alerts;
            }
        }
    }
}