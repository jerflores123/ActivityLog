using IJOS.Application.Common.Mappings;
using System;
using e = IJOS.Domain.Entities;

namespace IJOS.Application.ActivityLog.dtos
{
    public class ActivityLogGroupCountsdto : IMapFrom<e.ActivityLogGroupCount>
    {
        public string Group_name { get; set; }
        public long? Group_counts { get; set; }
        public string Facility { get; set; }
        public DateTime Created_date { get; set; }
        public DateTime? Modified_date { get; set; }
        public string Modified_by { get; set; }
        public string Created_by { get; set; }
    }
}
