using AutoMapper;
using IJOS.Application.ActivityLog.dtos;
using IJOS.Application.Common.Security;
using IJOS.Application.Persistence_Interfaces;
using IJOS.Domain.Common.Constants;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IJOS.Application.ActivityLog.Queries
{
    [Authorize(Features.Activity_Log, Privileges.Read)]
    public class ActivityLogCboDataQuery : IRequest<List<ActivityLogCboDatadto>>
    {
        public long sin { get; set; }
    }

    public class ActivityLogCboDataQueryHandler : IRequestHandler<ActivityLogCboDataQuery, List<ActivityLogCboDatadto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ActivityLogCboDataQueryHandler(IMapper mapper, IUnitOfWork unitOfwork)
        {
            _unitOfWork = unitOfwork;
            _mapper = mapper;
        }

        public async Task<List<ActivityLogCboDatadto>> Handle(ActivityLogCboDataQuery request, CancellationToken cancellationToken)
        {
                //this query will be based on Facility only for event
                string query = @"select * from [IJOS].[ACTIVITY_LOG_CBO_DATA] ORDER By CBO_DATA";
                var result = await _unitOfWork.Activity_cbo_dataRepository.QueryAsync(query);

                //I need to do another query but for area
                string query2 = @"select * from [IJOS].[ACTIVITY_LOG_CBO_DATA] WHERE CBO_DATA = 'AREA' ORDER By CBO_DATA";
                var result2 = await _unitOfWork.Activity_cbo_dataRepository.QueryAsync(query2);
                //another query for groups

                return _mapper.Map<List<ActivityLogCboDatadto>>(result);

                /*return new ActivityLogVm()
                {
                    cboDataList = _mapper.Map<List<ActivityLogCboDatadto>>(result),
                    areaDataList = _mapper.Map<List<PreCommitScreeningOutcomesDto>>(result_outcomes)
                    GroupsDataList = _mapper.Map<List<PreCommitScreeningOutcomesDto>>(result_outcomes)
                };*/
        }
    }
}