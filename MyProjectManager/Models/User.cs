using MyProjectManager.Enums;
using System;

namespace MyProjectManager.Models
{
	public class User
	{
		public int ID { get; set; }
		public string Username { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Password { get; set; }
		public string Email { get; set; }
		public DateTime? LastLogin { get; set; }
		public UserRole? UserRole { get; set; }
	}
}