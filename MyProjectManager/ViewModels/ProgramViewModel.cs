using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyProjectManager.Models;

namespace MyProjectManager.ViewModels
{
    public class ProgramViewModel
    {
        public ProgramViewModel()
        {
            Sprints = new List<Sprint>();
            Milestones = new List<Sprint>();
        }

        public List<Sprint> Sprints { get; set; }
        public List<Sprint> Milestones { get; set; }
    }
}