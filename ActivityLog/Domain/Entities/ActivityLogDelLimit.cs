using IJOS.Domain.Common;

namespace IJOS.Domain.Entities
{
    public class ActivityLogDelLimit : AuditableEntity
    {
        public long? DelLimit { get; set; }
        public string Facility { get; set; }
    }
}