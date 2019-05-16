using Common.CommonData;
using Common.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Model;

namespace Service.Interfaces
{
    public interface IUserService
    {

        /// <summary>
        /// Creates new user.
        /// </summary>
        /// <returns>
        /// IResult with Status.
        /// </returns>   
        Task<IResult> CreateUser(UserViewModel user, IFormFile img, bool fromAdmin);

        /// <summary>
        /// User detail.
        /// </summary>
        /// <param name="id">Id of user</param>
        /// <returns>
        /// Returns user details.
        /// </returns>
        Task<IResult> GetDetail(int id);

        /// <summary>
        /// Update user details.
        /// </summary>
        /// <returns>
        /// Detail of user updated.
        /// </returns>
        Task<IResult> Update(UserViewModel user, IFormFile img);

        /// <summary>
        /// Change password.
        /// </summary>
        /// <param name="oldPassword">Old password of user.</param>
        /// <param name="newPassword">New password set.</param>
        /// <returns>
        /// Status of password changed.
        /// </returns>
        Task<IResult> ChangePassword(string oldPassword, string newPassword);

        /// <summary>
        /// user list.
        /// </summary>
        /// <returns>
        /// Returns list of user to superAdmin only.
        /// </returns>
        IResult GetUserList(DataHelperModel dataHelper);

        /// <summary>
        /// Assign user roles.
        /// </summary>
        /// <param name="id">id of user.</param>
        /// <param name="add">check for addition of new roles</param>
        /// <param name="selectedRoles">new roles.</param>
        /// <returns>
        /// Iresult with status.
        /// </returns>
        Task<IResult> ChangeUserRole(int id, bool add, string selectedRoles);
    }
}
