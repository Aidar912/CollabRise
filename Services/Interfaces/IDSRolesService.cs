using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Dsana.Models;

namespace Dsana.Services.Interfaces
{
    public interface IDSRolesService
    {
        public Task<bool> IsUserInRoleAsync(DSUser user, string roleName);

        public Task<List<IdentityRole>> GetRolesAsync();

        public Task<IEnumerable<String>> GetUserRolesAsync(DSUser user);

        public Task<bool> AddUserToRoleAsync(DSUser role, string roleName);

        public Task<bool> RemoveUserFromRolesAsync(DSUser user, IEnumerable<string> roles);

        public Task<List<DSUser>> GetUsersInRoleAsync(string roleName, int companyId);
    }
}
