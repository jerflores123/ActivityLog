using System.Collections.Generic;

namespace IJOS.Application.ActivityLog.dtos
{
    public class ActivityLogSearchVM
    {
        public string Search_String { get; set; }
        public List<ActivityLogdto> ActivityLog { get; set; }
    }
}
