﻿using MyProjectManager.DAL;
using MyProjectManager.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MyProjectManager.Helpers;
using System.Collections.Generic;

namespace MyProjectManager.Controllers
{
	public class UsersController : Controller
	{
		private ProjectManagerContext dbContext = new ProjectManagerContext();

		// GET: Users
		public ActionResult Index()
		{
            var users = dbContext.Users.ToList();
            var projects = dbContext.Projects.ToList();
            var projectMembers = dbContext.ProjectMembers.ToList();
            var projectManagers = dbContext.ProjectManagers.ToList();

            foreach(var user in users)
            {
                var involvedID = projectMembers.Where(p => p.ProjectMemberID == user.ID).Select(i => i.ProjectID).ToList();
                involvedID.AddRange(projectManagers.Where(p => p.ProjectManagerID == user.ID).Select(i => i.ProjectID));
                ViewData[user.Username] = string.Join(",", projects.Where(p => involvedID.Any(i => p.ID == i)).Select(p => p.Name));
            }

            users.RemoveAll(u => u.UserRole == Enums.UserRole.AccountsAdministrator);
            return View(users);
		}

		// GET: Users/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			User user = dbContext.Users.Find(id);
			if (user == null)
			{
				return HttpNotFound();
			}
			return View(user);
		}

		// GET: Users/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: Users/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "ID,Username,Email,UserRole")] User user)
		{
			if (ModelState.IsValid)
			{
                User dbUser = dbContext.Users.ToList()
                    .Where(u => u.Email == user.Email || u.Username == user.Username)
                    .FirstOrDefault();

                if(dbUser != null)
                {
                    return FailedAction("User with specified details already exists!", user);
                }

                if (!user.UserRole.HasValue)
                {
                    return FailedAction("Please specify a role for the new user!", user);
                }

				dbContext.Users.Add(user);
				dbContext.SaveChanges();

                var description = ApplicationState.Instance.CurrentUser.FirstName + " " + ApplicationState.Instance.CurrentUser.LastName + " added new user with username " 
                    + user.Username + " and role " + user.UserRole.ToString();
                new ActivityMonitorUpdater(dbContext).WriteToDatabase(description, -1);

				return RedirectToAction("Index");
			}

			return View(user);
		}

		// GET: Users/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			User user = dbContext.Users.Find(id);
			if (user == null)
			{
				return HttpNotFound();
			}
			return View(user);
		}

		// POST: Users/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "ID,Username,FirstName,LastName,Password,Email,UserRole")] User user)
		{
			if (ModelState.IsValid)
			{
				dbContext.Entry(user).State = EntityState.Modified;
				dbContext.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(user);
		}

		// GET: Users/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			User user = dbContext.Users.Find(id);
			if (user == null)
			{
				return HttpNotFound();
			}
			return View(user);
		}

		// POST: Users/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			User user = dbContext.Users.Find(id);
			dbContext.Users.Remove(user);
			dbContext.SaveChanges();
			return RedirectToAction("Index");
		}

        public ActionResult AddUserToCurrentProject(int id)
        {
            var user = dbContext.Users.Where(u => u.ID == id).FirstOrDefault();

            // Change project manager
            if (user.UserRole == Enums.UserRole.ProjectManager)
            {
                var projectManagers = dbContext.ProjectManagers;
                var pm = projectManagers
                    .Where(p => p.ProjectID == ApplicationState.Instance.CurrentProjectID).FirstOrDefault();
                if(pm != null)
                {
                    pm.ProjectManagerID = id;
                }
                else
                {
                    pm = new ProjectManagers
                    {
                        ProjectID = ApplicationState.Instance.CurrentProjectID,
                        ProjectManagerID = id
                    };
                    dbContext.ProjectManagers.Add(pm);
                }
                dbContext.SaveChanges();
            }
            // ad user as member to project
            else
            {
                var projectMembers = new ProjectMembers
                {
                    ProjectID = ApplicationState.Instance.CurrentProjectID,
                    ProjectMemberID = id
                };
                dbContext.ProjectMembers.Add(projectMembers);
                dbContext.SaveChanges();
            }
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult RemoveUserFromCurrentProject(int id)
        {
            var user = dbContext.Users.Where(u => u.ID == id).FirstOrDefault();
            // Remove project manager
            if (user.UserRole == Enums.UserRole.ProjectManager)
            {
                var projectManagers = dbContext.ProjectManagers;
                var pm = projectManagers
                    .Where(p => p.ProjectID == ApplicationState.Instance.CurrentProjectID && p.ProjectManagerID == id).FirstOrDefault();
                projectManagers.Remove(pm);
            }
            // Remove project member
            else
            {
                var projectMember = dbContext.ProjectMembers
                    .Where(p => p.ProjectMemberID == id && p.ProjectID == ApplicationState.Instance.CurrentProjectID).FirstOrDefault();
                dbContext.ProjectMembers.Remove(projectMember);
            }
            dbContext.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }

        protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				dbContext.Dispose();
			}
			base.Dispose(disposing);
		}

        private ViewResult FailedAction(string message, User user)
        {
            TempData[Constants.NOTICE] = message;
            return View(user);
        }
	}
}
