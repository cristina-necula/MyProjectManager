using MyProjectManager.Enums;
using System.Collections.Generic;

namespace MyProjectManager.Models
{
	public class Task
	{
		public int ID { get; set; }
        public int SprintID { get; set; }
		public Status Status { get; set; }
		public int ResposibleUserID { get; set; }
		public string Summary { get; set; }
		public int EstimatedEffort { get; set; }
		public int ConsumedEffort { get; set; }
		public Component Component { get; set; }
	}
}