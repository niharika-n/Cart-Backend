using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using Microsoft.AspNetCore.Http;
using ViewModel.Model;
using Common.CommonData;
using Service.Interfaces;
using Common.Enums;

namespace WebAPIs.Controllers
{
    /// <summary>
    /// Login controller.
    /// </summary>
    [Route("api")]
    [ApiController]
    public class LoginController : Controller
    {
        private IConfiguration _config;
        private ILoginService _loginService;

        public LoginController(IConfiguration config, ILoginService loginService)
        {
            _config = config;
            _loginService = loginService;
        }


        /// <summary>
        /// Authenticates the user.
        /// </summary>
        /// <param name="loginModel">username and password of user.</param>
        /// <returns>
        /// Token string for correct details.
        /// </returns>
        [HttpGet("login")]
        [ProducesResponseType(typeof(UserViewModel), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public IResult LoginUser([FromQuery] LoginModel loginModel)
        {
            var user = _loginService.LoginUser(loginModel);
            if (user.Status == Status.Success)
            {
                var userObj = user.Body;
                var tokenObj = BuildToken(userObj);
                user.Body = new { token = tokenObj, userObj };
            }

            return user;
        }


        /// <summary>
        /// Generates the token.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>
        /// Returns token generated.
        /// </returns>        
        private string BuildToken(UserViewModel user)
        {
            var claims = new[] {
                                new Claim(ClaimTypes.NameIdentifier , user.UserId.ToString()),
                                new Claim(ClaimTypes.Name, user.UserName),
                                new Claim(ClaimTypes.Email, user.EmailId),
                                new Claim("Roles", string.Join(",", user.Roles))
                            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(75),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        /// <summary>
        /// Forgot password to reset new password.
        /// </summary>
        /// <param name="Username">Username/Email Address of user.</param>
        /// <returns>
        /// Status with message for email status.
        /// </returns>
        [HttpGet("forgotpassword")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public Task<IResult> ForgotPassword([FromQuery] string userName)
        {
            return _loginService.MailPasswordLink(userName);
        }


        /// <summary>
        /// Vadilates reset token.
        /// </summary>
        /// <param name="token">Token for reset password.</param>
        /// <returns>
        /// Returns status for email message sent.
        /// </returns>
        [HttpGet("validatetoken")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public IResult ValidateToken(string token)
        {
            var checkToken = _loginService.ValidateToken(token);
            return checkToken;
        }


        /// <summary>
        /// Change user paswword.
        /// </summary>
        /// <param name="userToken">usertoken for verification of url</param>
        /// <param name="newPassword">new password for user.</param>
        /// <returns>
        /// Status with message string.
        /// </returns>
        [HttpPut("changepassword")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public Task<IResult> ChangePassword(string userToken, string newPassword)
        {
            return _loginService.ChangePassword(userToken, newPassword);
        }

    }
}



