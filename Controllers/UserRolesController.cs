using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dsana.Extensions;
using Dsana.Models;
using Dsana.Models.ViewModels;
using Dsana.Services.Interfaces;

namespace Dsana.Controllers
{
    [Authorize]
    public class UserRolesController : Controller
    {
        private readonly IDSRolesService _rolesService;
        private readonly IDSCompanyInfoService _companyInfoService;

        
        public UserRolesController(IDSRolesService rolesService, IDSCompanyInfoService companyInfoService)
        {
            _rolesService = rolesService;
            _companyInfoService = companyInfoService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageUserRoles()
        {
            
            List <ManageUserRolesViewModel> model = new();
            
            int companyId = User.Identity.GetCompanyId().Value;

            List<DSUser> users = await _companyInfoService.GetAllMembersAsync(companyId);


            foreach (DSUser user in users)
            {
                ManageUserRolesViewModel viewModel = new();
                viewModel.DSUser = user;
                IEnumerable<string> selected = await _rolesService.GetUserRolesAsync(user);
                viewModel.Roles = new MultiSelectList(await _rolesService.GetRolesAsync(), "Name", "Name", selected);

                model.Add(viewModel);
            }


            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            DSUser appUser = (await _companyInfoService.GetAllMembersAsync(companyId)).FirstOrDefault(u => u.Id == member.DSUser.Id);

            IEnumerable<string> roles = await _rolesService.GetUserRolesAsync(appUser);

            string userRole = member.SelectedRoles.FirstOrDefault();

            if (!string.IsNullOrEmpty(userRole))
            {
                if(await _rolesService.RemoveUserFromRolesAsync(appUser, roles))
                {
                    await _rolesService.AddUserToRoleAsync(appUser, userRole);
                }
            }

            return RedirectToAction(nameof(ManageUserRoles));
        }
    }
}