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
    public class ActivityLogAreaQuery : IRequest<List<ActivityLogArea>>
    {
        public long sin { get; set; }
    }

    public class ActivityLogAreaQueryHandler : IRequestHandler<ActivityLogAreaQuery, List<ActivityLogArea>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ActivityLogAreaQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ActivityLogArea>> Handle(ActivityLogAreaQuery request, CancellationToken cancellationToken)
        {
            string query = @"select * from [IJOS].[ACTIVITY_LOG_DATA]";
            var result = await _unitOfWork.Activity_log_areaRepository.QueryAsync(query);
            return _mapper.Map<List<ActivityLogArea>>(result);
        }
    }
}