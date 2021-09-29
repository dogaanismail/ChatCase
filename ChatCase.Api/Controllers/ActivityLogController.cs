using ChatCase.Business.Interfaces.Logging;
using ChatCase.Framework.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ChatCase.Api.Controllers
{
    [ApiController]
    public partial class ActivityLogController : BaseApiController
    {
        #region Fields
        private readonly IAppUserActivityService _userActivityService;

        #endregion

        #region Ctor

        public ActivityLogController(IAppUserActivityService userActivityService)
        {
            _userActivityService = userActivityService;
        }
        #endregion

        #region Methods

        [HttpGet("id/{id}")]
        [AllowAnonymous]
        public virtual async Task<IActionResult> GetByIdAsync(string id)
        {
            var data = await _userActivityService.GetByIdDtoAsync(id);

            return OkResponse(data);
        }

        [HttpGet("get/username/{username}")]
        [AllowAnonymous]
        public virtual async Task<IActionResult> GetActivitiesByUserNameAsync(string username)
        {
            var data = await _userActivityService.GetActivitiesByUserName(username);

            return OkResponse(data);
        }

        [HttpGet("get/email/{email}")]
        [AllowAnonymous]
        public virtual async Task<IActionResult> GetActivitiesByEmailAsync(string email)
        {
            var data = await _userActivityService.GetActivitiesByUserName(email);

            return OkResponse(data);
        }

        #endregion
    }
}
