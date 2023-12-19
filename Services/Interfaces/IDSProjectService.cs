using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dsana.Models;

namespace Dsana.Services.Interfaces
{
    public interface IDSProjectService
    {
        public Task AddNewProjectAsync(Project project);

        public Task<bool> AddProjectManagerAsync(string userId, int projectId);

        public Task<bool> AddUserToProjectAsync(string userId, int projectId);

        public Task ArchiveProjectAsync(Project project);

        public Task<List<Project>> GetAllProjectsByCompanyAsync(int companyId);

        public Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName);

        public Task<List<DSUser>> GetAllProjectMembersExceptPMAsync(int projectId);

        public Task<List<Project>> GetArchivedProjectsByCompanyAsync(int companyId);

        public Task<DSUser> GetProjectManagerAsync(int projectId);

        public Task<List<DSUser>> GetProjectMembersByRoleAsync(int projectId, string role);

        public Task<Project> GetProjectByIdAsync(int projectId, int companyId);

        public Task<List<Project>> GetUnassignedProjectsAsync(int companyId);

        public Task<List<Project>> GetUserProjectsAsync(string userId);

        public Task<bool> IsAssignedProjectManagerAsync(string userId, int projectId);

        public Task<bool> IsUserOnProjectAsync(string userId, int projectId);

        public Task<int> LookupProjectPriorityId(string priorityName);

        public Task RemoveProjectManagerAsync(int projectId);

        public Task RemoveUserFromProjectAsync(string userId, int projectId);

        public Task RestoreProjectAsync(Project project);

        public Task UpdateProjectAsync(Project project);
    }
}
