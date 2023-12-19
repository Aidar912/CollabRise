using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dsana.Models;

namespace Dsana.Services.Interfaces
{
    public interface IDSLookupService
    {
        public Task<List<DTaskPriority>> GetDTaskPrioritiesAsync();

        public Task<List<DTaskStatus>> GetDTaskStatusesAsync();

        public Task<List<DTaskType>> GetDTaskTypesAsync();

        public Task<List<ProjectPriority>> GetProjectPrioritiesAsync();
    }
}
