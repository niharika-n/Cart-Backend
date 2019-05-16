using Common.CommonData;
using Common.Enums;
using Common.Extentions;
using Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Model;
using System.Linq;

namespace Service.Logic
{
    public class LoginService : ILoginService
    {
        private IConfiguration _config;
        private readonly ILoginRepository _loginRepository;
        private EmailExtention _emailExtention;
        private readonly ITemplateRepository _templateRepository;

        public LoginService(IConfiguration config, ILoginRepository loginRepository, ITemplateRepository templateRepository)
        {
            _config = config;
            _loginRepository = loginRepository;
            _templateRepository = templateRepository;
            _emailExtention = new EmailExtention(config);
        }

        public IResult LoginUser(LoginModel loginModel)
        {
            var result = new Result
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                if (loginModel.UserName != null && loginModel.Password != null)
                {
                    var userDetail = _loginRepository.GetUser(loginModel.UserName, loginModel.Password);
                    if (userDetail == null)
                    {
                        result.Status = Status.Fail;
                        result.StatusCode = HttpStatusCode.BadRequest;
                        result.Message = "Username or Password is incorrect";
                        return result;
                    }
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
                    result.Message = "Enter username and password";
                    return result;
                }
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;

                return result;
            }
        }

        public async Task<IResult> MailPasswordLink(string userName)
        {
            var result = new Result
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                if (userName != null)
                {
                    var userDetail = _loginRepository.FindUser(userName);
                    if (userDetail == null)
                    {
                        result.Status = Status.Fail;
                        result.StatusCode = HttpStatusCode.BadRequest;
                        result.Message = "Username/EmailId is incorrect";
                        return result;
                    }
                    UserViewModel userView = new UserViewModel();
                    userView.MapFromModel(userDetail);

                    var emailContent = _templateRepository.GetTemplate(2);
                    if (emailContent == null)
                    {
                        result.Status = Status.Fail;
                        result.StatusCode = HttpStatusCode.BadRequest;
                        result.Message = "Email Template not found.";
                        return result;
                    }

                    var emailUrl = await _loginRepository.SendPasswordEmail(userDetail.UserName, "change_password");
                    EmailViewModel emailView = new EmailViewModel();
                    emailView.Subject = "Reset your password";
                    emailView.Content = emailContent.Content.Replace("{ResetUrl}", emailUrl).Replace("{UserName}", userView.UserName);
                    emailView.ToEmailList.Add(new MailUser() { EmailId = userView.EmailId, Name = userView.UserName });
                    var mail = _emailExtention.SendEmail(emailView);

                    if (mail == "OnSuccess")
                    {
                        result.Status = Status.Success;
                        result.StatusCode = HttpStatusCode.OK;
                        result.Message = "Success";
                        return result;
                    }
                    else
                    {
                        result.Status = Status.Fail;
                        result.StatusCode = HttpStatusCode.BadRequest;
                        result.Message = "Fail";
                        return result;
                    }
                }
                else
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Body = "wrongEmail";
                    return result;
                }
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;

                return result;
            }
        }

        public IResult ValidateToken(string token)
        {
            var result = new Result
            {
                Operation = Operation.Read,
                Status = Status.Success
            };
            try
            {
                var tokenDetail = _loginRepository.ValidateToken(token);
                if (tokenDetail != null)
                {
                    result.Status = Status.Success;
                    result.StatusCode = HttpStatusCode.OK;
                    result.Message = "validToken";
                    return result;
                }
                else
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "invalidToken";
                    return result;
                }
            }
            catch (Exception e)
            {
                result.Status = Status.Error;
                result.Message = e.Message;
                result.StatusCode = HttpStatusCode.InternalServerError;

                return result;
            }
        }

        public async Task<IResult> ChangePassword(string token, string newPassword)
        {
            var result = new Result
            {
                Operation = Operation.Update,
                Status = Status.Success
            };
            try
            {
                var tokenVerify = _loginRepository.ValidateToken(token);
                if (tokenVerify != null && newPassword != null)
                {
                    await _loginRepository.ChangePassword(token, newPassword);
                    result.Status = Status.Success;
                    result.StatusCode = HttpStatusCode.OK;
                    result.Message = "Success";
                    return result;
                }
                else
                {
                    result.Status = Status.Fail;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Message = "This page does not exist.";
                    return result;
                }
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
