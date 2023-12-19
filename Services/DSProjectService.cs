using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Dsana.Data;
using Dsana.Models;
using Dsana.Models.Enums;
using Dsana.Services.Interfaces;

namespace Dsana.Services
{
    public class DSProjectService : IDSProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<DSUser> _userManager;
        private readonly IDSRolesService _rolesService;

        public DSProjectService(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<DSUser> userManager,
            IDSRolesService rolesService)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _rolesService = rolesService;
        }

        public async Task AddNewProjectAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            DSUser currentPM = await GetProjectManagerAsync(projectId);

            if(currentPM != null)
            {
                try
                {
                    await RemoveProjectManagerAsync(projectId);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error removing current PM. - Error: {ex.Message}");
                    return false;
                }
            }

            try
            {
                await AddUserToProjectAsync(userId, projectId);
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error adding new PM. - Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> AddUserToProjectAsync(string userId, int projectId)
        {
            DSUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if(user != null)
            {
                Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
                if(!await IsUserOnProjectAsync(userId, projectId))
                {
                    try
                    {
                        project.Members.Add(user);
                        await _context.SaveChangesAsync();
                        return true;
                    }
                    catch(Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }

        public async Task ArchiveProjectAsync(Project project)
        {
            try
            {
                project.Archived = true;
                await UpdateProjectAsync(project);

                foreach(DTask dtask in project.DTasks)
                {
                    dtask.ArchivedByProject = true;
                    _context.Update(dtask);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            
        }

        public async Task<List<DSUser>> GetAllProjectMembersExceptPMAsync(int projectId)
        {
            List<DSUser> developers = await GetProjectMembersByRoleAsync(projectId, Roles.Developer.ToString());
            List<DSUser> submitters = await GetProjectMembersByRoleAsync(projectId, Roles.Submitter.ToString());
            List<DSUser> admins = await GetProjectMembersByRoleAsync(projectId, Roles.Admin.ToString());

            List<DSUser> teamMembers = developers.Concat(submitters).Concat(admins).ToList();

            return teamMembers;
        }
        public async Task<List<Project>> GetAllProjectsByCompanyAsync(int companyId)
        {
            try
            {

                List<Project> projects = new();

                projects = await _context.Projects.Where(p => p.CompanyId == companyId && p.Archived == false)
                    .Include(p => p.Members)
                    .Include(p => p.DTasks)
                        .ThenInclude(t => t.Comments)
                    .Include(p => p.DTasks)
                        .ThenInclude(t => t.Attachments)
                    .Include(p => p.DTasks)
                        .ThenInclude(t => t.History)
                    .Include(p => p.DTasks)
                        .ThenInclude(t => t.DeveloperUser)
                    .Include(p => p.DTasks)
                        .ThenInclude(t => t.OwnerUser)
                    .Include(p => p.DTasks)
                        .ThenInclude(t => t.DTaskStatus)
                    .Include(p => p.DTasks)
                        .ThenInclude(t => t.DTaskPriority)
                    .Include(p => p.DTasks)
                        .ThenInclude(t => t.DTaskType)
                    .Include(p => p.ProjectPriority)
                    .ToListAsync();

                return projects;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
        {
            List<Project> projects = await GetAllProjectsByCompanyAsync(companyId);

            int priorityId = await LookupProjectPriorityId(priorityName);

            return projects.Where(p => p.ProjectPriorityId == priorityId).ToList();
        }

        public async Task<List<Project>> GetArchivedProjectsByCompanyAsync(int companyId)
        {
            try
            {

                List<Project> projects = await _context.Projects.Where(p => p.CompanyId == companyId && p.Archived == true)
                            .Include(p => p.Members)
                            .Include(p => p.DTasks)
                                .ThenInclude(t => t.Comments)
                            .Include(p => p.DTasks)
                                .ThenInclude(t => t.Attachments)
                            .Include(p => p.DTasks)
                                .ThenInclude(t => t.History)
                            .Include(p => p.DTasks)
                                .ThenInclude(t => t.DeveloperUser)
                            .Include(p => p.DTasks)
                                .ThenInclude(t => t.OwnerUser)
                            .Include(p => p.DTasks)
                                .ThenInclude(t => t.DTaskStatus)
                            .Include(p => p.DTasks)
                                .ThenInclude(t => t.DTaskPriority)
                            .Include(p => p.DTasks)
                                .ThenInclude(t => t.DTaskType)
                            .Include(p => p.ProjectPriority)
                            .ToListAsync();

                return projects;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
        {
            //Project project = await _context.Projects
            //    .Include(p => p.DTasks)
            //    .Include(p => p.Members)
            //    .Include(p => p.ProjectPriority)
            //    .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);

            //return project;

            Project project = await _context.Projects
                                .Include(p => p.DTasks)
                                    .ThenInclude(t => t.DTaskPriority)
                                .Include(p => p.DTasks)
                                    .ThenInclude(t => t.DTaskStatus)
                                .Include(p => p.DTasks)
                                    .ThenInclude(t => t.DTaskType)
                                .Include(p => p.DTasks)
                                    .ThenInclude(t => t.OwnerUser)
                                .Include(p => p.Members)
                                .Include(p => p.ProjectPriority)
                                .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);

            return project;
        }

        public async Task<DSUser> GetProjectManagerAsync(int projectId)
        {
            Project project = await _context.Projects
                                            .Include(p => p.Members)
                                            .FirstOrDefaultAsync(p => p.Id == projectId);

            foreach(DSUser member in project?.Members)
            {
                if(await _rolesService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
                {
                    return member;
                }
            }

            return null;
        }

        public async Task<List<DSUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            Project project = await _context.Projects
                                            .Include(p => p.Members)
                                            .FirstOrDefaultAsync(p => p.Id == projectId);

            List<DSUser> members = new();

            foreach(var user in project.Members)
            {
                if(await _rolesService.IsUserInRoleAsync(user,role))
                {
                    members.Add(user);
                }
            }

            return members;


        }

        public async Task<List<Project>> GetUnassignedProjectsAsync(int companyId)
        {
            List<Project> result = new();
            List<Project> projects = new();
            try
            {
                projects = await _context.Projects
                                         .Include(p => p.ProjectPriority)
                                         .Where(p => p.CompanyId == companyId).ToListAsync();
                foreach (Project project in projects)
                {
                    if ((await GetProjectMembersByRoleAsync(project.Id, Roles.ProjectManager.ToString())).Count == 0)
                    {
                        result.Add(project);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public async Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            try
            {
                List<Project> userProjects = (await _context.Users
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Company)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Members)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.DTasks)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.DTasks)
                            .ThenInclude(t => t.DeveloperUser)
                    .Include(u => u.Projects)
                        .ThenInclude(t => t.DTasks)
                            .ThenInclude(t => t.OwnerUser)
                    .Include(u => u.Projects)
                        .ThenInclude(t => t.DTasks)
                            .ThenInclude(t => t.DeveloperUser)
                    .Include(u => u.Projects)
                        .ThenInclude(t => t.DTasks)
                            .ThenInclude(t => t.DTaskStatus)
                    .Include(u => u.Projects)
                        .ThenInclude(t => t.DTasks)
                            .ThenInclude(t => t.DTaskType)
                    .FirstOrDefaultAsync(u => u.Id == userId)).Projects.ToList();

                return userProjects;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*** ERROR *** - Error Getting User Projects List.  ---> {ex.Message}");
                throw;
            }
        }

        public async Task<bool> IsAssignedProjectManagerAsync(string userId, int projectId)
        {
            try
            {
                string projectManagerId = (await GetProjectManagerAsync(projectId))?.Id;

                if (projectManagerId == userId)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
        {
            Project project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);

            bool result = false;

            if(project != null)
            {
                result = project.Members.Any(m => m.Id == userId);
            }
            return result;
        }

        public async Task<int> LookupProjectPriorityId(string priorityName)
        {
            int priorityId = (await _context.ProjectPriorities.FirstOrDefaultAsync(p => p.Name == priorityName)).Id;
            return priorityId;
        }

        public async Task RemoveProjectManagerAsync(int projectId)
        {
            Project project = await _context.Projects
                                            .Include(p => p.Members)
                                            .FirstOrDefaultAsync(p => p.Id == projectId);

            try
            {
                foreach(DSUser member in project?.Members)
                {
                    if(await _rolesService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
                    {
                        await RemoveUserFromProjectAsync(member.Id, projectId);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            try
            {
                DSUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

                try
                {
                    if (await IsUserOnProjectAsync(userId, projectId))
                    {
                        project.Members.Remove(user);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*** ERROR *** - Error Removing User From Project.  ---> {ex.Message}");
            }
        }


        public async Task RestoreProjectAsync(Project project)
        {
            try
            {
                project.Archived = false;
                await UpdateProjectAsync(project);

                foreach (DTask dtask in project.DTasks)
                {
                    dtask.ArchivedByProject = false;
                    _context.Update(dtask);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task UpdateProjectAsync(Project project)
        {
            _context.Update(project);
            await _context.SaveChangesAsync();
        }
    }
}
