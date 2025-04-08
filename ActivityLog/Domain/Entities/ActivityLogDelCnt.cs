using IJOS.Domain.Common;
using System;

namespace IJOS.Domain.Entities
{
    public class ActivityLogDelCnt : AuditableEntity
    {
        public DateTime? DelDate { get; set; }
        public long? DelCnt { get; set; }
        public string Facility { get; set; }
    }
}