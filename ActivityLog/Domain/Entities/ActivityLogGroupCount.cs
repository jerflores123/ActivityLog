using IJOS.Domain.Common;

namespace IJOS.Domain.Entities
{
    public class ActivityLogGroupCount : AuditableEntity
    {
        public string GroupName { get; set; }
        public long? GroupCounts { get; set; }
        public string Facility { get; set; }
    }
}