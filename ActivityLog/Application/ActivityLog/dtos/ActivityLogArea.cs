using IJOS.Application.Common.Mappings;
using System;
using e = IJOS.Domain.Entities;

namespace IJOS.Application.ActivityLog.dtos
{
    public class ActivityLogArea : IMapFrom<e.ActivityLogArea>
    {
        public long? Log_id { get; set; }
        public string Group_from { get; set; }
        public string Group_to { get; set; }
        public string Facility { get; set; }
        public DateTime Created_date { get; set; }
        public DateTime? Modified_date { get; set; }
        public string Modified_by { get; set; }
        public string Created_by { get; set; }
    }
}
