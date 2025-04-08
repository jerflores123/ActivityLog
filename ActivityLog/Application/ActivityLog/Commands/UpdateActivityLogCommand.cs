using AutoMapper;
using IJOS.Application.ActivityLog.dtos;
using IJOS.Application.Common.Security;
using IJOS.Application.Persistence_Interfaces;
using IJOS.Domain.Common.Constants;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace IJOS.Application.ActivityLog.Commands
{
    [Authorize(Features.Activity_Log, Privileges.Modify)]
    public class UpdateActivityLogCommand : IRequest
    {
        public ActivityLogdto activityLogDto { get; set; }
    }

    public class UpdateActivityLogCommandHandler : IRequestHandler<UpdateActivityLogCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateActivityLogCommandHandler(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateActivityLogCommand request, CancellationToken cancellationToken)
        {
            var activityLog = request.activityLogDto;
            string[] stringArray = activityLog.Description.Split(',');

            if (activityLog.Event_type == "Apprehension" || activityLog.Event_type == "Escape")
            {
                activityLog.Description = activityLog.Event_type + ":" + stringArray[7] + " from " + stringArray[3];
            }
            else if (activityLog.Event_type == "Movements")
            {
                activityLog.Description = stringArray[6] + " " + stringArray[2] + ", " + stringArray[7]
                    + " From " + stringArray[3] + " To " + stringArray[4];
            }
            else if (activityLog.Event_type == "Code Red" || activityLog.Event_type == "Code Yellow")
            {
                activityLog.Description = activityLog.Event_type + ":" + stringArray[6] + " " + stringArray[3];
            }
            else if (activityLog.Event_type == "Transport")
            {
                if (stringArray[9] != null)
                {
                    stringArray[9] = " Car# " + stringArray[9];
                }

                if (stringArray[10] != null)
                {
                    stringArray[10] = " Cell# " + stringArray[10];
                }
                //BLACKBURN, CURTIS Out With, CODY HARRIS.   Car# 4177 Cell# 3691  New SAWTOOTH Count: 17
                activityLog.Description = stringArray[5] + " Transporting " + stringArray[7] + " " + stringArray[9] + stringArray[10] + ".";
            }
            else if (activityLog.Event_type == "Attendance")
            {
                activityLog.Description = activityLog.Event_type + ":" + stringArray[6] + " " + stringArray[5];
            }
            else if (activityLog.Event_type == "Perimeter Search")
            {
                activityLog.Description = stringArray[5] + " Checked Area(s): " + stringArray[11];
            }
            else if (activityLog.Event_type == "Search")
            {
                activityLog.Description = activityLog.Event_type + ":" + stringArray[5] + ", " + stringArray[7];
            }
            else if (activityLog.Event_type == "Transfer")
            {
                activityLog.Description = activityLog.Event_type + ":" + stringArray[5] + ", " + stringArray[7]
                    + " to " + stringArray[2];
            }
            else if (activityLog.Event_type == "Radios & Keys")
            {
                //on hold
                activityLog.Description = activityLog.Event_type + ": " + stringArray[6];
            }
            else
            {
                activityLog.Description = activityLog.Event_type + ": " + stringArray[6];

            }
            await _unitOfWork.Activity_logRepository.QueryAsync(@"
                    UPDATE [IJOS].[ACTIVITY_LOG] SET   [DESCRIPTION] = @DESCRIPTION, 
                    [COMMENTS] = @COMMENTS, 
                    [MODIFIED_BY] = @MODIFIED_BY, [MODIFIED_DATE] = CURRENT_TIMESTAMP
                    WHERE [LOG_ID] = @LOG_ID",
                new
                {
                    DESCRIPTION = activityLog.Description,
                    COMMENTS = activityLog.Comments,
                    MODIFIED_BY = activityLog.Modified_by,
                    LOG_ID = activityLog.Log_id

                }
            );

            return Unit.Value;
        }
    }
}