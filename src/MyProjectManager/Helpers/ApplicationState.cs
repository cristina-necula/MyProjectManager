using MyProjectManager.DAL;
using MyProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyProjectManager.Helpers
{
    public class ApplicationState
    {
        private static readonly ApplicationState instance = new ApplicationState();

        private ApplicationState() { }

        public static ApplicationState Instance
        {
            get
            {
                return instance;
            }
        }

        public User CurrentUser { get; set; }
        public int CurrentProjectID { get; set; }

        public Project CurrentProject {
            get
            {
                if (CurrentProjectID <= 0)
                    return null;

                using (var dbContext = new ProjectManagerContext())
                {
                    return dbContext.Projects.Where(p => p.ID == CurrentProjectID).FirstOrDefault();
                }
            }
        }

        public bool IsUserAsignedToCurrentProject(int userID)
        {
            var projectID = CurrentProjectID;
            using (var dbContext = new ProjectManagerContext())
            {
                var user = dbContext.Users.Where(u => u.ID == userID).FirstOrDefault();
                if (user.UserRole == Enums.UserRole.AccountsAdministrator)
                {
                    return true;
                }
                return dbContext.ProjectManagers.Where(x => x.ProjectID == projectID && x.ProjectManagerID == userID).Any() ||
                    dbContext.ProjectMembers.Where(x => x.ProjectID == projectID && x.ProjectMemberID == userID).Any();
            }
        }

        public List<SelectListItem> Projects
        {
            get
            {
                using(var dbContext = new ProjectManagerContext())
                {
                    var projects = dbContext.Projects.ToList();
                    var selectList = new List<SelectListItem>();
                    selectList.Add(
                        new SelectListItem
                        {
                            Text = "Select project",
                            Value = "0",
                            Selected = 0 == CurrentProjectID
                        });
                    foreach (var project in projects)
                    {
                        selectList.Add(
                            new SelectListItem
                            {
                                Text = project.Name,
                                Value = project.ID.ToString(),
                                Selected = project.ID == CurrentProjectID
                            });
                    }
                    
                    return selectList;
                }
            }
        }

        public void Clear()
        {
            if(CurrentUser != null)
            {
                CurrentUser = null;
            }
            CurrentProjectID = 0;
        }
    }
}