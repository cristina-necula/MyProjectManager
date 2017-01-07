using System;
using System.Collections.Generic;

namespace MyProjectManager.Models
{
	public class Sprint
	{
		public int ID { get; set; }
        public int ProjectID { get; set; }
		public DateTime? StartDate { get; set; }
        public DateTime? EstimatedFinishDate { get; set; }
        public DateTime? ActualFinishDate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Milestone { get; set; }
	}
}