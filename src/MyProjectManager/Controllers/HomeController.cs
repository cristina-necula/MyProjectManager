using MyProjectManager.DAL;
using MyProjectManager.Helpers;
using MyProjectManager.Models;
using MyProjectManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MyProjectManager.Controllers
{
	public class HomeController : Controller
	{
		private ProjectManagerContext dbContext = new ProjectManagerContext();

		public ActionResult Index()
		{
            if(ApplicationState.Instance.CurrentUser != null)
            {
                return RedirectToAction("Activity");
            }
			return View();
		}

		public ActionResult Login()
		{
            ApplicationState.Instance.Clear();
			return View();
		}

		public ActionResult Signin()
		{
            ApplicationState.Instance.Clear();
            return View();
		}

        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult Activity()
        {
            ModelState.Clear();
            return View(GetActivitiesList());
        }

        [HttpPost]
        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public string UpdateCurrentProject(int projectID)
        {
            ApplicationState.Instance.CurrentProjectID = projectID;
            return "reloadPage";
        }

        // POST: Users/Login
        [HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ValidateLogin([Bind(Include = "Username,Password")] User user)
		{
			if (ModelState.IsValid)
			{
				User dbUser = dbContext.Users.ToList()
                    .Where(u => u.Username.Equals(user.Username) && u.Password.Equals(user.Password))
                    .FirstOrDefault();

				if (dbUser == null)
				{
					TempData[Constants.NOTICE] = "Invalid username or password! Please try again!";
					return RedirectToAction("Login");
				}

				dbUser.LastLogin = DateTime.UtcNow;
                ApplicationState.Instance.CurrentUser = dbUser;

				dbContext.Entry(dbUser).State = System.Data.Entity.EntityState.Modified;
				dbContext.SaveChanges();

				return RedirectToAction("Activity");
			}
			return RedirectToAction("Login");
		}

        // POST: Users/Signin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ValidateSignin([Bind(Include = "FirstName,LastName,Username,Email,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                User dbUser = dbContext.Users.ToList().Where(u => u.Email.Equals(user.Email) || u.Username.Equals(user.Username)).FirstOrDefault();

				if (dbUser == null)
				{
					return FailedSignIn("Your user has not been added yet in the system by an account administrator.");
				}

				if (dbUser.UserRole == null)
				{
					return FailedSignIn("Your account administrator has not set an user role for your account yet.");
				}

				if (string.IsNullOrEmpty(user.FirstName) ||
					string.IsNullOrEmpty(user.LastName) ||
					string.IsNullOrEmpty(user.Password))
				{
                    return FailedSignIn("Please complete all the fields in order to access your account");
				}

				UpdateDbEntry(dbUser, user);

                ApplicationState.Instance.CurrentUser = dbUser;

                return RedirectToAction("Activity");
            }
			return RedirectToAction("Signin");
		}

		private RedirectToRouteResult FailedSignIn(string message)
		{
			TempData[Constants.NOTICE] = message;
            return RedirectToAction("Signin");
		}

		private void UpdateDbEntry(User userEntry, User userView)
		{
			userEntry.Email = userView.Email;
			userEntry.FirstName = userView.FirstName;
			userEntry.LastName = userView.LastName;
			userEntry.Username = userView.Username;
			userEntry.Password = userView.Password;
			userEntry.LastLogin = DateTime.UtcNow;

			dbContext.Entry(userEntry).State = System.Data.Entity.EntityState.Modified;
			dbContext.SaveChanges();
		}

        private List<ActivityViewModel> GetActivitiesList()
        {
            var activities = dbContext.Activities.OrderByDescending(a => a.Timestamp).ToList();
            var activityViewModels = new List<ActivityViewModel>();
            var projects = dbContext.Projects.ToList();

            if (ApplicationState.Instance.CurrentProjectID > 0)
            {
                var currentProjectActivities = activities.Where(a => a.ProjectID == ApplicationState.Instance.CurrentProjectID).ToList();
                activityViewModels.Add(new ActivityViewModel
                {
                    ProjectName = projects.Where(p => p.ID == ApplicationState.Instance.CurrentProjectID).FirstOrDefault().Name,
                    Activities = currentProjectActivities
                });
                return activityViewModels;
            }

            foreach (var activity in activities)
            {
                var project = projects.Where(p => p.ID == activity.ProjectID).FirstOrDefault();
                activityViewModels.Add(new ActivityViewModel
                {
                    ProjectName = project?.Name,
                    Activities = new List<ActivityMonitor> { activity }
                });
            }
            return activityViewModels;
        }

	}
}