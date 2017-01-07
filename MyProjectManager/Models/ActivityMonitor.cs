using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyProjectManager.Models
{
    public class ActivityMonitor
    {
        public int ID { get; set; }
        public DateTime? Timestamp { get; set; }
        public string Description { get; set; }
        public int ProjectID { get; set; }
    }
}