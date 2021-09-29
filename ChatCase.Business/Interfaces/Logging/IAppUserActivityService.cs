using ChatCase.Core.Domain.Identity;
using ChatCase.Core.Domain.Logging;
using ChatCase.Domain.Dto.Response.Logging;
using System.Collections.Generic;
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
        Task<ActivityLog> GetByIdAsync(string activityId);

        /// <summary>
        /// Gets an activity by activityId
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        Task<ActivityLogDto> GetByIdDtoAsync(string activityId);

        /// <summary>
        /// Gets activity list by userName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<List<ActivityLogDto>> GetActivitiesByUserName(string userName);

        /// <summary>
        /// Gets activity list by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<List<ActivityLogDto>> GetActivitiesByEmail(string email);

        /// <summary>
        /// Inserts an activity log item
        /// </summary>
        /// <param name="appUser"></param>
        /// <param name="systemKeyword"></param>
        /// <param name="comment"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<ActivityLog> InsertAsync(string entityId, string entityName, string comment, AppUser appUser = null);

        /// <summary>
        /// Inserts an activity log item
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        Task<ActivityLog> InsertAsync( string entityName, string comment);
    }
}
