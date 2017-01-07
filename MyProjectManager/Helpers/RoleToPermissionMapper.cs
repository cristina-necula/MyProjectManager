using MyProjectManager.Enums;
using System;
using System.Collections.Generic;
using MyProjectManager.DAL;
using MyProjectManager.Models;
using System.Linq;

namespace MyProjectManager.Helpers
{
	public class RoleToPermissionMapper
	{
		private static readonly Lazy<RoleToPermissionMapper> lazyInstance =
			new Lazy<RoleToPermissionMapper>(() => new RoleToPermissionMapper());

		public static RoleToPermissionMapper Instance
		{
			get
			{
				return lazyInstance.Value;
			}
		}

		public Dictionary<UserRole, List<Permission>> PermissionsDictionary = new Dictionary<UserRole, List<Permission>>();

        public static bool CurrentUserHasAccess(Permission permission)
        {
            using(var dbContext = new ProjectManagerContext())
            {
                var user = ApplicationState.Instance.CurrentUser;
                var project = ApplicationState.Instance.CurrentProject;
                var userPermissions = Instance.PermissionsDictionary[user.UserRole.Value];

                var projectMembers = dbContext.ProjectMembers.Where(p => p.ProjectID == project.ID).Select(u => u.ProjectMemberID).ToList();
                projectMembers.Add(dbContext.ProjectManagers.Where(p => p.ProjectID == project.ID).FirstOrDefault().ProjectManagerID);

                if(projectMembers.Where(id => id == user.ID).Any() && userPermissions.Contains(permission))
                {
                    return true;
                }
                return false;
            }
        }

		private RoleToPermissionMapper()
		{
			PermissionsDictionary.Add(UserRole.AccountsAdministrator,
				new List<Permission>
                {
                    Permission.CreateUser,
                    Permission.CreateProject
                });

			PermissionsDictionary.Add(UserRole.ProjectManager,
				new List<Permission>
                {
                    Permission.CreateProject,
                    Permission.CreateUser,
                    Permission.CreateSprint,
				    Permission.CreateTask,
                    Permission.AlocateUser,
                    Permission.EditTask,
                    Permission.EstablishMilestone
                });

			PermissionsDictionary.Add(UserRole.Developer,
				new List<Permission>
                {
                    Permission.AlocateUser,
                    Permission.EditTask
                });

			PermissionsDictionary.Add(UserRole.QualityAssurance,
				new List<Permission>
                {
                    Permission.AlocateUser,
                    Permission.EditTask
                });

			PermissionsDictionary.Add(UserRole.Tester,
				new List<Permission>
                {
                    Permission.AlocateUser,
                    Permission.EditTask
                });
		}
	}
}