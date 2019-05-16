using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Security.Principal;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net;
using Common.CommonData;
using Common.Enums;
using ViewModel.Model;
using Service.Interfaces;

namespace WebAPIs.Controllers
{
    /// <summary>
    /// User controller.
    /// </summary>
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IConfiguration config;
        private IUserService _userService;
        private readonly IHostingEnvironment environment;

        public UserController(IPrincipal _principal, IConfiguration _config, IHostingEnvironment _environment, IUserService userService)
        {
            config = _config;
            environment = _environment;
            _userService = userService;
        }


        /// <summary>
        /// Creates new user.
        /// </summary>
        /// <returns>
        /// IResult with Status.
        /// </returns>        
        [HttpPost("register")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public async Task<IResult> CreateUser()
        {
            var result = new Result
            {
                Operation = Operation.Create,
                Status = Status.Success
            };
            if (!ModelState.IsValid)
            {
                result.Status = Status.Fail;
                result.StatusCode = HttpStatusCode.BadRequest;
                return result;
            }
            IFormFile img = null;
            var model = JsonConvert.DeserializeObject<UserViewModel>(Request.Form["model"]);
            var image = Request.Form.Files;
            foreach (var i in image)
            {
                img = image[0];
            }
            var newUser = await _userService.CreateUser(model, img, false);
            return newUser;
        }


        /// <summary>
        /// Creates new user.
        /// </summary>
        /// <returns>
        /// IResult with Status.
        /// </returns>
        [HttpPost("registerfromadmin")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public async Task<IResult> CreateUserFromAdmin()
        {
            var result = new Result
            {
                Operation = Operation.Create,
                Status = Status.Success
            };
            if (!ModelState.IsValid)
            {
                result.Status = Status.Fail;
                result.StatusCode = HttpStatusCode.BadRequest;
                return result;
            }
            try
            {
                IFormFile img = null;
                var file = JsonConvert.DeserializeObject<UserViewModel>(Request.Form["model"]);
                var image = Request.Form.Files;
                foreach (var i in image)
                {
                    img = image[0];
                }
                var totalRoles = Enum.GetValues(typeof(UserRoles)).Cast<int>();
                var selectedRoles = file.Roles.Intersect(totalRoles).ToList();
                if (selectedRoles.Contains((int)UserRoles.Admin) && selectedRoles.Count() == 1)
                {
                    selectedRoles.Add((int)UserRoles.User);
                }
                List<int> roleList = new List<int>();
                foreach (var role in selectedRoles)
                {
                    UserRoles roleVal = (UserRoles)role;
                    roleList.Add((int)roleVal);
                }
                file.Roles = roleList.ToArray();
                var newUser = await _userService.CreateUser(file, img, true);
                return newUser;
            }
            catch(Exception e)
            {
                result.Status = Status.Error;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Body = e;
                return result;
            }
        }


        /// <summary>
        /// User detail.
        /// </summary>
        /// <param name="id">Id of user</param>
        /// <returns>
        /// Returns user details.
        /// </returns>
        [HttpGet("detail/{id}")]
        [ProducesResponseType(typeof(UserViewModel), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IResult> Detail(int id)
        {
            var user = await _userService.GetDetail(id);
            return user;
        }


        /// <summary>
        /// Update user details.
        /// </summary>
        /// <returns>
        /// Detail of user updated.
        /// </returns>
        [HttpPut("update")]
        [ProducesResponseType(typeof(UserViewModel), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IResult> Update()
        {
            var result = new Result
            {
                Operation = Operation.Update,
                Status = Status.Success
            };
            if (!ModelState.IsValid)
            {
                result.Status = Status.Fail;
                result.StatusCode = HttpStatusCode.BadRequest;
                return result;
            }
            var model = JsonConvert.DeserializeObject<UserViewModel>(Request.Form["model"]);
            var emptyUser = await _userService.GetDetail(model.UserId);
            if (emptyUser.Status != Status.Success)
            {
                return emptyUser;
            }
            IFormFile img = null;
            if (Request.Form.Files.Count != 0)
            {
                var image = Request.Form.Files;
                foreach (var i in image)
                {
                    img = image[0];
                }
            }
            var updateUser = await _userService.Update(model, img);
            return updateUser;
        }


        /// <summary>
        /// Change password.
        /// </summary>
        /// <param name="oldPassword">Old password of user.</param>
        /// <param name="newPassword">New password set.</param>
        /// <returns>
        /// Status of password changed.
        /// </returns>
        [HttpPut("changepassword")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IResult> ChangePassword(string oldPassword, string newPassword)
        {
            var changePassword = await _userService.ChangePassword(oldPassword, newPassword);
            return changePassword;
        }

        /// <summary>
        /// user list.
        /// </summary>
        /// <returns>
        /// Returns list of user to superAdmin only.
        /// </returns>
        [HttpGet("getuserlist")]
        [ProducesResponseType(typeof(List<UserViewModel>), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "SuperAdminOnly")]
        public IResult GetUserList([FromQuery] DataHelperModel dataHelper)
        {
            return _userService.GetUserList(dataHelper);
        }


        /// <summary>
        /// Assign user roles.
        /// </summary>
        /// <param name="id">id of user.</param>
        /// <param name="add">check for addition of new roles</param>
        /// <param name="selectedRoles">new roles.</param>
        /// <returns>
        /// Iresult with status.
        /// </returns>
        [HttpGet("changeuserrole")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(IResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "SuperAdminOnly")]
        public async Task<IResult> ChangeUserRole(int id, bool add, string selectedRoles)
        {
            var changeRoles = await _userService.ChangeUserRole(id, add, selectedRoles);
            return changeRoles;
        }

    }
}