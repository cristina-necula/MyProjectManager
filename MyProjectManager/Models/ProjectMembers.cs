using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyProjectManager.Models
{
    public class ProjectMembers
    {
        public int ID { get; set; }
        public int ProjectID { get; set; }
        public int ProjectMemberID { get; set; }
    }
}