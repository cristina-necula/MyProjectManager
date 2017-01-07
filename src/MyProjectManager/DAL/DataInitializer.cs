using MyProjectManager.Enums;
using MyProjectManager.Models;
using System.Collections.Generic;

namespace MyProjectManager.DAL
{
	public class DataInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<ProjectManagerContext>
	{
		protected override void Seed(ProjectManagerContext context)
		{
			CreateDefaultUsers().ForEach(s => context.Users.Add(s));
			context.SaveChanges();
		}

		private List<User> CreateDefaultUsers()
		{
			return new List<User>
			{
				new User
				{
					FirstName="Admin",
					LastName="Admin",
					Username="admin",
					Password="admin",
					Email="admin@email.com",
					UserRole=UserRole.AccountsAdministrator,
					LastLogin = null
				},
				new User
				{
					FirstName="Alexandru",
					LastName="Florea",
					Username="alexandru.florea",
					Password="password",
					Email="alexandru.florea@email.com",
					UserRole=UserRole.ProjectManager,
					LastLogin = null
				},
                new User
                {
                    FirstName="Mircea",
                    LastName="Manole",
                    Username="mircea.manole",
                    Password="password",
                    Email="mircea.manole@email.com",
                    UserRole=UserRole.ProjectManager,
                    LastLogin = null
                },
                new User
                {
                    FirstName="Mark",
                    LastName="Smith",
                    Username="mark.smith",
                    Password="password",
                    Email="mark.smith@email.com",
                    UserRole=UserRole.Developer,
                    LastLogin = null
                },
                new User
                {
                    FirstName="Ana",
                    LastName="Popescu",
                    Username="ana.popescu",
                    Password="password",
                    Email="ana.popescu@email.com",
                    UserRole=UserRole.Developer,
                    LastLogin = null
                },
                new User
                {
                    FirstName="Maria",
                    LastName="Ionescu",
                    Username="maria.ionescu",
                    Password="password",
                    Email="maria.ionescu@email.com",
                    UserRole=UserRole.Developer,
                    LastLogin = null
                },
                new User
                {
                    FirstName="Dan",
                    LastName="Stefanescu",
                    Username="dan.stefanescu",
                    Password="password",
                    Email="dan.stefanescu@email.com",
                    UserRole=UserRole.Developer,
                    LastLogin = null
                },
                new User
                {
                    FirstName="Daniel",
                    LastName="Popa",
                    Username="daniel.popa",
                    Password="password",
                    Email="daniel.popa@email.com",
                    UserRole=UserRole.QualityAssurance,
                    LastLogin = null
                },
                new User
                {
                    FirstName="Razvan",
                    LastName="Branza",
                    Username="razvan.branza",
                    Password="password",
                    Email="razvan.branza@email.com",
                    UserRole=UserRole.QualityAssurance,
                    LastLogin = null
                },
                new User
                {
                    FirstName="Stefan",
                    LastName="Popa",
                    Username="stefan.popa",
                    Password="password",
                    Email="stefan.popa@email.com",
                    UserRole=UserRole.Tester,
                    LastLogin = null
                },
                new User
                {
                    FirstName="Mihaela",
                    LastName="Dumitrescu",
                    Username="mihaela.dumitrescu",
                    Password="password",
                    Email="mihaela.dumitrescu@email.com",
                    UserRole=UserRole.Tester,
                    LastLogin = null
                }
            };
		}
	}
}