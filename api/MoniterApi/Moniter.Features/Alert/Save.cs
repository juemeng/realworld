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
using Moniter.Models;

namespace Moniter.Features.Alert
{
    public class Save
    {
        public class Command : IRequest<SaveResultEnvelope>
        {
            public AlertMessage Message { get; set; }
            public string Description { get; set; }
            public Guid MasterId { get; set; }
            public string SlaveNumber { get; set; }
            public DateTime AlertTime { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.MasterId).NotNull().WithMessage(ErrorMessages.MasterIdNotFound);
            }
        }
        
        public class Handler : IRequestHandler<Command,SaveResultEnvelope>
        {
            private readonly MoniterContext _context;
            private readonly IMapper _mapper;

            public Handler(MoniterContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<SaveResultEnvelope> Handle(Command request, CancellationToken cancellationToken)
            {
                Models.Alert alert = null;
                if (request.MasterId == Guid.Empty)
                {
                    throw new RestException(HttpStatusCode.Unauthorized,new {Error = ErrorMessages.MasterIdDidNotBind});
                }

                var boundFloor = await _context.Floors.SingleOrDefaultAsync(f => f.MasterId == request.MasterId, cancellationToken);
                if (boundFloor == null)
                {
                    throw new RestException(HttpStatusCode.Unauthorized,new {Error = ErrorMessages.MasterIdDidNotBind});
                }
                
                if (request.Message != AlertMessage.Time)
                {
                    var masterId = request.MasterId;
                    var floor = _context.Floors
                        .Include(f=>f.Building)
                        .Include(f=>f.Rooms)
                        .ThenInclude(x=>x.Beds)
                        .SingleOrDefault(f => f.MasterId == masterId);
                    
                    var building = floor.Building;
                    var bed = floor.Rooms.SelectMany(x => x.Beds).SingleOrDefault(b => b.SlaveNumber == request.SlaveNumber);
                    var room = bed.Room;

                    alert = await _context.Alerts.SingleOrDefaultAsync(
                        a => a.BedNumber == bed.Number 
                             && a.SlaveNumber == bed.SlaveNumber 
                             && (a.Message == AlertMessage.NormalCall || a.Message == AlertMessage.UrgencyCall), cancellationToken);

                    if (alert != null)
                    {
                        if (request.Message != AlertMessage.UrgencyCall && request.Message != AlertMessage.NormalCall)
                        {
                            alert.Status = AlertStatus.Reset;
                        }
                    }
                    else
                    {
                        alert = new Models.Alert
                        {
                            Id = Guid.NewGuid(),
                            Message = request.Message,
                            Description = request.Description,
                            BuildingId = building.Id,
                            BuildingName = building.Name,
                            FloorId = floor.Id,
                            FloorNumber = floor.Number,
                            FloorDescription = floor.Description,
                            RoomId = room.Id,
                            RoomName = room.Name,
                            RoomNumber = room.Number,
                            BedNumber = bed.Number,
                            MasterId = request.MasterId,
                            SlaveNumber = request.SlaveNumber,
                            AlertTime = request.AlertTime,
                            Status = AlertStatus.New
                        };

                        _context.Alerts.Add(alert);
                    }
                    
                    
                    await _context.SaveChangesAsync(cancellationToken);

                    var alertViewModel = alert.ToViewModel();
                    
                    var result = new SaveResult
                    {
                        AlertId = alert.Id,
                        AlertTime = alert.AlertTime,
                        Description = alert.Description,
                        Status = SaveAlertStatus.Saved,
                        Message = alert.Status == AlertStatus.Reset?@"已重置报警信息":@"已添加报警信息"
                    };
                    
                    return new SaveResultEnvelope(alertViewModel,result);
                }
                
                return new SaveResultEnvelope
                {
                    Skipped = true
                };
            }
        }
    }
}