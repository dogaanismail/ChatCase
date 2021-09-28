using ChatCase.Business.Interfaces.Identity;
using ChatCase.Business.Interfaces.Logging;
using ChatCase.Business.Services.Logging;
using ChatCase.Core;
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
        private readonly IAppUserActivityService _userActivityService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor
        public AccountController(ITokenService tokenService,
            SignInManager<AppUser> signInManager,
            IUserService userService,
            IAppUserActivityService userActivityService,
            IWorkContext workContext)
        {
            _tokenService = tokenService;
            _signInManager = signInManager;
            _userService = userService;
            _userActivityService = userActivityService;
            _workContext = workContext;

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
        public virtual async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest model)
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
        public virtual async Task<IActionResult> LoginAsync([FromBody] LoginRequest model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);

            var appUser = await _userService.GetByUserNameAsync(model.UserName);

            if (result.Succeeded && appUser != null)
            {
                var token = _tokenService.GenerateToken(new AppUserDto
                {
                    UserId = appUser.Id,
                    UserName = appUser.UserName,
                    Email = appUser.Email
                });

                await _userActivityService.InsertActivityAsync(nameof(AppUser), "UserLoggedIn");

                return OkResponse(token);
            }
            else
            {
                await _userActivityService.InsertActivityAsync(nameof(AppUser), "UserLoginError");

                LoggerFactory.DatabaseLogManager().Error($"AccountController- LoginAsync error: {JsonConvert.SerializeObject(result)}");
                Result.Status = false;
                Result.Message = "Username or password are wrong !";
                return BadResponse(Result);
            }
        }

        [HttpPost("logout")]
        public virtual async Task<IActionResult> LogOut()
        {
            await _userActivityService.InsertActivityAsync(nameof(AppUser), "UserLoginError");

            await _signInManager.SignOutAsync();
            Result.Status = true;
            Result.Message = "Successfully has logged out !";
            return OkResponse(Result);
        }

        #endregion
    }
}
