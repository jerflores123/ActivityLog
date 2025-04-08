using AutoMapper;
using IJOS.Application.ActivityLog.dtos;
using IJOS.Application.Common.Interfaces;
using IJOS.Application.Persistence_Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace IJOS.Application.ActivityLog.Commands.UpdateGroupCount
{
    public class UpdateGroupCountCommand : IRequest
    {
        public ActivityLogGroupCountsdto activityLogGroupCountsDto { get; set; }
    }

    public class UpdateGroupCountCommandHandler : IRequestHandler<UpdateGroupCountCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public UpdateGroupCountCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateGroupCountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var current_UsrID = _currentUserService.UserId;

                var activityGCLog = request.activityLogGroupCountsDto;
                await _unitOfWork.Activity_log_groups_countsRepository.QueryAsync(@"
                    UPDATE [IJOS].[ACTIVITY_LOG_GROUP_COUNTS] 
                    SET   [GROUP_COUNTS] = @GROUP_COUNTS, 
                          [MODIFIED_BY] = @MODIFIED_BY,
                          [MODIFIED_DATE] = CURRENT_TIMESTAMP
                    WHERE [GROUP_NAME] = @GROUP_NAME 
                    AND [FACILITY] = @FACILITY",
                    new
                    {
                        GROUP_COUNTS = activityGCLog.Group_counts,
                        MODIFIED_BY = current_UsrID,
                        GROUP_NAME = activityGCLog.Group_name,
                        FACILITY = activityGCLog.Facility

                    }

                 );
                return Unit.Value;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

    }

}
