using IJOS.Domain.Common;
using System;

namespace IJOS.Domain.Entities
{
    public class ActivityLogDevicePriv : AuditableEntity
    {
        public string DeviceName { get; set; }
        public string ApplicationName { get; set; }
        public string AppPrivilege { get; set; }
        public long? StaffKey { get; set; }
        public string DeviceType { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string UpdatedBy { get; set; }
        public string AgencyName { get; set; }
    }
}