using IJOS.Domain.Common;

namespace IJOS.Domain.Entities
{
    public class ActivityLogRadio : AuditableEntity
    {
        public long? RadioNo { get; set; }
        public long? KeyNo { get; set; }
        public long? Returned { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Facility { get; set; }
        public long? ButtonNo { get; set; }
    }
}