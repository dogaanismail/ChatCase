using ChatCase.Core;
using ChatCase.Core.Domain.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using System;
using System.Threading.Tasks;

namespace ChatCase.Framework
{
    public class WebWorkContext : IWorkContext
    {
        #region Fields
        private IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        private AppUser _cachedUser;

        #endregion

        #region Ctor

        public WebWorkContext(IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Gets the current user/ guest user
        /// </summary>
        /// <returns></returns>
        public virtual async Task<AppUser> GetCurrentUserAsync()
        {
            if (_cachedUser != null)
                return _cachedUser;

            await SetCurrentUserAsync();

            return _cachedUser;
        }

        /// <summary>
        /// Sets current app user
        /// </summary>
        /// <param name="appUser"></param>
        /// <returns></returns>
        public virtual async Task SetCurrentUserAsync(AppUser appUser = null)
        {
            if (appUser == null)
            {
                if (_httpContextAccessor.HttpContext?.User != null && !string.IsNullOrEmpty(_httpContextAccessor.HttpContext?.User?.Identity.Name))
                    appUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User);

                if (appUser == null)
                    appUser = await CreateGuestUser();
            }

            _cachedUser = appUser;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a guest user
        /// </summary>
        /// <returns></returns>
        private async Task<AppUser> CreateGuestUser()
        {
            AppUser userEntity = new();
            userEntity.Id = ObjectId.GenerateNewId().ToString();
            userEntity.UserName = $"GuestUser-{userEntity.Id}";
            userEntity.Email = $"guest-{userEntity.Id}@chatcase.com";
            userEntity.RegisteredDate = DateTime.UtcNow;

            IdentityResult result = await _userManager.CreateAsync(userEntity);
            if (result.Succeeded)
            {
                await CheckRoleExistsAsync();
                await _userManager.AddToRoleAsync(userEntity, "Guest");
                return userEntity;
            }

            return null;
        }

        #region Private Methods

        private async Task CheckRoleExistsAsync()
        {
            if (!await _roleManager.RoleExistsAsync("Guest"))
                await _roleManager.CreateAsync(new AppRole
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Name = "Guest",
                    CreatedAt = DateTime.UtcNow
                });
        }

        #endregion

        #endregion
    }
}
