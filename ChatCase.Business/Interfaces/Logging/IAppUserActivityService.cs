using ChatCase.Core.Domain.Identity;
using ChatCase.Core.Domain.Logging;
using System.Threading.Tasks;

namespace ChatCase.Business.Interfaces.Logging
{
    public interface IAppUserActivityService
    {
        /// <summary>
        /// Gets an activity by activityId
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        Task<ActivityLog> GetActivityByIdAsync(string activityId);

        /// <summary>
        /// Gets an activity by userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ActivityLog> GetActivityByUserId(string userId);

        /// <summary>
        /// Gets an activity by userName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<ActivityLog> GetActivityByUserName(string userName);

        /// <summary>
        /// Inserts an activity log item
        /// </summary>
        /// <param name="appUser"></param>
        /// <param name="systemKeyword"></param>
        /// <param name="comment"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<ActivityLog> InsertActivityAsync(string entityId, string entityName, string comment, AppUser appUser = null);

        /// <summary>
        /// Inserts an activity log item
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        Task<ActivityLog> InsertActivityAsync( string entityName, string comment);
    }
}
