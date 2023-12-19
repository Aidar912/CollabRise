using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Principal;

namespace Dsana.Extensions
{
    public static class IdentityExtensions
    {
        public static int? GetCompanyId(this IIdentity identity)
        {
            Claim claim = ((ClaimsIdentity)identity).FindFirst("CompanyID");
            return (claim != null) ? int.Parse(claim.Value) : null;

        }
    }
}
