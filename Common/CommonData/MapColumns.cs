using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Common.CommonData
{
    public static class Helper
    {
        public static UserClaim GetUserClaimDetails(ClaimsIdentity identity)
        {
            UserClaim userClaim = null;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                userClaim = new UserClaim
                {
                    UserName = identity.FindFirst(ClaimTypes.Name).Value,
                    EmailId = identity.FindFirst(ClaimTypes.Email).Value,
                    Roles = identity.FindFirst("Roles").Value,
                    UserId = identity.FindFirst(ClaimTypes.NameIdentifier).Value
                };
            }
            return userClaim;
        }
        public static void MapAuditColumns(this object model, ClaimsIdentity identity)
        {
            if (identity != null)
            {
                var authorizedInfo = GetUserClaimDetails(identity);
                if (model != null && authorizedInfo != null)
                {
                    if (Convert.ToDateTime(GetColumnValue(ConstantStrings.CreatedDate, model)) == default(DateTime))
                    {
                        SetColumnValue(ConstantStrings.IsActiveColumn, model, true);
                        SetColumnValue(ConstantStrings.CreatedDate, model, DateTime.Now);
                        SetColumnValue(ConstantStrings.CreatedBy, model, authorizedInfo.UserId);
                    }
                    SetColumnValue(ConstantStrings.ModifiedDate, model, DateTime.Now);
                    SetColumnValue(ConstantStrings.ModifiedBy, model, authorizedInfo.UserId);
                }
            }
        }

        public static object GetColumnValue(string columnName, object entity)
        {
            var pinfo = entity.GetType().GetProperty(columnName);
            if (pinfo == null) { return null; }
            return pinfo.GetValue(entity, null);
        }
       
        public static void SetColumnValue(string columnName, object entity, object value)
        {
            var pinfo = entity.GetType().GetProperty(columnName);
            if (pinfo != null) { pinfo.SetValue(entity, value, null); }
        }
    }
}
