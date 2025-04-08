using AutoMapper;
using IJOS.Application.ActivityLog.dtos;
using IJOS.Application.Common.Interfaces;
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
    public class ActivityLogQuery : IRequest<ActivityLogVm>
    {
        public long sin { get; set; }
    }

    public class ActivityLogQueryHandler : IRequestHandler<ActivityLogQuery, ActivityLogVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public ActivityLogQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<ActivityLogVm> Handle(ActivityLogQuery request, CancellationToken cancellationToken)
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
                                    where o.sin = p.sin and p.FACILITY_ID = fp.FACILITY_ID and fp.FACILITY_ID = 382
                                    and p.ACTUAL_RELEASE_DATE is null";

                }
                else if (dbResults3.CountyId == (int)Counties.NEZ_PERCE)
                {
                    usr_Facility = "JCCL";
                    query2 = @"select distinct o.LAST_NAME, o.FIRST_NAME 
                                    from ijos.FACILITY_PROGRAM fp , ijos.offender o, ijos.placement p 
                                    where o.sin = p.sin and p.FACILITY_ID = fp.FACILITY_ID and fp.FACILITY_ID = 210
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
                    //select * from ijos.OFFENDER where COUNTY_NAME = 'BONNER'
                    query2 = @"select * from ijos.OFFENDER where COUNTY_NAME = 'BONNER'";
                }
                else if (dbResults3.CountyId == (int)Counties.NEZ_PERCE)
                {
                    usr_Facility = "NEZPERCE";
                    query2 = @"select * from ijos.OFFENDER where COUNTY_NAME = 'NEZ PERCE'";
                }
            }

            string query = @"select * from [IJOS].[ACTIVITY_LOG] WHERE IS_ACTIVE = '1' AND FACILITY = '" + usr_Facility + "' ORDER By LOG_DATE DESC ";
            var result = await _unitOfWork.Activity_logRepository.QueryAsync(query);
            var activityLogDto = _mapper.Map<List<ActivityLogdto>>(result);

            var result2 = await _unitOfWork.OffenderRepository.QueryAsync(query2, new { sin = request.sin });
            var offenderFacilityDto = _mapper.Map<List<OffenderDto>>(result2);

            string query3 = @"select * from [IJOS].[STAFF] WHERE AGENCY_ID = '" + dbResults3.AgencyId + "' AND IS_ACTIVE = 'True' ORDER By LAST_NAME";
            var result3 = await _unitOfWork.StaffRepository.QueryAsync(query3);
            var staffList = _mapper.Map<List<StaffDto>>(result3);

            string query4 = @"select * from [IJOS].[ACTIVITY_LOG_CBO_DATA] WHERE FACILITY = '" + usr_Facility + "' AND CBO_TYPE = 'event' ORDER By CBO_DATA";
            var result4 = await _unitOfWork.Activity_cbo_dataRepository.QueryAsync(query4);

            string query5 = @"select * from [IJOS].[ACTIVITY_LOG_CBO_DATA] WHERE FACILITY = '" + usr_Facility + "' AND CBO_TYPE = 'area' ORDER By CBO_DATA";
            var result5 = await _unitOfWork.Activity_cbo_dataRepository.QueryAsync(query5);

            string query6 = @"select * from [IJOS].[ACTIVITY_LOG_CBO_DATA] WHERE FACILITY = '" + usr_Facility + "' AND CBO_TYPE = 'group' ORDER By CBO_DATA";
            var result6 = await _unitOfWork.Activity_cbo_dataRepository.QueryAsync(query6);

            string query7 = @"select distinct cd.sin, o.LAST_NAME, o.FIRST_NAME from ijos.COURT_COMMITMENT_DATE cd, ijos.OFFENDER o where o.sin = cd.sin and DATE_OF_DISCHARGE is null";
            var result7 = await _unitOfWork.OffenderRepository.QueryAsync(query7, new { sin = request.sin });
            var offenderCommitedDto = _mapper.Map<List<OffenderDto>>(result7);
            //aztecs!!
            string aztecsQuery = @"select o.sin, o.LAST_NAME, o.FIRST_NAME 
                                            from ijos.PLACEMENT_GROUP pg, ijos.OFFENDER o, ijos.FACILITY_PROGRAM fp, ijos.FACILITY_PROGRAM_UNIT fpu, ijos.placement p
                                            where o.sin = p.SIN and p.PROGRAM_ID = fp.FACILITY_PROGRAM_ID
                                            and fp.FACILITY_PROGRAM_ID = fpu.FACILITY_PROGRAM_ID
                                            and fpu.FACILITY_PROGRAM_UNIT_ID = pg.FACILITY_PROGRAM_UNIT_ID
                                            and p.ACTUAL_RELEASE_DATE is null and pg.GROUP_ID = p.GROUP_ID and p.GROUP_ID = 1";
            var aztecsResult = await _unitOfWork.OffenderRepository.QueryAsync(aztecsQuery, new { sin = request.sin });
            var aztecsOutput = _mapper.Map<List<OffenderDto>>(aztecsResult);
            //List<string> aztecsDto = aztecsResult.Select(o => o.First_name).Distinct().ToList();
            //choices
            string choicesQuery = @"select fp.PROGRAM, fp.FACILITY_PROGRAM_ID, o.sin, o.LAST_NAME, o.FIRST_NAME 
                                            from ijos.FACILITY_PROGRAM fp , ijos.offender o, ijos.placement p 
                                            where o.sin = p.sin and p.PROGRAM_ID = fp.FACILITY_PROGRAM_ID and p.ACTUAL_RELEASE_DATE is null
                                            and p.PROGRAM_ID = 7";
            var choicesResult = await _unitOfWork.OffenderRepository.QueryAsync(choicesQuery);
            var choicesOutput = _mapper.Map<List<OffenderDto>>(choicesResult);
            //elements!!
            string elementsQuery = @"select o.sin, o.LAST_NAME, o.FIRST_NAME 
                                            from ijos.PLACEMENT_GROUP pg, ijos.OFFENDER o, ijos.FACILITY_PROGRAM fp, ijos.FACILITY_PROGRAM_UNIT fpu, ijos.placement p
                                            where o.sin = p.SIN and p.PROGRAM_ID = fp.FACILITY_PROGRAM_ID
                                            and fp.FACILITY_PROGRAM_ID = fpu.FACILITY_PROGRAM_ID
                                            and fpu.FACILITY_PROGRAM_UNIT_ID = pg.FACILITY_PROGRAM_UNIT_ID
                                            and p.ACTUAL_RELEASE_DATE is null and pg.GROUP_ID = p.GROUP_ID and p.GROUP_ID = 8";
            var elementsResult = await _unitOfWork.OffenderRepository.QueryAsync(elementsQuery);
            var elementsOutput = _mapper.Map<List<OffenderDto>>(elementsResult);
            //everest!!
            string everestQuery = @"select o.sin, o.LAST_NAME, o.FIRST_NAME 
                                            from ijos.PLACEMENT_GROUP pg, ijos.OFFENDER o, ijos.FACILITY_PROGRAM fp, ijos.FACILITY_PROGRAM_UNIT fpu, ijos.placement p
                                            where o.sin = p.SIN and p.PROGRAM_ID = fp.FACILITY_PROGRAM_ID
                                            and fp.FACILITY_PROGRAM_ID = fpu.FACILITY_PROGRAM_ID
                                            and fpu.FACILITY_PROGRAM_UNIT_ID = pg.FACILITY_PROGRAM_UNIT_ID
                                            and p.ACTUAL_RELEASE_DATE is null and pg.GROUP_ID = p.GROUP_ID and p.GROUP_ID = 4";
            var everestResult = await _unitOfWork.OffenderRepository.QueryAsync(everestQuery);
            var everestOutput = _mapper.Map<List<OffenderDto>>(everestResult);
            //incas!!
            string incasQuery = @"select o.sin, o.LAST_NAME, o.FIRST_NAME 
                                            from ijos.PLACEMENT_GROUP pg, ijos.OFFENDER o, ijos.FACILITY_PROGRAM fp, ijos.FACILITY_PROGRAM_UNIT fpu, ijos.placement p
                                            where o.sin = p.SIN and p.PROGRAM_ID = fp.FACILITY_PROGRAM_ID
                                            and fp.FACILITY_PROGRAM_ID = fpu.FACILITY_PROGRAM_ID
                                            and fpu.FACILITY_PROGRAM_UNIT_ID = pg.FACILITY_PROGRAM_UNIT_ID
                                            and p.ACTUAL_RELEASE_DATE is null and pg.GROUP_ID = p.GROUP_ID and p.GROUP_ID = 2";
            var incasResult = await _unitOfWork.OffenderRepository.QueryAsync(incasQuery);
            var incasOutput = _mapper.Map<List<OffenderDto>>(incasResult);
            //mayas!!
            string mayasQuery = @"select o.sin, o.LAST_NAME, o.FIRST_NAME 
                                            from ijos.PLACEMENT_GROUP pg, ijos.OFFENDER o, ijos.FACILITY_PROGRAM fp, ijos.FACILITY_PROGRAM_UNIT fpu, ijos.placement p
                                            where o.sin = p.SIN and p.PROGRAM_ID = fp.FACILITY_PROGRAM_ID
                                            and fp.FACILITY_PROGRAM_ID = fpu.FACILITY_PROGRAM_ID
                                            and fpu.FACILITY_PROGRAM_UNIT_ID = pg.FACILITY_PROGRAM_UNIT_ID
                                            and p.ACTUAL_RELEASE_DATE is null and pg.GROUP_ID = p.GROUP_ID and p.GROUP_ID = 3";
            var mayasResult = await _unitOfWork.OffenderRepository.QueryAsync(mayasQuery);
            var mayasOutput = _mapper.Map<List<OffenderDto>>(mayasResult);
            //o/a ask?
            string oaQuery = @"select fp.PROGRAM, fp.FACILITY_PROGRAM_ID, o.sin, o.LAST_NAME, o.FIRST_NAME 
                                        from ijos.FACILITY_PROGRAM fp , ijos.offender o, ijos.placement p 
                                        where o.sin = p.sin and p.PROGRAM_ID = fp.FACILITY_PROGRAM_ID and p.ACTUAL_RELEASE_DATE is null
                                        and p.PROGRAM_ID = 6";
            var oaResult = await _unitOfWork.OffenderRepository.QueryAsync(oaQuery);
            var oaOutput = _mapper.Map<List<OffenderDto>>(oaResult);
            //pathways
            string pathwaysQuery = @"select fp.PROGRAM, fp.FACILITY_PROGRAM_ID, o.sin, o.LAST_NAME, o.FIRST_NAME 
                                                from ijos.FACILITY_PROGRAM fp , ijos.offender o, ijos.placement p 
                                                where o.sin = p.sin and p.PROGRAM_ID = fp.FACILITY_PROGRAM_ID and p.ACTUAL_RELEASE_DATE is null
                                                and p.PROGRAM_ID = 8";
            var pathwaysResult = await _unitOfWork.OffenderRepository.QueryAsync(pathwaysQuery);
            var pathwaysOutput = _mapper.Map<List<OffenderDto>>(pathwaysResult);
            //sawtooth!!
            string sawtoothQuery = @"select o.sin, o.LAST_NAME, o.FIRST_NAME 
                                            from ijos.PLACEMENT_GROUP pg, ijos.OFFENDER o, ijos.FACILITY_PROGRAM fp, ijos.FACILITY_PROGRAM_UNIT fpu, ijos.placement p
                                            where o.sin = p.SIN and p.PROGRAM_ID = fp.FACILITY_PROGRAM_ID
                                            and fp.FACILITY_PROGRAM_ID = fpu.FACILITY_PROGRAM_ID
                                            and fpu.FACILITY_PROGRAM_UNIT_ID = pg.FACILITY_PROGRAM_UNIT_ID
                                            and p.ACTUAL_RELEASE_DATE is null and pg.GROUP_ID = p.GROUP_ID and p.GROUP_ID = 6";
            var sawtoothResult = await _unitOfWork.OffenderRepository.QueryAsync(sawtoothQuery);
            var sawtoothOutput = _mapper.Map<List<OffenderDto>>(sawtoothResult);
            //solutions
            string solutionsQuery = @"select fp.PROGRAM, fp.FACILITY_PROGRAM_ID, o.sin, o.LAST_NAME, o.FIRST_NAME 
                                                from ijos.FACILITY_PROGRAM fp , ijos.offender o, ijos.placement p 
                                                where o.sin = p.sin and p.PROGRAM_ID = fp.FACILITY_PROGRAM_ID and p.ACTUAL_RELEASE_DATE is null
                                                and p.PROGRAM_ID = 9";
            var solutionsResult = await _unitOfWork.OffenderRepository.QueryAsync(solutionsQuery);
            var solutionsOutput = _mapper.Map<List<OffenderDto>>(solutionsResult);
            //staging ask?
            string stagingQuery = @"select o.sin, o.LAST_NAME, o.FIRST_NAME 
                                            from ijos.PLACEMENT_GROUP pg, ijos.OFFENDER o, ijos.FACILITY_PROGRAM fp, ijos.FACILITY_PROGRAM_UNIT fpu, ijos.placement p
                                            where o.sin = p.SIN and p.PROGRAM_ID = fp.FACILITY_PROGRAM_ID
                                            and fp.FACILITY_PROGRAM_ID = fpu.FACILITY_PROGRAM_ID
                                            and fpu.FACILITY_PROGRAM_UNIT_ID = pg.FACILITY_PROGRAM_UNIT_ID
                                            and p.ACTUAL_RELEASE_DATE is null and pg.GROUP_ID = p.GROUP_ID and p.GROUP_ID = 20";
            var stagingResult = await _unitOfWork.OffenderRepository.QueryAsync(stagingQuery);
            var stagingOutput = _mapper.Map<List<OffenderDto>>(stagingResult);
            //vanguard!!
            string vanguardQuery = @"select o.sin, o.LAST_NAME, o.FIRST_NAME 
                                            from ijos.PLACEMENT_GROUP pg, ijos.OFFENDER o, ijos.FACILITY_PROGRAM fp, ijos.FACILITY_PROGRAM_UNIT fpu, ijos.placement p
                                            where o.sin = p.SIN and p.PROGRAM_ID = fp.FACILITY_PROGRAM_ID
                                            and fp.FACILITY_PROGRAM_ID = fpu.FACILITY_PROGRAM_ID
                                            and fpu.FACILITY_PROGRAM_UNIT_ID = pg.FACILITY_PROGRAM_UNIT_ID
                                            and p.ACTUAL_RELEASE_DATE is null and pg.GROUP_ID = p.GROUP_ID and p.GROUP_ID = 9";
            var vanguardResult = await _unitOfWork.OffenderRepository.QueryAsync(vanguardQuery);
            var vanguardOutput = _mapper.Map<List<OffenderDto>>(vanguardResult);
            //wasatch!!
            string wasatchQuery = @"select o.sin, o.LAST_NAME, o.FIRST_NAME 
                                            from ijos.PLACEMENT_GROUP pg, ijos.OFFENDER o, ijos.FACILITY_PROGRAM fp, ijos.FACILITY_PROGRAM_UNIT fpu, ijos.placement p
                                            where o.sin = p.SIN and p.PROGRAM_ID = fp.FACILITY_PROGRAM_ID
                                            and fp.FACILITY_PROGRAM_ID = fpu.FACILITY_PROGRAM_ID
                                            and fpu.FACILITY_PROGRAM_UNIT_ID = pg.FACILITY_PROGRAM_UNIT_ID
                                            and p.ACTUAL_RELEASE_DATE is null and pg.GROUP_ID = p.GROUP_ID and p.GROUP_ID = 7";
            var wasatchResult = await _unitOfWork.OffenderRepository.QueryAsync(wasatchQuery);
            var wasatchOutput = _mapper.Map<List<OffenderDto>>(wasatchResult);


            return new ActivityLogVm()
            {

                Offender = offenderFacilityDto,
                AllOffenders = offenderCommitedDto,
                Staff = staffList,
                ActivityLog = activityLogDto,
                EventData = _mapper.Map<List<ActivityLogCboDatadto>>(result4),
                AreaData = _mapper.Map<List<ActivityLogCboDatadto>>(result5),
                GroupData = _mapper.Map<List<ActivityLogCboDatadto>>(result6),
                Aztecs = aztecsOutput,
                Choices = choicesOutput,
                Elements = elementsOutput,
                Everest = everestOutput,
                Incas = incasOutput,
                Mayas = mayasOutput,
                Oa = oaOutput,
                Pathways = pathwaysOutput,
                Sawtooth = sawtoothOutput,
                Solutions = solutionsOutput,
                Staging = stagingOutput,
                Vanguard = vanguardOutput,
                Wasatch = wasatchOutput

            };
        }
    }
}
