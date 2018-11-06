using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moniter.Infrastructure;

namespace Moniter.Features.Deploy
{
    public class FetchBindings
    {
        public class Command : IRequest<List<Bindings>>
        {
            
        }

        public class Handler : IRequestHandler<Command,List<Bindings>>
        {
            private readonly MoniterContext _context;

            public Handler(MoniterContext context, IMapper mapper)
            {
                _context = context;
            }

            public async Task<List<Bindings>> Handle(Command request, CancellationToken cancellationToken)
            {
                var bindings = _context.Floors
                    .Include(x=>x.Building)
                    .Include(x=>x.Binding)
                    .OrderBy(x=>x.CreatedTime)
                    .Select(x=>x.ToBindingsViewModel());
                return await bindings.ToListAsync(cancellationToken);
            }
        }
    }
}