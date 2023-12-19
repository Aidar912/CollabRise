using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dsana.Models;

namespace Dsana.Services.Interfaces
{
    public interface IDSDTaskService
    {
        public Task AddNewDTaskAsync(DTask dtask);
        public Task UpdateDTaskAsync(DTask dtask);
        public Task<DTask> GetDTaskByIdAsync(int dtaskId);
        public Task AddDTaskCommentAsync(DTaskComment dtaskComment);
        public Task AddDTaskAttachmentAsync(DTaskAttachment dtaskAttachment);
        public Task AssignDTaskAsync(int dtaskId, string userId);
        public Task<List<DTask>> GetArchivedDTasksAsync(int companyId);
        public Task<List<DTask>> GetAllDTasksByCompanyAsync(int companyId);
        public Task<List<DTask>> GetAllDTasksByPriorityAsync(int companyId, string priorityName);
        public Task<List<DTask>> GetAllDTasksByStatusAsync(int companyId, string statusName);
        public Task<List<DTask>> GetAllDTasksByTypeAsync(int companyId, string typeName);
        public Task<DTask> GetDTasksAsNoTrackingAsync(int dtaskId);
        public Task<List<DTask>> GetDTasksByRoleAsync(string role, string userId, int companyId);
        public Task<List<DTask>> GetDTasksByUserIdAsync(string userId, int companyId);
        public Task<DTaskAttachment> GetDTaskAttachmentByIdAsync(int dtaskAttachmentId);
        public Task<List<DTask>> GetUnassignedDTasksAsync(int companyId);


        public Task<int?> LookupDTaskPriorityIdAsync(string priorityName);
        public Task<int?> LookupDTaskStatusIdAsync(string statusName);
        public Task<int?> LookupDTaskTypeIdAsync(string typeName);
    }
}
