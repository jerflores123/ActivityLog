using AutoMapper;
using IJOS.Application.ActivityLog.dtos;
using IJOS.Application.Common.Interfaces;
using IJOS.Application.Common.Security;
using IJOS.Application.Persistence_Interfaces;
using IJOS.Domain.Common.Constants;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IJOS.Application.ActivityLog.Queries
{
    [Authorize(Features.Activity_Log, Privileges.Read)]
    public class ActivityLogGroupsCountsQuery : IRequest<ActivityLogVm>
    {
        public long sin { get; set; }
    }

    public class ActivityLogGroupsCountsQueryHandler : IRequestHandler<ActivityLogGroupsCountsQuery, ActivityLogVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public ActivityLogGroupsCountsQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<ActivityLogVm> Handle(ActivityLogGroupsCountsQuery request, CancellationToken cancellationToken)
        {
            try
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
                string query2 = "";
                if (dbResults3.AgencyId == 90)
                {
                    if (dbResults3.CountyId == (int)Counties.CANYON)
                    {
                        usr_Facility = "JCCN";
                        query2 = @"select distinct o.LAST_NAME, o.FIRST_NAME 
                                    from ijos.FACILITY_PROGRAM fp , ijos.offender o, ijos.placement p 
                                    where o.sin = p.sin and p.FACILITY_ID = fp.FACILITY_ID and fp.FACILITY_ID = 536
                                    and p.ACTUAL_RELEASE_DATE is null";

                    }
                    else if (dbResults3.CountyId == (int)Counties.NEZ_PERCE)
                    {
                        usr_Facility = "JCCL";
                        query2 = @"select distinct o.LAST_NAME, o.FIRST_NAME 
                                    from ijos.FACILITY_PROGRAM fp , ijos.offender o, ijos.placement p 
                                    where o.sin = p.sin and p.FACILITY_ID = fp.FACILITY_ID and fp.FACILITY_ID = 535
                                    and p.ACTUAL_RELEASE_DATE is null";
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

                string query = @"select * from [IJOS].[ACTIVITY_LOG_GROUP_COUNTS] WHERE FACILITY = '" + usr_Facility + "'";
                var result = await _unitOfWork.Activity_log_groups_countsRepository.QueryAsync(query);

                return /*_mapper.Map<List<ActivityLogGroupCountsdto>>(result);*/

                new ActivityLogVm()
                {
                    GroupCount = _mapper.Map<List<ActivityLogGroupCountsdto>>(result)


                };
            }


            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
