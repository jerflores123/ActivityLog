using IJOS.Application.Common.Mappings;
using System;
using e = IJOS.Domain.Entities;

namespace IJOS.Application.ActivityLog.dtos
{
    public class ActivityLogCboDatadto : IMapFrom<e.ActivityLogCboDatum>
    {
        public string Cbo_type { get; set; }
        public string Cbo_data { get; set; }
        public string Facility { get; set; }
        public DateTime Created_date { get; set; }
        public DateTime? Modified_date { get; set; }
        public string Modified_by { get; set; }
        public string Created_by { get; set; }
    }
}
