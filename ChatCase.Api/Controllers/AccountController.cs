using ChatCase.Business.Interfaces.Identity;
using ChatCase.Business.Services.Logging;
using ChatCase.Core.Domain.Identity;
using ChatCase.Core.Security;
using ChatCase.Domain.Common;
using ChatCase.Domain.Dto.Request.Identity;
using ChatCase.Domain.Dto.Response.Identity;
using ChatCase.Framework.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChatCase.Api.Controllers
{
    [ApiController]
    public partial class AccountController : BaseApiController
    {
        #region Fields
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IUserService _userService;

        #endregion

        #region Ctor
        public AccountController(ITokenService tokenService,
            SignInManager<AppUser> signInManager,
            IUserService userService)
        {
            _tokenService = tokenService;
            _signInManager = signInManager;
            _userService = userService;

        }
        #endregion

        #region Methods

        /// <summary>
        /// User register async 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public virtual async Task<JsonResult> RegisterAsync([FromBody] RegisterRequest model)
        {
            LoggerFactory.DatabaseLogManager().Information($"AccountController- RegisterAsync request: {JsonConvert.SerializeObject(model)}");

            var serviceResponse = await _userService.RegisterAsync(model);

            if (serviceResponse.Warnings.Count > 0 || serviceResponse.Warnings.Any())
            {
                LoggerFactory.DatabaseLogManager().Error($"AccountController- RegisterAsync error: {JsonConvert.SerializeObject(serviceResponse)}");

                return BadResponse(new ResultModel
                {
                    Status = false,
                    Message = string.Join(Environment.NewLine, serviceResponse.Warnings.Select(err => string.Join(Environment.NewLine, err))),
                });
            }

            return OkResponse(Result);
        }

        /// <summary>
        /// User login async
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public virtual async Task<JsonResult> LoginAsync([FromBody] LoginRequest model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                var user = await _userService.GetByUserNameAsync(model.UserName);

                var token = _tokenService.GenerateToken(new AppUserDto
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email                 
                });

                return OkResponse(token);
            }
            else
            {
                LoggerFactory.DatabaseLogManager().Error($"AccountController- LoginAsync error: {JsonConvert.SerializeObject(result)}");
                Result.Status = false;
                Result.Message = "Username or password are wrong !";
                return BadResponse(Result);
            }
        }

        [HttpPost("logout")]
        public virtual async Task<JsonResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            Result.Status = true;
            Result.Message = "Successfully has logged out !";
            return OkResponse(Result);
        }

        #endregion
    }
}
