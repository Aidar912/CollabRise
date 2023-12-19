using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Dsana.Data;
using Dsana.Models;
using Dsana.Services.Interfaces;

namespace Dsana.Services
{
    public class DSLookupService : IDSLookupService
    {
        private readonly ApplicationDbContext _context;

        public DSLookupService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectPriority>> GetProjectPrioritiesAsync()
        {
            try
            {
                return await _context.ProjectPriorities.ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<DTaskPriority>> GetDTaskPrioritiesAsync()
        {
            try
            {
                return await _context.DTaskPriorities.ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<DTaskStatus>> GetDTaskStatusesAsync()
        {
            try
            {
                return await _context.DTaskStatuses.ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<DTaskType>> GetDTaskTypesAsync()
        {
            try
            {
                return await _context.DTaskTypes.ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
