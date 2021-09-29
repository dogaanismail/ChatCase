using ChatCase.Business.Interfaces.Chatting;
using ChatCase.Business.Interfaces.Logging;
using ChatCase.Business.Services.Logging;
using ChatCase.Core.Domain.Identity;
using ChatCase.Domain.Common;
using ChatCase.Domain.Dto.Request.Chatting;
using ChatCase.Framework.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChatCase.Api.Controllers
{
    [ApiController]
    public class ChattingController : BaseApiController
    {
        #region Fields
        private readonly IChattingService _chattingService;
        private readonly IAppUserActivityService _userActivityService;

        #endregion

        #region Ctor
        public ChattingController(IChattingService chattingService,
            IAppUserActivityService userActivityService)
        {
            _chattingService = chattingService;
            _userActivityService = userActivityService;
        }
        #endregion

        #region Methods

        [HttpPost("create-group")]
        [Authorize]
        public virtual async Task<IActionResult> CreateGroupAsync([FromBody] ChatGroupCreateRequest model)
        {
            LoggerFactory.DatabaseLogManager().Information($"ChattingController- CreateGroupAsync request: {JsonConvert.SerializeObject(model)}");

            var serviceResponse = await _chattingService.CreateGroupAsync(model);

            if (serviceResponse.Warnings.Count > 0 || serviceResponse.Warnings.Any())
            {
                LoggerFactory.DatabaseLogManager().Error($"ChattingController- CreateGroupAsync error: {JsonConvert.SerializeObject(serviceResponse)}");

                return BadResponse(new ResultModel
                {
                    Status = false,
                    Message = string.Join(Environment.NewLine, serviceResponse.Warnings.Select(err => string.Join(Environment.NewLine, err))),
                });
            }

            await _userActivityService.InsertAsync(nameof(AppUser), "UserCreatedGroup");

            return OkResponse(Result);
        }

        [HttpDelete("delete-group/id/{id}")]
        [Authorize]
        public virtual async Task<IActionResult> DeleteGroupAsync(string id)
        {
            var serviceResponse = await _chattingService.DeleteGroupAsync(id);

            if (serviceResponse.Warnings.Count > 0 || serviceResponse.Warnings.Any())
            {
                LoggerFactory.DatabaseLogManager().Error($"ChattingController- DeleteGroupAsync error: {JsonConvert.SerializeObject(serviceResponse)}");

                return BadResponse(new ResultModel
                {
                    Status = false,
                    Message = string.Join(Environment.NewLine, serviceResponse.Warnings.Select(err => string.Join(Environment.NewLine, err))),
                });
            }

            await _userActivityService.InsertAsync(nameof(AppUser), "UserDeletedGroup");

            return OkResponse(Result);
        }

        #endregion
    }
}
