using Common.CommonData;
using Entity.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Model;

namespace Service.Interfaces
{
    public interface ILoginService
    {
        /// <summary>
        /// Authenticates the user.
        /// </summary>
        /// <param name="loginModel">username and password of user.</param>
        /// <returns>
        /// ViewModel of the user.
        /// </returns>
        IResult LoginUser(LoginModel loginModel);

        /// <summary>
        /// Forgot password to reset new password.
        /// </summary>
        /// <param name="userName">Username/Email Address of user.</param>
        /// <returns>
        /// Status with message for email status.
        /// </returns>
        Task<IResult> MailPasswordLink(string userName);

        /// <summary>
        /// Vadilates reset token.
        /// </summary>
        /// <param name="token">Token for reset password.</param>
        /// <returns>
        /// Returns status for email message sent.
        /// </returns>
        IResult ValidateToken(string token);

        /// <summary>
        /// Change user paswword.
        /// </summary>
        /// <param name="userToken">usertoken for verification of url</param>
        /// <param name="newPassword">new password for user.</param>
        /// <returns>
        /// Status with message string.
        /// </returns>
        Task<IResult> ChangePassword(string userToken, string newPassword);
    }
}
