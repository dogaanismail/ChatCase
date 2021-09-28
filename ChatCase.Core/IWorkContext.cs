using ChatCase.Core.Domain.Identity;
using System.Threading.Tasks;

namespace ChatCase.Core
{
    public interface IWorkContext
    {
        /// <summary>
        /// Gets the current user/ guest user
        /// </summary>
        /// <returns></returns>
        Task<AppUser> GetCurrentUserAsync();

        /// <summary>
        /// Sets current app user
        /// </summary>
        /// <param name="appUser"></param>
        /// <returns></returns>
        Task SetCurrentUserAsync(AppUser appUser = null);
    }
}
