using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyProjectManager.Models;
using MyProjectManager.DAL;
using MyProjectManager.Helpers;
using MyProjectManager.ViewModels;

namespace MyProjectManager.Controllers
{
    public class SprintsController : Controller
    {
        private ProjectManagerContext dbContext = new ProjectManagerContext();

        // GET: Sprints/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "Sprint,Tasks")] SprintViewModel sprintVM)
        {
            if (ModelState.IsValid)
            {
                var isSprintValid = DbEntityValidator.CheckSprint(sprintVM.Sprint);
                if (!string.IsNullOrEmpty(isSprintValid))
                {
                    return FailedAction(isSprintValid, sprintVM);
                }

                foreach(var task in sprintVM.Tasks)
                {
                    var isTaskValid = DbEntityValidator.CheckTask(task);
                    if (!string.IsNullOrEmpty(isTaskValid))
                    {
                        return FailedAction(isTaskValid, sprintVM);
                    }
                }

                var sprint = new Sprint
                {
                    ProjectID = ApplicationState.Instance.CurrentProjectID,
                    StartDate = sprintVM.Sprint.StartDate,
                    EstimatedFinishDate = sprintVM.Sprint.EstimatedFinishDate,
                    Description = sprintVM.Sprint.Description,
                    Milestone = sprintVM.Sprint.Milestone,
                    Name = sprintVM.Sprint.Name,
                    ActualFinishDate = null
                };
                dbContext.Sprints.Add(sprint);
                dbContext.SaveChanges();

                var activityDescription = ApplicationState.Instance.CurrentUser.FirstName + " "
                    + ApplicationState.Instance.CurrentUser.LastName
                    + " created new sprint - " + sprint.Name;
                new ActivityMonitorUpdater(dbContext).WriteToDatabase(activityDescription, ApplicationState.Instance.CurrentProjectID);

                var tasks = new List<Task>();
                foreach(var task in sprintVM.Tasks)
                {
                    task.SprintID = sprint.ID;
                    activityDescription = ApplicationState.Instance.CurrentUser.FirstName + " "
                        + ApplicationState.Instance.CurrentUser.LastName
                        + " created new task - " + task.Summary;
                    new ActivityMonitorUpdater(dbContext).WriteToDatabase(activityDescription, ApplicationState.Instance.CurrentProjectID);
                }
                dbContext.Tasks.AddRange(sprintVM.Tasks);
                dbContext.SaveChanges();

                return RedirectToAction("Index", "Program");
            }
            return FailedAction("Sprint is not valid, please check all fields", sprintVM);
        }

        public ActionResult AddNewTask()
        {
            var projectMembers = dbContext.ProjectMembers.Where(x => x.ProjectID == ApplicationState.Instance.CurrentProjectID);
            var projectManager = dbContext.ProjectManagers.Where(x => x.ProjectID == ApplicationState.Instance.CurrentProjectID).FirstOrDefault();

            var users = dbContext.Users.Where(u => projectMembers.Any(p => p.ProjectMemberID == u.ID)).ToList();
            users.Add(dbContext.Users.Where(u => u.ID == projectManager.ProjectManagerID).FirstOrDefault());

            var partialView = PartialView("~/Views/EditorTemplates/TaskEditor.cshtml", new Task());
            partialView.ViewBag.Users = new SelectList(users, "Id", "Username");

            return partialView;
        }

        private ViewResult FailedAction(string message, SprintViewModel sprintVM)
        {
            TempData[Constants.NOTICE] = message;
            sprintVM.Tasks = new List<Task>();
            return View(sprintVM);
        }
    }
}