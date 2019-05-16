using Common.Enums;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Common.Roles
{
    public class RoleRequirement : IAuthorizationRequirement
    {
        public RoleRequirement(params UserRoles[] role)
        {
            roleId = role;
        }

        public UserRoles[] roleId { get; set; }
    }

}
