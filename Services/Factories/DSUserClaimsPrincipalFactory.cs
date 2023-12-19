using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Dsana.Models;

namespace Dsana.Services.Factories
{
    public class DSUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<DSUser, IdentityRole>
    {
        public DSUserClaimsPrincipalFactory(UserManager<DSUser> userManager,
                                            RoleManager<IdentityRole> roleManager,
                                            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager,roleManager,optionsAccessor)
        {

        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(DSUser user)
        {
            ClaimsIdentity identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("CompanyID", user.CompanyID.ToString()));

            return identity;
        }
    }
}
