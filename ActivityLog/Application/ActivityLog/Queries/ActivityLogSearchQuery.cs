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
    public class ActivityLogSearchQuery : IRequest<ActivityLogSearchVM>
    {
        public string searchString { get; set; }
    }

    public class ActivityLogSearchQueryHandler : IRequestHandler<ActivityLogSearchQuery, ActivityLogSearchVM>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ActivityLogSearchQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ActivityLogSearchVM> Handle(ActivityLogSearchQuery request, CancellationToken cancellationToken)
        {
            ActivityLogSearchVM activityLogSearch = new ActivityLogSearchVM();
            activityLogSearch.Search_String = request.searchString.Trim();

            var query = await _unitOfWork.Activity_logRepository.QueryAsync(

                        @"SELECT * FROM IJOS.ACTIVITY_LOG
                            WHERE UPPER(DESCRIPTION) LIKE '%' + @searchString + '%'"
                , new { searchString = request.searchString.Trim() });
            activityLogSearch.ActivityLog = _mapper.Map<List<ActivityLogdto>>(query);

            return activityLogSearch;
        }
    }
}
