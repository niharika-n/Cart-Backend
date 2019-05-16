using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace Common.CommonData
{
    public class SpecificClaim
    {
        private readonly ClaimsPrincipal principal;

        public SpecificClaim(IPrincipal _principal)
        {
            principal = _principal as ClaimsPrincipal;
        }

        public dynamic GetSpecificClaim(string type)
        {
            var claimsIdentity = (ClaimsIdentity)principal.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userName = claimsIdentity.FindFirst(ClaimTypes.Name).Value;
            var userEmail = claimsIdentity.FindFirst(ClaimTypes.Email).Value;
            var roleId = claimsIdentity.FindFirst("Roles").Value;
            if (type == "Id")
            {
                return Convert.ToInt32(userId);
            }
            else if (type == "Email")
            {
                return Convert.ToString(userEmail);
            }
            else if (type == "RoleId")
            {
                return roleId;
            }
            else
            {
                return Convert.ToString(userName);
            }
        }
        
    }
}
