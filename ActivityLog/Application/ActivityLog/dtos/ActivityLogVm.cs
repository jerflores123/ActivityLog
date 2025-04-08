using System.Collections.Generic;

namespace IJOS.Application.ActivityLog.dtos
{
    public class ActivityLogVm
    {
        public List<OffenderDto> Offender { get; set; }
        public List<OffenderDto> AllOffenders { get; set; }
        public List<ActivityLogGroupCountsdto> GroupCount { get; set; }
        public ActivityLogDelLimit DelLimit { get; set; }
        public List<StaffDto> Staff { get; set; }
        public List<ActivityLogdto> ActivityLog { get; set; }
        public List<ActivityLogCboDatadto> EventData { get; set; }
        public List<ActivityLogCboDatadto> AreaData { get; set; }
        public List<ActivityLogCboDatadto> GroupData { get; set; }
        public List<OffenderDto> Aztecs { get; set; }
        public List<OffenderDto> Choices { get; set; }
        public List<OffenderDto> Elements { get; set; }
        public List<OffenderDto> Everest { get; set; }
        public List<OffenderDto> Incas { get; set; }
        public List<OffenderDto> Mayas { get; set; }
        public List<OffenderDto> Oa { get; set; }
        public List<OffenderDto> Pathways { get; set; }
        public List<OffenderDto> Sawtooth { get; set; }
        public List<OffenderDto> Solutions { get; set; }
        public List<OffenderDto> Staging { get; set; }
        public List<OffenderDto> Vanguard { get; set; }
        public List<OffenderDto> Wasatch { get; set; }
    }
}

