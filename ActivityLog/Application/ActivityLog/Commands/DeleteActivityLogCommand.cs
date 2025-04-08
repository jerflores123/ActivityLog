using AutoMapper;
using IJOS.Application.Common.Interfaces;
using IJOS.Application.Common.Security;
using IJOS.Domain.Common.Constants;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace IJOS.Application.ActivityLog.Commands
{
    [Authorize(Features.Activity_Log, Privileges.Delete)]
    public class DeleteActivityLogCommand : IRequest
    {
        public long log_Id { get; set; }
    }

    public class DeleteActivityLogCommandHandler : IRequestHandler<DeleteActivityLogCommand>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public DeleteActivityLogCommandHandler(IMapper mapper, IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteActivityLogCommand request, CancellationToken cancellationToken)
        {
            var activityLog = await _applicationDbContext.ActivityLogs.FindAsync(new object[] { request.log_Id }, cancellationToken: cancellationToken);
            activityLog.IsActive = false;
            await _applicationDbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}