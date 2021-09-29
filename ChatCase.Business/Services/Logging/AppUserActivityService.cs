using ChatCase.Business.Interfaces.Identity;
using ChatCase.Business.Interfaces.Logging;
using ChatCase.Core;
using ChatCase.Core.Domain.Identity;
using ChatCase.Core.Domain.Logging;
using ChatCase.Core.Infrastructure;
using ChatCase.Core.Infrastructure.Mapper;
using ChatCase.Domain.Dto.Response.Logging;
using ChatCase.Repository.Generic;
using System;
using System.Collections.Generic;
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
        public virtual async Task<ActivityLog> GetByIdAsync(string activityId)
        {
            if (string.IsNullOrEmpty(activityId))
                throw new ArgumentNullException(nameof(activityId));

            return await _activityLogRepository.GetByIdAsync(activityId);
        }

        /// <summary>
        /// Gets an activity by activityId
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public virtual async Task<ActivityLogDto> GetByIdDtoAsync(string activityId)
        {
            if (string.IsNullOrEmpty(activityId))
                throw new ArgumentNullException(nameof(activityId));

            var activity = await _activityLogRepository.GetByIdAsync(activityId);
            return AutoMapperConfiguration.Mapper.Map<ActivityLogDto>(activity);
        }

        /// <summary>
        /// Gets activity list by userName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public virtual async Task<List<ActivityLogDto>> GetActivitiesByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));

            var appUser = await EngineContext.Current.Resolve<IUserService>().GetByUserNameAsync(userName);

            if (appUser != null)
            {
                var activityList = await _activityLogRepository.GetListAsync(x => x.AppUserId == appUser.Id);
                return AutoMapperConfiguration.Mapper.Map<List<ActivityLogDto>>(activityList);
            }

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
        public virtual async Task<ActivityLog> InsertAsync(string entityId, string entityName, string comment, AppUser appUser = null)
        {
            var logItem = new ActivityLog
            {
                EntityId = entityId,
                EntityName = entityName,
                AppUserId = appUser?.Id,
                Comment = CommonHelper.EnsureMaximumLength(comment ?? string.Empty, 4000),
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
        public virtual async Task<ActivityLog> InsertAsync(string entityName, string comment, AppUser appUser = null)
        {
            var logItem = new ActivityLog
            {
                EntityId = appUser.Id,
                EntityName = entityName,
                AppUserId = appUser?.Id,
                Comment = CommonHelper.EnsureMaximumLength(comment ?? string.Empty, 4000),
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
        public virtual async Task<ActivityLog> InsertAsync(string entityName, string comment)
        {
            return await InsertAsync(entityName, comment, await _workContext.GetCurrentUserAsync());
        }

        /// <summary>
        /// Gets activity list by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public virtual async Task<List<ActivityLogDto>> GetActivitiesByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email));

            var appUser = await EngineContext.Current.Resolve<IUserService>().GetByUserNameAsync(email);

            if (appUser != null)
            {
                var activityList = await _activityLogRepository.GetListAsync(x => x.AppUserId == appUser.Id);
                return AutoMapperConfiguration.Mapper.Map<List<ActivityLogDto>>(activityList);
            }

            return null;
        }

        #endregion
    }
}
