using ChatCase.Core.Domain.Identity;
using ChatCase.Domain.Common;
using ChatCase.Domain.Dto.Request.Identity;
using ChatCase.Domain.Dto.Response.Identity;
using System.Threading.Tasks;

namespace ChatCase.Business.Interfaces.Identity
{
    public interface IUserService
    {
        /// <summary>
        /// Register a user and returns a service response model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ServiceResponse<RegisterResponse>> RegisterAsync(RegisterRequest model);

        /// <summary>
        /// Returns an user by userName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<AppUser> GetByUserNameAsync(string userName);

        /// <summary>
        /// Returns an user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<AppUser> GetByEmailAsync(string email);

        /// <summary>
        /// Returns an user by userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<AppUser> GetByIdAsync(string userId);
    }
}
