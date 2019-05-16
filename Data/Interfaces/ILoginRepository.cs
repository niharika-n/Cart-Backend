using Common.CommonData;
using Entity.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface ILoginRepository
    {
        /// <summary>
        /// Authenticates the user.
        /// </summary>
        /// <param name="loginModel">username and password of user.</param>
        /// <returns>
        /// UserModel for correct details.
        /// </returns>
        UserModel GetUser(string userName, string password);

        /// <summary>
        /// Finds the user.
        /// </summary>
        /// <param name="userName">userName of the user.</param>
        /// <returns>
        /// Details of the user.
        /// </returns>
        UserModel FindUser(string userName);        

        /// <summary>
        /// Sends email for changing password.
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="templateName">templateName</param>
        /// <returns>
        /// Url for the password change page.
        /// </returns>
        Task<string> SendPasswordEmail(string userName, string templateName);

        /// <summary>
        /// Validate the token.
        /// </summary>
        /// <param name="token">token</param>
        /// <returns>
        /// Details of the valid token.
        /// </returns>
        PasswordResetModel ValidateToken(string token);

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="newPassword">newPassword</param>
        /// <returns></returns>
        Task<bool> ChangePassword(string token, string newPassword);
    }
}
