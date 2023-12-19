using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Dsana.Data;
using Dsana.Models;
using Dsana.Services.Interfaces;

namespace Dsana.Services
{
    public class DSDTaskHistory : IDSDTaskHistoryService
    {
        private readonly ApplicationDbContext _context;

        public DSDTaskHistory(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task AddHistoryAsync(DTask oldDTask, DTask newDTask, string userId)
        {
            //NEW TICKET HAS BEEN ADDED
            if(oldDTask == null && newDTask != null)
            {
                DTaskHistory history = new()
                {
                    DTaskId = newDTask.Id,
                    Property = "",
                    OldValue = "",
                    NewValue = "",
                    Created = DateTimeOffset.Now,
                    UserId = userId,
                    Description = "New Task Created"
                };

                try
                {
                    await _context.DTaskHistories.AddAsync(history);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            else
            {
                if(oldDTask.Title != newDTask.Title)
                {
                    DTaskHistory history = new()
                    {
                        DTaskId = newDTask.Id,
                        Property = "Title",
                        OldValue = oldDTask.Title,
                        NewValue = newDTask.Title,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New task title: {newDTask.Title}"
                    };
                    await _context.DTaskHistories.AddAsync(history);
                }

                if(oldDTask.Description != newDTask.Description)
                {
                    DTaskHistory history = new()
                    {
                        DTaskId = newDTask.Id,
                        Property = "Description",
                        OldValue = oldDTask.Description,
                        NewValue = newDTask.Description,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New task description: {newDTask.Description}"
                    };
                    await _context.DTaskHistories.AddAsync(history);
                }

                if (oldDTask.DTaskPriorityId != newDTask.DTaskPriorityId)
                {
                    DTaskHistory history = new()
                    {
                        DTaskId = newDTask.Id,
                        Property = "DTaskPriority",
                        OldValue = oldDTask.DTaskPriority.Name,
                        NewValue = newDTask.DTaskPriority.Name,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New task priority: {newDTask.DTaskPriority.Name}"
                    };
                    await _context.DTaskHistories.AddAsync(history);
                }

                if (oldDTask.DTaskStatusId != newDTask.DTaskStatusId)
                {
                    DTaskHistory history = new()
                    {
                        DTaskId = newDTask.Id,
                        Property = "DTaskStatus",
                        OldValue = oldDTask.DTaskStatus.Name,
                        NewValue = newDTask.DTaskStatus.Name,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New task status: {newDTask.DTaskStatus.Name}"
                    };
                    await _context.DTaskHistories.AddAsync(history);
                }

                if (oldDTask.DTaskTypeId != newDTask.DTaskTypeId)
                {
                    DTaskHistory history = new()
                    {
                        DTaskId = newDTask.Id,
                        Property = "DTaskTypeId",
                        OldValue = oldDTask.DTaskType.Name,
                        NewValue = newDTask.DTaskType.Name,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New task type: {newDTask.DTaskType.Name}"
                    };
                    await _context.DTaskHistories.AddAsync(history);
                }

                if (oldDTask.DeveloperUserId != newDTask.DeveloperUserId)
                {
                    DTaskHistory history = new()
                    {
                        DTaskId = newDTask.Id,
                        Property = "Developer",
                        OldValue = oldDTask.DeveloperUser?.FullName ?? "Not Assigned",
                        NewValue = newDTask.DeveloperUser?.FullName,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New task developer: {newDTask.DeveloperUser.FullName}"
                    };
                    await _context.DTaskHistories.AddAsync(history);
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task AddHistoryAsync(int dtaskId, string model, string userId)
        {
            try
            {
                DTask dtask = await _context.DTasks.FindAsync(dtaskId);
                string description = model.ToLower().Replace("task", "");
                description = $"New {description} added to Task: {dtask.Title}";

                DTaskHistory history = new()
                {
                    DTaskId = dtask.Id,
                    Property = model,
                    OldValue = "",
                    NewValue = "",
                    Created = DateTimeOffset.Now,
                    UserId = userId,
                    Description = description
                };
                await _context.DTaskHistories.AddAsync(history);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<List<DTaskHistory>> GetProjectDTaskHistoriesAsync(int projectId, int companyId)
        {
            try
            {
                Project project = await _context.Projects.Where(p => p.CompanyId == companyId)
                                                            .Include(p => p.DTasks)
                                                                .ThenInclude(t => t.History)
                                                                    .ThenInclude(h => h.User)
                                                            .FirstOrDefaultAsync(p => p.Id == projectId);

                List<DTaskHistory> dtaskHistory = project.DTasks.SelectMany(t => t.History).ToList();

                return dtaskHistory;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
