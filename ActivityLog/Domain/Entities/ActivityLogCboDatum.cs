using IJOS.Domain.Common;

namespace IJOS.Domain.Entities
{
    public class ActivityLogCboDatum : AuditableEntity
    {
        public string CboType { get; set; }
        public string CboData { get; set; }
        public string Facility { get; set; }
    }
}