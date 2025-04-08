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
    public class ActivityLogRadiosQuery : IRequest<List<ActivityLogRadios>>
    {
        public long sin { get; set; }
    }

    public class ActivityLogRadiosQueryHandler : IRequestHandler<ActivityLogRadiosQuery, List<ActivityLogRadios>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ActivityLogRadiosQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ActivityLogRadios>> Handle(ActivityLogRadiosQuery request, CancellationToken cancellationToken)
        {

            string query = @"select * from [IJOS].[ACTIVITY_LOG_RADIOS]";
            var result = await _unitOfWork.Activity_log_radiosRepository.QueryAsync(query);
            return _mapper.Map<List<ActivityLogRadios>>(result);
        }
    }
}