using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dsana.Models;

namespace Dsana.Services.Interfaces
{
    public interface IDSCompanyInfoService
    {
        public Task<Company> GetCompanyInfoByIdAsync(int? companyId);

        public Task<List<DSUser>> GetAllMembersAsync(int companyId);

        public Task<List<Project>> GetAllProjectsAsync(int companyId);

        //public Task<List<DTask>> GetAllDTasksAsync(int companyId);


    }
}
