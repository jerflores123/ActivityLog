using AutoMapper;
using IJOS.Application.ActivityLog.dtos;
using IJOS.Application.Common.Interfaces;
using IJOS.Application.Common.Security;
using IJOS.Application.Persistence_Interfaces;
using IJOS.Domain.Common.Constants;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace IJOS.Application.ActivityLog.Queries
{
    [Authorize(Features.Activity_Log, Privileges.Read)]
    public class ActivityLogDelLimitQuery : IRequest<ActivityLogVm>
    {
        public long sin { get; set; }
    }

    public class ActivityLogDelLimitQueryHandler : IRequestHandler<ActivityLogDelLimitQuery, ActivityLogVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public ActivityLogDelLimitQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<ActivityLogVm> Handle(ActivityLogDelLimitQuery request, CancellationToken cancellationToken)
        {
            var current_UsrID = _currentUserService.UserId;

            string sql3_Usr_Agency_ID =
            @" select s.AGENCY_ID, s.COUNTY_NAME  
                                  from [IJOS].[ASPNETUSERS] as a
                                  join [IJOS].[STAFF] as s on a.Id = s.UserID
                                  where Id =@Id ";
            var dbResults3 = await _unitOfWork.StaffRepository.QuerySingleAsync
                (sql3_Usr_Agency_ID, new { Id = current_UsrID });

            var usr_Facility = "";

            if (dbResults3.AgencyId == 90)
            {
                if (dbResults3.CountyId == (int)Counties.CANYON)
                {
                    usr_Facility = "JCCN";
                }
                else if (dbResults3.CountyId == (int)Counties.NEZ_PERCE)
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
                if (dbResults3.CountyId == (int)Counties.BONNER)
                {
                    usr_Facility = "BONNER";
                }
                else if (dbResults3.CountyId == (int)Counties.NEZ_PERCE)
                {
                    usr_Facility = "NEZPERCE";
                }
            }

            string query = @"select * from [IJOS].[ACTIVITY_LOG_DEL_LIMIT] WHERE FACILITY = '" + usr_Facility + "'";
            var result = await _unitOfWork.Activity_log_del_limitRepository.QuerySingleAsync(query);

            return /*_mapper.Map<List<ActivityLogDelLimit>>(result);*/
            new ActivityLogVm()
            {
                DelLimit = _mapper.Map<ActivityLogDelLimit>(result)
            };
        }
    }
}