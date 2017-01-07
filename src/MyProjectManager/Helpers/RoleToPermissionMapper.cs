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

                if(user == null || project == null)
                {
                    return false;
                }

                var userPermissions = Instance.PermissionsDictionary[user.UserRole.Value];

                if (permission == Permission.AllocateUser && userPermissions.Contains(Permission.AllocateUser))
                {
                    return true;
                }

                if(permission == Permission.CreateUser && userPermissions.Contains(Permission.CreateUser))
                {
                    return true;
                }

                var projectMembers = dbContext.ProjectMembers.Where(p => p.ProjectID == project.ID).Select(u => u.ProjectMemberID).ToList();
                var projectManagers = dbContext.ProjectManagers.Where(p => p.ProjectID == project.ID).ToList();
                if(projectManagers.Count > 0)
                {
                    projectMembers.AddRange(projectManagers.Select(p => p.ProjectManagerID).ToList());
                }
                
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
                    Permission.AllocateUser,
                    Permission.EditTask
                });

			PermissionsDictionary.Add(UserRole.Developer,
				new List<Permission>
                {
                    Permission.EditTask
                });

			PermissionsDictionary.Add(UserRole.QualityAssurance,
				new List<Permission>
                {
                    Permission.EditTask
                });

			PermissionsDictionary.Add(UserRole.Tester,
				new List<Permission>
                {
                    Permission.EditTask
                });
		}
	}
}