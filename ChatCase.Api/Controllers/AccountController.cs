using ChatCase.Business.Interfaces.Identity;
using ChatCase.Business.Interfaces.Logging;
using ChatCase.Business.Services.Logging;
using ChatCase.Core;
using ChatCase.Core.Domain.Identity;
using ChatCase.Domain.Common;
using ChatCase.Domain.Dto.Request.Identity;
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
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IUserService _userService;
        private readonly IAppUserActivityService _userActivityService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor
        public AccountController(SignInManager<AppUser> signInManager,
            IUserService userService,
            IAppUserActivityService userActivityService,
            IWorkContext workContext)
        {
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

            if (result.Succeeded)
            {
                await _userActivityService.InsertAsync(nameof(AppUser), "UserLoggedIn");

                Result.Status = true;
                Result.Message = "Successfully has logged in !";
                return OkResponse(Result);
            }
            else
            {
                await _userActivityService.InsertAsync(nameof(AppUser), "UserLoginError");

                LoggerFactory.DatabaseLogManager().Error($"AccountController- LoginAsync error: {JsonConvert.SerializeObject(result)}");
                Result.Status = false;
                Result.Message = "Username or password are wrong !";
                return BadResponse(Result);
            }
        }

        [HttpPost("logout")]
        public virtual async Task<IActionResult> LogOut()
        {
            await _userActivityService.InsertAsync(nameof(AppUser), "UserLogOut");

            await _signInManager.SignOutAsync();
            Result.Status = true;
            Result.Message = "Successfully has logged out !";
            return OkResponse(Result);
        }

        #endregion
    }
}
