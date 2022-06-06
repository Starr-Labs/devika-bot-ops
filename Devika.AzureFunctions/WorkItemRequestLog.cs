using System;

namespace Devika.DevOps.WorkItems
{ 
    public class WorkItemRequestLog 
    {
        public int Id { get; set; }
        public string CreatorId { get; set; }
        public int WorkItemId { get; set; }
        public string WorkItemTitle { get; set; }
        public string WorkItemUrl { get; set; }
        public DateTime RequestTimeStamp { get; set; }
    }
}