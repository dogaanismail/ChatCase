using ChatCase.Business.Interfaces.Identity;
using ChatCase.Business.Interfaces.Logging;
using ChatCase.Business.Services.Logging;
using ChatCase.Core.Domain.Identity;
using ChatCase.Domain.Common;
using ChatCase.Domain.Dto.Request.Identity;
using ChatCase.Domain.Dto.Response.Identity;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatCase.Business.Services.Identity
{
    /// <summary>
    /// User service implementations
    /// </summary>
    public partial class UserService : ServiceExecute, IUserService
    {
        #region Fields
        private readonly UserManager<AppUser> _userManager;
        private readonly IAppUserActivityService _userActivityService;
        private readonly RoleManager<AppRole> _roleManager;

        #endregion

        #region Ctor

        public UserService(UserManager<AppUser> userManager,
            IAppUserActivityService userActivityService,
             RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _userActivityService = userActivityService;
            _roleManager = roleManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Register a user and returns a service response model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ServiceResponse<RegisterResponse>> RegisterAsync(RegisterRequest model)
        {
            if (!model.RePassword.Equals(model.Password))
                return ServiceResponse((RegisterResponse)null, new List<string> { "Repassword must match password!" });

            var serviceResponse = new ServiceResponse<RegisterResponse>
            {
                Success = false
            };

            try
            {
                AppUser userEntity = new()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    UserName = model.UserName,
                    Email = model.Email,
                    RegisteredDate = DateTime.UtcNow
                };

                IdentityResult result = await _userManager.CreateAsync(userEntity, model.Password);

                if (!result.Succeeded && result.Errors != null && result.Errors.Any())
                {
                    await _userActivityService.InsertAsync(userEntity.Id, nameof(AppUser), "UserRegisterError", userEntity);

                    LoggerFactory.DatabaseLogManager().Error($"UserService- RegisterAsync error: {JsonConvert.SerializeObject(result)}");

                    serviceResponse.Warnings = result.Errors.Select(x => x.Description).ToList();
                    return serviceResponse;
                }

                await CheckRoleExistsAsync();

                await _userManager.AddToRoleAsync(userEntity, "Registered");
                await _userManager.AddClaimAsync(userEntity, new Claim(ClaimTypes.Role, "Registered"));

                await _userActivityService.InsertAsync(userEntity.Id, nameof(AppUser), "UserRegistered", userEntity);

                serviceResponse.Success = true;
                serviceResponse.ResultCode = ResultCode.Success;
                serviceResponse.Data = new RegisterResponse
                {
                    Succeeded = result.Succeeded
                };

                return serviceResponse;
            }
            catch (Exception ex)
            {
                LoggerFactory.DatabaseLogManager().Error($"UserService- RegisterAsync error: {JsonConvert.SerializeObject(ex)}");
                serviceResponse.Success = false;
                serviceResponse.ResultCode = ResultCode.Exception;
                serviceResponse.Warnings.Add(ex.Message);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Returns an user by userName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public virtual async Task<AppUser> GetByUserNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));

            return await _userManager.FindByNameAsync(userName);
        }

        /// <summary>
        /// Returns an user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public virtual async Task<AppUser> GetByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email));

            return await _userManager.FindByEmailAsync(email);
        }

        /// <summary>
        /// Returns an user by userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<AppUser> GetByIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            return await _userManager.FindByIdAsync(userId);
        }

        /// <summary>
        /// Creates a guest user
        /// </summary>
        /// <returns></returns>
        public virtual async Task<AppUser> CreateGuestUser()
        {
            AppUser userEntity = new();
            userEntity.Id = ObjectId.GenerateNewId().ToString();
            userEntity.UserName = $"Guest User-{userEntity.Id}";
            userEntity.Email = $"guest-{userEntity.Id}@chatcase.com";

            IdentityResult result = await _userManager.CreateAsync(userEntity);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(userEntity, "Guest");
                return userEntity;
            }

            return null;
        }

        #endregion

        #region Private Methods
        private async Task CheckRoleExistsAsync()
        {
            if (!await _roleManager.RoleExistsAsync("Registered"))
                await _roleManager.CreateAsync(new AppRole
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Name = "Registered",
                    CreatedAt = DateTime.UtcNow
                });
        }

        #endregion
    }
}
