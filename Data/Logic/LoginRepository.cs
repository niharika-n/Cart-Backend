using Common.CommonData;
using Common.Enums;
using Data.Interfaces;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Data.Logic
{
    public class LoginRepository : Repository<UserModel>, ILoginRepository
    {
        private readonly WebApisContext _context;
        private IConfiguration _config;

        public LoginRepository(WebApisContext APIcontext, IConfiguration config) : base(APIcontext)
        {
            _context = APIcontext;
            _config = config;
        }

        public UserModel GetUser(string userName, string password)
        {
            var user = from loginUser in _context.Users
                       where (loginUser.UserName == userName
                       && loginUser.Password == password) || (loginUser.EmailId == userName && loginUser.Password == password)
                       from roles in loginUser.Roles
                       select new UserModel
                       {
                           Roles = loginUser.Roles,
                           UserId = loginUser.UserId,
                           EmailId = loginUser.EmailId,
                           FirstName = loginUser.FirstName,
                           LastName = loginUser.LastName,
                           ImageContent = loginUser.ImageContent,
                           UserName = loginUser.UserName
                       };

            var userObj = user.FirstOrDefault();

            return userObj;
        }

        public UserModel FindUser(string userName)
        {
            var user = _context.Users.Where(u => (u.UserName == userName) || (u.EmailId == userName)).FirstOrDefault();

            return user;
        }        

        public async Task<string> SendPasswordEmail(string userName, string templateName)
        {
            var userObj = FindUser(userName);
            string num = Guid.NewGuid().ToString().Replace("-", "");
            PasswordResetModel resetModel = new PasswordResetModel
            {
                Email = userObj.EmailId,
                OldPassword = userObj.Password,
                Token = num,
                TokenTimeOut = DateTime.Now.AddHours(2),
                UserId = userObj.UserId
            };

            _context.PasswordReset.Add(resetModel);
            await _context.SaveChangesAsync();

            var template = _context.Content.Where(x => x.TemplateName == templateName).FirstOrDefault();

            var url = _config["DefaultCorsPolicyName"] + "reset_password/" + num;
            return url;
        }

        public PasswordResetModel ValidateToken(string token)
        {
            var tokenObj = _context.PasswordReset.Where(p => p.Token == token && p.PasswordChanged != true && p.TokenTimeOut > DateTime.Now).SingleOrDefault();
            return tokenObj;
        }

        public async Task<bool> ChangePassword(string token, string newPassword)
        {
            var tokenDetail = ValidateToken(token);
            var user = _context.Users.Where(x => x.UserId == tokenDetail.UserId).SingleOrDefault();
            user.Password = newPassword;
            await _context.SaveChangesAsync();
            tokenDetail.PasswordChanged = true;
            tokenDetail.ResetDate = DateTime.Now;
            await _context.SaveChangesAsync();
            if (tokenDetail == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

