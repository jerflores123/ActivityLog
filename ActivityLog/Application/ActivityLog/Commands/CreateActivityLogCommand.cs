using AutoMapper;
using IJOS.Application.ActivityLog.dtos;
using IJOS.Application.Common.Interfaces;
using IJOS.Application.Common.Security;
using IJOS.Application.Persistence_Interfaces;
using IJOS.Domain.Common.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IJOS.Application.ActivityLog.Commands
{
    [Authorize(Features.Activity_Log, Privileges.Create)]
    public class CreateActivityLogCommand : IRequest
    {
        public ActivityLogdto activityLogDto { get; set; }
    }

    public class CreateActivityLogCommandHandler : IRequestHandler<CreateActivityLogCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public CreateActivityLogCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IApplicationDbContext dbContext, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(CreateActivityLogCommand request, CancellationToken cancellationToken)
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

            var currentUserStaff = await _dbContext.Staff.Where(x => x.StaffKey == _currentUserService.StaffKey).Select(x => new { x.CountyId, x.AgencyId }).SingleAsync(cancellationToken);

            //trying this to insert facility
            var usr_Facility = "";
            if (currentUserStaff.AgencyId == (long)Agencies.IDJC)
            {
                if (currentUserStaff.CountyId == (int)Counties.CANYON)
                {
                    usr_Facility = "JCCN";
                }
                else if (currentUserStaff.CountyId == (int)Counties.NEZ_PERCE)
                {
                    usr_Facility = "JCCL";
                }
                else
                {
                    var email = "email ijos";
                }
            }
            else
            {
                if (currentUserStaff.CountyId == (int)Counties.BONNER)
                {
                    usr_Facility = "BONNER";
                    //select * from ijos.OFFENDER where COUNTY_NAME = 'BONNER'

                }
                else if (currentUserStaff.CountyId == (int)Counties.NEZ_PERCE)
                {
                    usr_Facility = "NEZ PERCE";
                }
            }

            await _unitOfWork.Activity_logRepository.QueryAsync(@"
                    INSERT INTO [IJOS].[ACTIVITY_LOG]([LOG_DATE],[DESCRIPTION],[COMMENTS],
                    [EVENT_TYPE],[IS_ACTIVE],[FACILITY],[CREATED_DATE],[CREATED_BY])
                    VALUES (CURRENT_TIMESTAMP, @DESCRIPTION, @COMMENTS, @EVENT_TYPE, @IS_ACTIVE, @FACILITY,
                    CURRENT_TIMESTAMP, @CREATED_BY)",
                new
                {

                    DESCRIPTION = activityLog.Description,
                    COMMENTS = activityLog.Comments,
                    EVENT_TYPE = activityLog.Event_type,
                    IS_ACTIVE = activityLog.Is_active,
                    FACILITY = usr_Facility,
                    CREATED_BY = activityLog.Created_by
                }
            );

            return Unit.Value;
        }
    }
}
