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
    public class DSRolesService : IDSRolesService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<DSUser> _userManager;


        public DSRolesService(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<DSUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }


        public async Task<bool> AddUserToRoleAsync(DSUser role, string roleName)
        {
            bool result = (await _userManager.AddToRoleAsync(role, roleName)).Succeeded;
            return result;
        }

        public async Task<List<IdentityRole>> GetRolesAsync()
        {
            try
            {
                List<IdentityRole> result = new();

                result = await _context.Roles.ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        public async Task<IEnumerable<string>> GetUserRolesAsync(DSUser user)
        {
            IEnumerable<string> result = await _userManager.GetRolesAsync(user);
            return result; 
        }

        public async Task<List<DSUser>> GetUsersInRoleAsync(string roleName, int companyId)
        {
            List<DSUser> users = (await _userManager.GetUsersInRoleAsync(roleName)).ToList();
            List<DSUser> result = users.Where(u => u.CompanyID == companyId).ToList();
            return result; 

        }


        public async Task<bool> IsUserInRoleAsync(DSUser user, string roleName)
        {
            bool result = await _userManager.IsInRoleAsync(user, roleName);
            return result;
        }

        public async Task<bool> RemoveUserFromRolesAsync(DSUser user, IEnumerable<string> roles)
        {
            bool result = (await _userManager.RemoveFromRolesAsync(user, roles)).Succeeded;
            return result;
        }
    }
}
