using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyProjectManager.Models;

namespace MyProjectManager.ViewModels
{
    public class ActivityViewModel
    {
        public string ProjectName { get; set; }
        
        public List<ActivityMonitor> Activities { get; set; }
    }
}