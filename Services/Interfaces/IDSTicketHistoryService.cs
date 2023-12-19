using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dsana.Models;

namespace Dsana.Services.Interfaces
{
    public interface IDSDTaskHistoryService
    {
        Task AddHistoryAsync(DTask oldDTask, DTask newDTask, string userId);

        Task AddHistoryAsync(int dtaskId, string model, string userId);

        Task<List<DTaskHistory>> GetProjectDTaskHistoriesAsync(int projectId, int companyId);

    }
}
