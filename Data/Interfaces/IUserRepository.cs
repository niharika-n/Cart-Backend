using Common.CommonData;
using Common.Enums;
using Entity.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IUserRepository
    {
        Task<IResult> CreateUser(UserModel user);

        Task<string> CheckUserDetail(UserModel user);

        Task<UserModel> GetDetail(int id);

        Task<IResult> Update(UserModel user);

        Task<IResult> CheckOldPassword(int id, string oldPassword);

        Task<IResult> ChangePassword(int id, string newPassword);

        IQueryable<UserModel> GetUserList(int id, string search);

        Task<bool> ChangeUserRole(int id, bool add, int[] currentRoles, int[] roles);
    }
}
