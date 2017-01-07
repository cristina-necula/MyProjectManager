using MyProjectManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyProjectManager.ViewModels
{
    public class ProjectViewModel
    {
        public ProjectViewModel()
        {
            TeamMembers = new List<User>();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public int ProjectManagerID { get; set; }
        public List<User> TeamMembers { get; set; }
    }
}