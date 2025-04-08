using IJOS.Application.Common.Mappings;
using System;
using e = IJOS.Domain.Entities;

namespace IJOS.Application.ActivityLog.dtos
{
    public class ActivityLogdto : IMapFrom<e.ActivityLog>
    {
        public decimal Log_id { get; set; }
        public DateTime? Log_date { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        public string Event_type { get; set; }
        public long? Is_active { get; set; }
        public string Facility { get; set; }
        public DateTime Created_date { get; set; }
        public DateTime? Modified_date { get; set; }
        public string Modified_by { get; set; }
        public string Created_by { get; set; }
    }
}