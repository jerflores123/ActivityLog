using IJOS.Application.Common.Mappings;
using System;
using e = IJOS.Domain.Entities;

namespace IJOS.Application.ActivityLog.dtos
{
    public class ActivityLogRadios : IMapFrom<e.ActivityLogRadio>
    {
        public long? Radio_no { get; set; }
        public long? Key_no { get; set; }
        public long? Returned { get; set; }
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public string Facility { get; set; }
        public long? Button_no { get; set; }
        public DateTime Created_date { get; set; }
        public DateTime? Modified_date { get; set; }
        public string Modified_by { get; set; }
        public string Created_by { get; set; }
    }
}
