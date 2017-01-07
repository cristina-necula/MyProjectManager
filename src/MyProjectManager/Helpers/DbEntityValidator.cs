using MyProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyProjectManager.Helpers
{
    public static class DbEntityValidator
    {
        public static string CheckTask(Task task)
        {
            if (task.EstimatedEffort == 0)
            {
                return "Please enter estimated effort";
            }
            if (task.ResposibleUserID <= 0)
            {
                return "Please enter responsible user";
            }
            if (string.IsNullOrEmpty(task.Summary))
            {
                return "Please enter task summary";
            }
            return null;
        }

        public static string CheckSprint(Sprint sprint)
        {
            if (sprint.StartDate == null)
            {
                return "Please enter a start date";
            }
            if (sprint.EstimatedFinishDate == null)
            {
                return "Please enter an estimated finish date";
            }
            if (sprint.StartDate > sprint.EstimatedFinishDate)
            {
                return "Estimated finnish date cannot be smaller than start date";
            }
            if (string.IsNullOrEmpty(sprint.Name) || string.IsNullOrWhiteSpace(sprint.Name))
            {
                return "Please enter a name";
            }
            if (string.IsNullOrEmpty(sprint.Description) || string.IsNullOrWhiteSpace(sprint.Description))
            {
                return "Please enter a description";
            }
            if (string.IsNullOrEmpty(sprint.Milestone) || string.IsNullOrWhiteSpace(sprint.Milestone))
            {
                return "Please enter a milestone";
            }
            return null;
        }
    }
}