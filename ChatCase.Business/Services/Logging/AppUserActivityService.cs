using ChatCase.Business.Interfaces.Identity;
using ChatCase.Business.Interfaces.Logging;
using ChatCase.Core;
using ChatCase.Core.Domain.Identity;
using ChatCase.Core.Domain.Logging;
using ChatCase.Core.Infrastructure;
using ChatCase.Repository.Generic;
using System;
using System.Threading.Tasks;

namespace ChatCase.Business.Services.Logging
{
    public class AppUserActivityService : IAppUserActivityService
    {
        #region Fields
        private readonly IRepository<ActivityLog> _activityLogRepository;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor
        public AppUserActivityService(IRepository<ActivityLog> activityLogRepository,
            IWebHelper webHelper,
            IWorkContext workContext)
        {
            _activityLogRepository = activityLogRepository;
            _webHelper = webHelper;
            _workContext = workContext;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Gets an activity by activityId
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public virtual async Task<ActivityLog> GetActivityByIdAsync(string activityId)
        {
            if (string.IsNullOrEmpty(activityId))
                throw new ArgumentNullException(nameof(activityId));

            return await _activityLogRepository.GetByIdAsync(activityId);
        }

        /// <summary>
        /// Gets an activity by userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<ActivityLog> GetActivityByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            return await _activityLogRepository.GetAsync(x => x.AppUserId == userId);
        }

        /// <summary>
        /// Gets an activity by userName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public virtual async Task<ActivityLog> GetActivityByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));

            var appUser = await EngineContext.Current.Resolve<IUserService>().GetByUserNameAsync(userName);

            if (appUser != null)
                return await _activityLogRepository.GetAsync(x => x.AppUserId == appUser.Id);

            return null;
        }

        /// <summary>
        /// Inserts an activity log item
        /// </summary>
        /// <param name="appUser"></param>
        /// <param name="systemKeyword"></param>
        /// <param name="comment"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<ActivityLog> InsertActivityAsync(string entityId, string entityName, string comment, AppUser appUser = null)
        {
            var logItem = new ActivityLog
            {
                EntityId = entityId,
                EntityName = entityName,
                AppUserId = appUser?.Id,
                Comment = CommonHelper.EnsureMaximumLength(comment ?? string.Empty, 4000),
                CreatedOnUtc = DateTime.UtcNow,
                IpAddress = _webHelper.GetCurrentIpAddress()
            };

            await _activityLogRepository.AddAsync(logItem);

            return logItem;
        }

        /// <summary>
        /// Inserts an activity log item
        /// </summary>
        /// <param name="appUser"></param>
        /// <param name="systemKeyword"></param>
        /// <param name="comment"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<ActivityLog> InsertActivityAsync(string entityName, string comment, AppUser appUser = null)
        {
            var logItem = new ActivityLog
            {
                EntityId = appUser.Id,
                EntityName = entityName,
                AppUserId = appUser?.Id,
                Comment = CommonHelper.EnsureMaximumLength(comment ?? string.Empty, 4000),
                CreatedOnUtc = DateTime.UtcNow,
                IpAddress = _webHelper.GetCurrentIpAddress()
            };

            await _activityLogRepository.AddAsync(logItem);

            return logItem;
        }


        /// <summary>
        /// Inserts an activity log item
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public virtual async Task<ActivityLog> InsertActivityAsync(string entityName, string comment)
        {
            return await InsertActivityAsync(entityName, comment, await _workContext.GetCurrentUserAsync());
        }

        #endregion
    }
}
