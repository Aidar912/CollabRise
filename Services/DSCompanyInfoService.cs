using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Dsana.Data;
using Dsana.Models;
using Dsana.Services.Interfaces;

namespace Dsana.Services
{
    public class DSCompanyInfoService : IDSCompanyInfoService
    {

        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<DSUser> _userManager;


        public DSCompanyInfoService(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<DSUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            
        }


        public async Task<List<DSUser>> GetAllMembersAsync(int companyId)
        {
            List<DSUser> result = new();

            result = await _context.Users.Where(u => u.CompanyID == companyId).ToListAsync();

            return result;
        }

        public async Task<List<Project>> GetAllProjectsAsync(int companyId)
        {
            List<Project> result = new();

            result = await _context.Projects.Where(p => p.CompanyId == companyId)
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

            return result;
        }


        public async Task<Company> GetCompanyInfoByIdAsync(int? companyId)
        {
            Company result = new();

            if(companyId != null)
            {
                result = await _context.Companies
                    .Include(c => c.Members)
                    .Include(c => c.Projects)
                    .FirstOrDefaultAsync(c => c.Id == companyId);
            }
            return result;
        }
    }
}
