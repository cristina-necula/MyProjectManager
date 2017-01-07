using MyProjectManager.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyProjectManager.Models;
using MyProjectManager.ViewModels;
using MyProjectManager.Helpers;

namespace MyProjectManager.Controllers
{
    public class ProgramController : Controller
    {
        private ProjectManagerContext db = new ProjectManagerContext();

        // GET: Program
        [HttpGet]
        public ActionResult Index()
        {
            var allSprints = db.Sprints.Where(s => s.ProjectID == ApplicationState.Instance.CurrentProjectID);
            var sprints = allSprints.Where(s => s.ActualFinishDate == null);
            var milestones = allSprints.Where(s => s.ActualFinishDate != null);

            var programVM = new ProgramViewModel();
            programVM.Sprints = sprints.ToList();
            programVM.Milestones = milestones.ToList();

            return View(programVM);
        }

        public ActionResult FinishSprint(int sprintID)
        {
            var sprint = db.Sprints.Where(s => s.ID == sprintID).FirstOrDefault();
            sprint.ActualFinishDate = DateTime.Now;
            db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}
