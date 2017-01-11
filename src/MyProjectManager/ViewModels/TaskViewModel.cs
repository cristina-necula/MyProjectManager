using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyProjectManager.Models;

namespace MyProjectManager.ViewModels
{
    public class TaskViewModel
    {
        public string SprintName { get; set; }
        public List<Task> Tasks { get; set; }

        public bool CanBeEdited { get; set; }
    }
}