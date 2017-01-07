using MyProjectManager.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyProjectManager.Models;

namespace MyProjectManager.Helpers
{
    public class ActivityMonitorUpdater
    {
        private ProjectManagerContext dbContext;

        public ActivityMonitorUpdater(ProjectManagerContext context)
        {
            dbContext = context;
        }

        public void WriteToDatabase(string description, int projectID)
        {
            var activityMonitor = new ActivityMonitor();
            activityMonitor.Timestamp = DateTime.Now;
            activityMonitor.ProjectID = projectID;
            activityMonitor.Description = description;

            dbContext.Activities.Add(activityMonitor);
            dbContext.SaveChanges();
        }
    }
}