using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyProjectManager.Models;

namespace MyProjectManager.ViewModels
{
    public class SprintViewModel
    {
        public Sprint Sprint { get; set; }
        public List<Task> Tasks { get; set; }
    }
}