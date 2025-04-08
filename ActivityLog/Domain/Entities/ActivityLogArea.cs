using IJOS.Domain.Common;

namespace IJOS.Domain.Entities
{
    public class ActivityLogArea : AuditableEntity
    {
        public long? LogId { get; set; }
        public string GroupFrom { get; set; }
        public string GroupTo { get; set; }
        public string Facility { get; set; }
    }
}