using Common.CommonData;
using Common.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Model;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Security.Principal;
using Microsoft.EntityFrameworkCore;
using Common.Extentions;
using Entity.Model;
using Data.Interfaces;

namespace Service.Logic
{
    public class UserService : IUserService
    {
        private IConfiguration _config;
        private readonly IUserRepository _userRepository;
        private readonly IHostingEnvironment _environment;
        private readonly ILoginService _loginService;
        private EmailExtention _emailExtention;
        private SpecificClaim _specificClaim;

        public UserService(IConfiguration config, IUserRepository userRepository, IHostingEnvironment environment,
            ILoginService loginService, IPrincipal _principal)
        {
            _config = config;
            _environment = environment;
            _emailExtention = new EmailExtention(config);
            _userRepository = userRepository;
            _loginService = loginService;
            _specificClaim = new SpecificClaim(_principal);
        }

        public async Task<IResult> CreateUser(UserViewModel viewModel, IFormFile img, bool fromAdmin)
        {
            var result = new Result
            {
                Operation = Operation.Create,
                Status = Status.Success
            };
            try
            {
                if (viewModel == null)
                {
                    result.Message = "noUser";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
                if (img == null)
                {
                    result.Message = "noUserImage";
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    return result;
                }
                var user = new UserModel();
                ImageExtention imageExtention = new ImageExtention();
                user.MapFromViewModel(viewModel);
                user.ImageContent = imageExtention.Image(img);
                if(user.Password == null)
                {
                    user.Password = Guid.NewGuid().ToString().Replace("-", ""); 
                }
                var userRoles = new List<UserRolesModel>();
                foreach (var role in viewModel.Roles)
                {
                    UserRolesModel rolesModel = new UserRolesModel();
                    rolesModel.RoleId = role;
                    userRoles.Add(rolesModel);
                }
                user.Roles = userRoles;
                var newUser = await _userRepository.CreateUser(user);
                if (newUser.Status == Status.Success)
                {
                    if (fromAdmin)
                    {
                        var mailLink = await _loginService.MailPasswordLink(newUser.Body);
                        return mailLink;
                    }
                    result.Status = Status.Success;
                    result.StatusCode = HttpStatusCode.OK;
                    result.Body = "success";
                    return result;
                }
                return newUser;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }

        public async Task<IResult> GetDetail(int id)
        {
            var result = new Result
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                if (id != 0)
                {
                    var userDetail = await _userRepository.GetDetail(id);
                    if (userDetail != null)
                    {
                        UserViewModel userView = new UserViewModel();
                        userView.MapFromModel(userDetail);
                        userView.Roles = userDetail.Roles.Select(r => r.RoleId).ToArray();
                        result.Status = Status.Success;
                        result.StatusCode = HttpStatusCode.OK;
                        result.Body = userView;
                        return result;
                    }
                    else
                    {
                        result.Status = Status.Fail;
                        result.StatusCode = HttpStatusCode.BadRequest;
                        result.Message = "User does not exist.";
                        return result;
                    }
                }
                result.Status = Status.Fail;
                result.StatusCode = HttpStatusCode.BadRequest;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;

                return result;
            }
        }

        public async Task<IResult> Update(UserViewModel viewModel, IFormFile img)
        {
            var result = new Result()
            {
                Operation = Operation.Update,
                Status = Status.Success
            };
            try
            {
                UserModel userModel = new UserModel();
                userModel.MapFromViewModel(viewModel);
                if (img != null)
                {
                    ImageExtention imageExtention = new ImageExtention();
                    userModel.ImageContent = imageExtention.Image(img);
                }
                var updateUser = await _userRepository.Update(userModel);
                if (updateUser.Status != Status.Success)
                {
                    return updateUser;
                }
                userModel = updateUser.Body;
                UserViewModel userView = new UserViewModel();
                userView.MapFromModel(userModel);
                result.Body = userView;
                result.Status = Status.Success;
                result.StatusCode = HttpStatusCode.OK;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }

        public async Task<IResult> ChangePassword(string oldPassword, string newPassword)
        {
            var result = new Result
            {
                Operation = Operation.Update,
                Status = Status.Success
            };
            try
            {
                var userId = _specificClaim.GetSpecificClaim("Id");
                var PasswordCheck = await _userRepository.CheckOldPassword(userId, oldPassword);
                if (PasswordCheck.Status != Status.Success)
                {
                    return PasswordCheck;
                }
                UserModel userModel = new UserModel();
                userModel = PasswordCheck.Body;
                if (userId == userModel.UserId)
                {
                    if (userModel.Password != oldPassword)
                    {
                        result.Status = Status.Fail;
                        result.StatusCode = HttpStatusCode.BadRequest;
                        result.Message = "Incorrect password entered.";
                        return result;
                    }
                    else
                    {
                        var changePassword = await _userRepository.ChangePassword(userModel.UserId, newPassword);
                        if (changePassword.Status == Status.Success)
                        {
                            result.Status = Status.Success;
                            result.StatusCode = HttpStatusCode.OK;
                            result.Message = "Password changed successfully.";
                            return result;
                        }
                        else
                        {
                            return changePassword;
                        }
                    }
                }
                result.Status = Status.Success;
                result.StatusCode = HttpStatusCode.OK;
                result.Message = "Incorrect Password entered.";
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;

                return result;
            }
        }

        public IResult GetUserList(DataHelperModel dataHelper)
        {
            var result = new Result()
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                int userId = _specificClaim.GetSpecificClaim("Id");
                var userList = _userRepository.GetUserList(userId, dataHelper.Search);
                if (userList.Count() == 0)
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "noUserPresent";
                    return result;
                }
                var list = userList;
                list = DataSortExtention.SortBy(list, dataHelper.SortColumn, dataHelper.SortOrder);
                var resultCount = list.Count();
                var pagedList = DataCount.Page(list, dataHelper.PageNumber, dataHelper.PageSize);
                var resultList = pagedList.ToList();
                var userViewModels = new List<UserViewModel>();
                userViewModels = resultList.Select(r =>
                {
                    var userViewModel = new UserViewModel();
                    userViewModel.MapFromModel(r);
                    userViewModel.Roles = r.Roles.Select(x => x.RoleId).ToArray();
                    return userViewModel;
                }).ToList();

                ResultModel resultModel = new ResultModel();
                resultModel.UserResult = userViewModels;
                resultModel.TotalCount = resultCount;

                result.Status = Status.Success;
                result.StatusCode = HttpStatusCode.OK;
                result.Body = resultModel;
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }

        public async Task<IResult> ChangeUserRole(int id, bool add, string selectedRoles)
        {
            var result = new Result()
            {
                Operation = Operation.Update,
                Status = Status.Success
            };
            try
            {
                var userObj = await _userRepository.GetDetail(id);
                var userRoles = userObj.Roles.Select(r => r.RoleId).ToArray();
                string[] roles = selectedRoles.Split(',');
                int[] newRoles = Array.ConvertAll(roles, int.Parse);
                if (newRoles.Count() == 0)
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "noRoleSelected";
                    return result;
                }
                var assignRoles = await _userRepository.ChangeUserRole(id, add, userRoles, newRoles);
                if (assignRoles)
                {
                    result.StatusCode = HttpStatusCode.OK;
                    result.Message = "newRolesAlloted";
                    return result;
                }
                result.Status = Status.Fail;
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Message = "rolesNotAlloted";
                return result;
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;

                return result;
            }
        }
    }
}
