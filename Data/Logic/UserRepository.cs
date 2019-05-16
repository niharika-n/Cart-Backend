using Data.Interfaces;
using Entity.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Common.CommonData;
using Common.Enums;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Common.Extentions;

namespace Data.Logic
{
    public class UserRepository : Repository<UserModel>, IUserRepository
    {
        private readonly WebApisContext _context;
        private IConfiguration _config;
        private readonly ILoginRepository _loginRepository;

        public UserRepository(WebApisContext APIcontext, ILoginRepository loginRepository, IConfiguration config) : base(APIcontext)
        {
            _context = APIcontext;
            _config = config;
            _loginRepository = loginRepository;
        }

        public async Task<string> CheckUserDetail(UserModel user)
        {
            string message = null;
            var userCheckQuery = await _context.Users.Where(u => (u.UserName == user.UserName) || (u.EmailId == user.EmailId)).FirstOrDefaultAsync();
            if (userCheckQuery != null)
            {
                if (userCheckQuery.UserName == user.UserName)
                {
                    message = "usernameExists";
                    return message;
                }
                else if (userCheckQuery.EmailId == user.EmailId)
                {
                    message = "emailExists";
                    return message;
                }
            }
            return message;
        }

        public async Task<IResult> CreateUser(UserModel userModel)
        {
            var result = new Result()
            {
                Status = Status.Success
            };
            var duplicateUser = await CheckUserDetail(userModel);
            try
            {
                if (duplicateUser != null)
                {
                    result.Message = duplicateUser;
                    result.Status = Status.Fail;
                    return result;
                }
                _context.Users.Add(userModel);
                await _context.SaveChangesAsync();
                var createdUser = await _context.Users.Where(u => u.EmailId == userModel.EmailId).FirstOrDefaultAsync();
                result.Body = createdUser.UserName;
                return result;
            }
            catch (Exception e)
            {
                result.Message = e.Message;
                result.Status = Status.Error;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }

        public async Task<UserModel> GetDetail(int id)
        {
            var userDetail = from user in _context.Users
                             where user.UserId == id
                             from roles in user.Roles
                             select new UserModel
                             {
                                 Roles = user.Roles,
                                 UserId = user.UserId,
                                 EmailId = user.EmailId,
                                 FirstName = user.FirstName,
                                 LastName = user.LastName,
                                 ImageContent = user.ImageContent,
                                 Password = "",
                                 UserName = user.UserName
                             };
            var userObj = await userDetail.FirstOrDefaultAsync();
            return userObj;
        }

        public async Task<IResult> Update(UserModel userModel)
        {
            var result = new Result()
            {
                Status = Status.Success
            };
            var user = await _context.Users.Where(u => u.UserId == userModel.UserId).FirstOrDefaultAsync();
            var existingUser = await _context.Users.Where(u => ((u.UserName == user.UserName) || (u.EmailId == user.EmailId)) && (u.UserId != user.UserId)).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                if (existingUser.UserName == user.UserName)
                {
                    result.Status = Status.Fail;
                    result.Message = "usernameMessage";
                    return result;
                }
                else if (existingUser.EmailId == user.EmailId)
                {
                    result.Status = Status.Fail;
                    result.Message = "emailMessage";
                    return result;
                }
            }
            user.UserName = userModel.UserName;
            user.FirstName = userModel.FirstName;
            user.LastName = userModel.LastName;
            user.EmailId = userModel.EmailId;
            if (userModel.ImageContent != null)
            {
                user.ImageContent = userModel.ImageContent;
            }
            await _context.SaveChangesAsync();
            var updatedDetails = await GetDetail(userModel.UserId);
            result.Body = updatedDetails;
            return result;
        }

        public async Task<IResult> CheckOldPassword(int id, string oldPassword)
        {
            var result = new Result()
            {
                Status = Status.Success
            };
            var password = await _context.Users.Where(u => u.Password == oldPassword && u.UserId == id).FirstOrDefaultAsync();
            if (password == null)
            {
                result.Message = "User details incorrect";
                result.Status = Status.Fail;
                result.StatusCode = HttpStatusCode.BadRequest;
                return result;
            }
            result.Body = password;
            return result;
        }

        public async Task<IResult> ChangePassword(int id, string newPassword)
        {
            var result = new Result()
            {
                Status = Status.Success
            };
            try
            {
                var user = await _context.Users.Where(u => u.UserId == id).FirstOrDefaultAsync();
                user.Password = newPassword;
                await _context.SaveChangesAsync();

                result.Status = Status.Success;
                result.StatusCode = HttpStatusCode.OK;
                return result;
            }
            catch(Exception e)
            {
                result.Body = e;
                result.Status = Status.Error;
                result.StatusCode = HttpStatusCode.BadRequest;
                return result;
            }
        }

        public IQueryable<UserModel> GetUserList(int id, string search)
        {
            var userObj = from user in _context.Users
                          join role in _context.UserRoles
                          on user.UserId equals role.UserId
                          into assignedRoles
                          from userRole in assignedRoles.DefaultIfEmpty()
                          group new { user } by
                          new { user, assignedRoles } into userDetail
                          select new UserModel
                          {
                              UserId = userDetail.Key.user.UserId,
                              EmailId = userDetail.Key.user.EmailId,
                              FirstName = userDetail.Key.user.FirstName,
                              LastName = userDetail.Key.user.LastName,
                              UserName = userDetail.Key.user.UserName,
                              Password = null,
                              Roles = userDetail.Key.assignedRoles.Where(x => x.UserId == userDetail.Key.user.UserId).ToArray(),
                              ImageContent = null
                          };
            if (search != null)
            {
                userObj = userObj.Where(x => x.UserName.Contains(search) || x.EmailId.Contains(search));
            }
            var userList = userObj.Where(u => !u.UserId.Equals(id)).Select(u => u);

            return userList;
        }

        public async Task<bool> ChangeUserRole(int id, bool add, int[] currentRoles, int[] roles)
        {
            try
            {
                foreach (var role in roles)
                {
                    if (!currentRoles.Contains(role) && add)
                    {
                        UserRolesModel userRoles = new UserRolesModel();
                        userRoles.RoleId = role;
                        userRoles.UserId = id;
                        _context.UserRoles.Add(userRoles);
                        await _context.SaveChangesAsync();
                    }
                    else if (currentRoles.Contains(role) && !add)
                    {
                        var unrequiredRole = await _context.UserRoles.Where(x => x.UserId == id && x.RoleId == role).FirstOrDefaultAsync();
                        _context.UserRoles.Remove(unrequiredRole);
                        await _context.SaveChangesAsync();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}