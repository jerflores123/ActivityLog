using IJOS.Domain.Common;
using System;

namespace IJOS.Domain.Entities
{
    public class ActivityLog : AuditableEntity
    {
        public decimal LogId { get; set; }
        public DateTime? LogDate { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        public string EventType { get; set; }
        public bool IsActive { get; set; }
        public string Facility { get; set; }
    }
}