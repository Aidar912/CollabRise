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
    public class DSDTaskService : IDSDTaskService
    {

        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<DSUser> _userManager;
        private readonly IDSRolesService _rolesService;
        private readonly IDSProjectService _projectService;


        public DSDTaskService(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<DSUser> userManager, IDSRolesService rolesService, IDSProjectService projectService)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _rolesService = rolesService;
            _projectService = projectService;
        }

        public async Task AddNewDTaskAsync(DTask dtask)
        {
            try
            {
                _context.Add(dtask);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task AddDTaskAttachmentAsync(DTaskAttachment dtaskAttachment)
        {
            try
            {
                await _context.AddAsync(dtaskAttachment);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task AddDTaskCommentAsync(DTaskComment dtaskComment)
        {
            try
            {
                await _context.AddAsync(dtaskComment);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task AssignDTaskAsync(int dtaskId, string userId)
        {
            DTask dtask = await _context.DTasks.FirstOrDefaultAsync(t => t.Id == dtaskId);

            try
            {
                if(dtask != null)
                {
                    try
                    {
                        dtask.DeveloperUserId = userId;
                        dtask.DTaskStatusId = (await LookupDTaskStatusIdAsync("Development")).Value;
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<DTask>> GetAllDTasksByCompanyAsync(int companyId)
        {
            try
            {
                List<DTask> dtasks = await _context.Projects.Where(p => p.CompanyId == companyId)
                                                               .SelectMany(p => p.DTasks)
                                                               .Include(t => t.Attachments)
                                                               .Include(t => t.Comments)
                                                               .Include(t => t.DeveloperUser)
                                                               .Include(t => t.History)
                                                               .Include(t => t.OwnerUser)
                                                               .Include(t => t.DTaskPriority)
                                                               .Include(t => t.DTaskStatus)
                                                               .Include(t => t.DTaskType)
                                                               .Include(t => t.Project)
                                                               .ToListAsync();
                return dtasks;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task<List<DTask>> GetAllDTasksByPriorityAsync(int companyId, string priorityName)
        {
            int priorityId = (await LookupDTaskPriorityIdAsync(priorityName)).Value;
            try
            {
                List<DTask> dtasks = await _context.Projects.Where(p => p.CompanyId == companyId)
                                                                .SelectMany(p => p.DTasks)
                                                                .Include(t => t.Attachments)
                                                               .Include(t => t.Comments)
                                                               .Include(t => t.DeveloperUser)
                                                               .Include(t => t.History)
                                                               .Include(t => t.OwnerUser)
                                                               .Include(t => t.DTaskPriority)
                                                               .Include(t => t.DTaskStatus)
                                                               .Include(t => t.DTaskType)
                                                               .Include(t => t.Project)
                                                               .Where(t => t.DTaskPriorityId == priorityId)
                                                                .ToListAsync();
                return dtasks;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task<List<DTask>> GetAllDTasksByStatusAsync(int companyId, string statusName)
        {
            int statusId = (await LookupDTaskStatusIdAsync(statusName)).Value;
            try
            {
                List<DTask> dtasks = await _context.Projects.Where(p => p.CompanyId == companyId)
                                                                .SelectMany(p => p.DTasks)
                                                                .Include(t => t.Attachments)
                                                               .Include(t => t.Comments)
                                                               .Include(t => t.DeveloperUser)
                                                               .Include(t => t.OwnerUser)
                                                               .Include(t => t.DTaskPriority)
                                                               .Include(t => t.DTaskStatus)
                                                               .Include(t => t.DTaskType)
                                                               .Include(t => t.Project)
                                                               .Where(t => t.DTaskStatusId == statusId)
                                                                .ToListAsync();
                return dtasks;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<DTask>> GetAllDTasksByTypeAsync(int companyId, string typeName)
        {
            int typeId = (await LookupDTaskTypeIdAsync(typeName)).Value;
            try
            {
                List<DTask> dtasks = await _context.Projects.Where(p => p.CompanyId == companyId)
                                                                .SelectMany(p => p.DTasks)
                                                                .Include(t => t.Attachments)
                                                               .Include(t => t.Comments)
                                                               .Include(t => t.DeveloperUser)
                                                               .Include(t => t.OwnerUser)
                                                               .Include(t => t.DTaskPriority)
                                                               .Include(t => t.DTaskStatus)
                                                               .Include(t => t.DTaskType)
                                                               .Include(t => t.Project)
                                                               .Where(t => t.DTaskTypeId == typeId)
                                                                .ToListAsync();
                return dtasks;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<DTask>> GetArchivedDTasksAsync(int companyId)
        {
            try
            {
                List<DTask> dtasks = (await GetAllDTasksByCompanyAsync(companyId)).Where(t => t.Archived == true).ToList();
                return dtasks;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<DTask> GetDTaskByIdAsync(int dtaskId)
        {
            try
            {

                return await _context.DTasks.Include(t => t.DeveloperUser)
                                            .Include(t => t.OwnerUser)
                                            .Include(t => t.Project)
                                            .Include(t => t.DTaskPriority)
                                            .Include(t => t.DTaskStatus)
                                            .Include(t => t.DTaskType)
                                            .Include(t=>t.Comments)
                                            .Include(t=>t.Attachments)
                                            .Include(t=>t.History)
                                            .FirstOrDefaultAsync(m => m.Id == dtaskId);
            }
            catch (Exception ex)
            {
                throw;
            }

        }


        public async Task<List<DTask>> GetDTasksByRoleAsync(string role, string userId, int companyId)
        {
            List<DTask> dtasks = new();

            try
            {
                if (role == Roles.Admin.ToString())
                {
                    dtasks = await GetAllDTasksByCompanyAsync(companyId);
                }
                else if(role ==Roles.Developer.ToString())
                {
                    dtasks = (await GetAllDTasksByCompanyAsync(companyId)).Where(t => t.DeveloperUserId == userId).ToList();
                }
                else if (role == Roles.Submitter.ToString())
                {
                    dtasks = (await GetAllDTasksByCompanyAsync(companyId)).Where(t => t.DeveloperUserId == userId).ToList();
                }
                else if (role == Roles.ProjectManager.ToString())
                {
                    dtasks = await GetDTasksByUserIdAsync(userId, companyId);
                }

                return dtasks;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<DTask>> GetDTasksByUserIdAsync(string userId, int companyId)
        {
            DSUser appUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            List<DTask> dtasks = new();

            try
            {
                if(await _rolesService.IsUserInRoleAsync(appUser, Roles.Admin.ToString()))
                {
                    dtasks = (await _projectService.GetAllProjectsByCompanyAsync(companyId)).SelectMany(p => p.DTasks).ToList();
                }
                else if(await _rolesService.IsUserInRoleAsync(appUser, Roles.Developer.ToString()))
                {
                    dtasks = (await _projectService.GetAllProjectsByCompanyAsync(companyId))
                                                    .SelectMany(p => p.DTasks).Where(t => t.DeveloperUserId == userId).ToList();
                }
                else if (await _rolesService.IsUserInRoleAsync(appUser, Roles.Submitter.ToString()))
                {
                    dtasks = (await _projectService.GetAllProjectsByCompanyAsync(companyId))
                                                    .SelectMany(t => t.DTasks).Where(t => t.OwnerUserId == userId).ToList();
                }
                else if(await _rolesService.IsUserInRoleAsync(appUser, Roles.ProjectManager.ToString()))
                {
                    dtasks = (await _projectService.GetUserProjectsAsync(userId)).SelectMany(t => t.DTasks).ToList();
                }

                return dtasks;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<DTaskAttachment> GetDTaskAttachmentByIdAsync(int dtaskAttachmentId)
        {
            try
            {
                DTaskAttachment dtaskAttachment = await _context.DTaskAttachments
                                                                  .Include(t => t.User)
                                                                  .FirstOrDefaultAsync(t => t.Id == dtaskAttachmentId);
                return dtaskAttachment;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<List<DTask>> GetUnassignedDTasksAsync(int companyId)
        {
            List<DTask> dtasks = new();


            try
            {
                dtasks = (await GetAllDTasksByCompanyAsync(companyId)).Where(t=> string.IsNullOrEmpty(t.DeveloperUserId)).ToList();
;                return dtasks;

            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<int?> LookupDTaskPriorityIdAsync(string priorityName)
        {
            try
            {
                DTaskPriority priority = await _context.DTaskPriorities.FirstOrDefaultAsync(p => p.Name == priorityName);
                return priority?.Id;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task<int?> LookupDTaskStatusIdAsync(string statusName)
        {
            try
            {
                DTaskStatus status = await _context.DTaskStatuses.FirstOrDefaultAsync(p => p.Name == statusName);
                return status?.Id;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task<int?> LookupDTaskTypeIdAsync(string typeName)
        {
            try
            {
                DTaskType type = await _context.DTaskTypes.FirstOrDefaultAsync(p => p.Name == typeName);
                return type?.Id;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task UpdateDTaskAsync(DTask dtask)
        {
            try
            {

                _context.Update(dtask);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<DTask> GetDTasksAsNoTrackingAsync(int dtaskId)
        {
            try
            {
                DTask dtask = await _context.DTasks
                                              .Include(t => t.DTaskPriority)
                                              .Include(t => t.DTaskStatus)
                                              .Include(t => t.DTaskType)
                                              .Include(t => t.Project)
                                              .Include(t => t.DeveloperUser)
                                              .AsNoTracking().FirstOrDefaultAsync(t => t.Id == dtaskId);
                return dtask;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
