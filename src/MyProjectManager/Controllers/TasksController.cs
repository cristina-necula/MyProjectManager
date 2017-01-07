using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyProjectManager.DAL;
using MyProjectManager.Models;
using MyProjectManager.Helpers;
using MyProjectManager.ViewModels;

namespace MyProjectManager.Controllers
{
    public class TasksController : Controller
    {
        private ProjectManagerContext db = new ProjectManagerContext();

        // GET: Tasks
        public ActionResult Index()
        {
            var tasksVM = new List<TaskViewModel>();
            var sprints = db.Sprints.Where(s => s.ProjectID == ApplicationState.Instance.CurrentProjectID).ToList();
            var allTasks = db.Tasks.ToList();
            foreach(var sprint in sprints)
            {
                var taskVM = new TaskViewModel
                {
                    SprintName = sprint.Name
                };

                var sprintTasks = allTasks.Where(t => t.SprintID == sprint.ID).ToList();
                taskVM.Tasks = sprintTasks;

                tasksVM.Add(taskVM);
            }
            return View(tasksVM);
        }

        // GET: Tasks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.Tasks.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            
            return View(task);
        }

        // GET: Tasks/Create
        public ActionResult Create()
        {
            var projectSprints = db.Sprints.Where(s => s.ProjectID == ApplicationState.Instance.CurrentProjectID && s.ActualFinishDate == null).ToList();
            ViewBag.ProjectSprints = new SelectList(projectSprints, "Id", "Name");
            SetProjectUsersInViewBag();
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,SprintID,ResposibleUserID,Status,Summary,EstimatedEffort,ConsumedEffort,Component")] Task task)
        {
            if (ModelState.IsValid)
            {
                db.Tasks.Add(task);
                db.SaveChanges();
                var activityDescription = ApplicationState.Instance.CurrentUser.FirstName + " "
                    + ApplicationState.Instance.CurrentUser.LastName + " created task - " + task.Summary;
                new ActivityMonitorUpdater(db).WriteToDatabase(activityDescription, ApplicationState.Instance.CurrentProjectID);
                return RedirectToAction("Index");
            }

            return View(task);
        }

        // GET: Tasks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.Tasks.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            SetProjectUsersInViewBag();
            return View(task);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Status,ResposibleUserID,Summary,ConsumedEffort")] Task task)
        {
            if (ModelState.IsValid)
            {
                var dbTask = db.Tasks.Where(t => t.ID == task.ID).FirstOrDefault();
                dbTask.Status = task.Status;
                dbTask.ResposibleUserID = task.ResposibleUserID;
                dbTask.Summary = task.Summary;
                dbTask.ConsumedEffort = task.ConsumedEffort;

                db.SaveChanges();
                var activityDescription = ApplicationState.Instance.CurrentUser.FirstName + " "
                    + ApplicationState.Instance.CurrentUser.LastName + " edited task - " + task.Summary;
                new ActivityMonitorUpdater(db).WriteToDatabase(activityDescription, ApplicationState.Instance.CurrentProjectID);
                return RedirectToAction("Index");
            }
            return View(task);
        }

        // GET: Tasks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = db.Tasks.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Task task = db.Tasks.Find(id);
            db.Tasks.Remove(task);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void SetProjectUsersInViewBag()
        {
            var projectMembers = db.ProjectMembers.Where(x => x.ProjectID == ApplicationState.Instance.CurrentProjectID);
            var projectManager = db.ProjectManagers.Where(x => x.ProjectID == ApplicationState.Instance.CurrentProjectID).FirstOrDefault();

            var users = db.Users.Where(u => projectMembers.Any(p => p.ProjectMemberID == u.ID)).ToList();
            users.Add(db.Users.Where(u => u.ID == projectManager.ProjectManagerID).FirstOrDefault());

            ViewBag.ProjecUsers = new SelectList(users, "Id", "Username");
        }
    }
}
