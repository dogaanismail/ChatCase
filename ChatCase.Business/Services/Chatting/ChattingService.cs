using ChatCase.Business.Interfaces.Chatting;
using ChatCase.Business.Services.Logging;
using ChatCase.Core.Domain.Chatting;
using ChatCase.Core.Domain.Identity;
using ChatCase.Core.Infrastructure.Mapper;
using ChatCase.Domain.Common;
using ChatCase.Domain.Dto.Request.Chatting;
using ChatCase.Domain.Dto.Response.Chatting;
using ChatCase.Repository.Generic;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace ChatCase.Business.Services.Chatting
{
    public class ChattingService : ServiceResponse, IChattingService
    {
        #region Fields
        private readonly IRepository<ChatGroup> _chatGroupRepository;
        private readonly IRepository<Chat> _chatRepository;
        private readonly IRepository<ChatGroupUser> _chatGroupUserRepository;
        private readonly UserManager<AppUser> _userManager;

        #endregion

        #region Ctor
        public ChattingService(IRepository<ChatGroup> chatGroupRepository,
            IRepository<Chat> chatRepository,
            IRepository<ChatGroupUser> chatGroupUserRepository,
            UserManager<AppUser> userManager)
        {
            _chatGroupRepository = chatGroupRepository;
            _chatRepository = chatRepository;
            _chatGroupUserRepository = chatGroupUserRepository;
            _userManager = userManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a user from a group
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public virtual async Task DeleteUserFromGroupAsync(string groupName, string userName)
        {
            var appUser = await _userManager.FindByNameAsync(userName);

            if (appUser is not null)
            {
                var chatGroup = await _chatGroupRepository.GetAsync(x => x.Name == groupName);

                await _chatGroupUserRepository.DeleteAsync(cgu => cgu.ChatGroupId == chatGroup.Id && cgu.MemberId == appUser.Id);
            }
        }

        /// <summary>
        /// Returns the group exists or not
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public virtual async Task<bool> GroupExistsAsync(string groupName)
        {
            var chatGroup = await _chatGroupRepository.GetAsync(cg => cg.Name == groupName);

            if (chatGroup == null)
                return false;

            return true;
        }

        /// <summary>
        /// Creates a group
        /// </summary>
        /// <param name="createRequest"></param>
        /// <returns></returns>
        public virtual async Task<ServiceResponse<string>> CreateGroupAsync(ChatGroupCreateRequest createRequest)
        {
            if (createRequest is null)
                throw new ArgumentNullException(nameof(createRequest));

            var serviceResponse = new ServiceResponse<string>
            {
                Success = false
            };

            try
            {
                ChatGroup newEntity = new();
                newEntity.Id = ObjectId.GenerateNewId().ToString();
                newEntity.Name = createRequest.Name;
                newEntity.GroupFlag = createRequest.GroupFlag;

                await _chatGroupRepository.AddAsync(newEntity);

                foreach (var userId in createRequest.AppUserIds)
                {
                    newEntity.ChatGroupMembers.Add(new ChatGroupUser
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        ChatGroupId = newEntity.Id,
                        MemberId = userId
                    });
                }

                await _chatGroupRepository.UpdateAsync(newEntity.Id, newEntity);

                serviceResponse.Success = true;
                serviceResponse.ResultCode = ResultCode.Success;
                serviceResponse.Data = $"{createRequest.Name} group has been successfully created!";

                return serviceResponse;
            }
            catch (Exception ex)
            {
                LoggerFactory.DatabaseLogManager().Error($"ChattingService- CreateGroupAsync error: {JsonConvert.SerializeObject(ex)}");
                serviceResponse.Success = false;
                serviceResponse.ResultCode = ResultCode.Exception;
                serviceResponse.Warnings.Add(ex.Message);
                serviceResponse.Data = $"{createRequest.Name} group could not be created!";
                return serviceResponse;
            }
        }

        /// <summary>
        /// Returns the group exists or not
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public virtual async Task<bool> GroupExistsByIdAsync(string groupId)
        {
            var chatGroup = await _chatGroupRepository.GetAsync(cg => cg.Id == groupId);

            if (chatGroup == null)
                return false;

            return true;
        }

        /// <summary>
        /// Creates a message
        /// </summary>
        /// <param name="messageRequest"></param>
        /// <returns></returns>
        public virtual async Task<ServiceResponse<string>> CreateMessageAsync(ChatGroupMessageRequest messageRequest)
        {
            if (messageRequest is null)
                throw new ArgumentNullException(nameof(messageRequest));

            var serviceResponse = new ServiceResponse<string>
            {
                Success = false
            };

            try
            {
                Chat newChat = new()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    SenderId = messageRequest.SenderId,
                    ChatGroupId = messageRequest.ChatGroupId,
                    Text = messageRequest.Text,
                };

                await _chatRepository.AddAsync(newChat);

                serviceResponse.Success = true;
                serviceResponse.ResultCode = ResultCode.Success;
                serviceResponse.Data = "Message has been successfully created!";

                return serviceResponse;
            }
            catch (Exception ex)
            {
                LoggerFactory.DatabaseLogManager().Error($"ChattingService- CreateMessageAsync error: {JsonConvert.SerializeObject(ex)}");
                serviceResponse.Success = false;
                serviceResponse.ResultCode = ResultCode.Exception;
                serviceResponse.Warnings.Add(ex.Message);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Gets messages by group name
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public virtual async Task<MessageDto> GetMessagesByGroupNameAsync(string groupName)
        {
            var chatGroup = await _chatGroupRepository.GetAsync(x => x.Name == groupName);

            var messages = await _chatRepository.GetListAsync(x => x.ChatGroupId == chatGroup.Id);
            return AutoMapperConfiguration.Mapper.Map<MessageDto>(messages);
        }

        /// <summary>
        /// Deletes a group by Id
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public virtual async Task<ServiceResponse<string>> DeleteGroupAsync(string groupId)
        {
            if (string.IsNullOrEmpty(groupId))
                throw new ArgumentNullException(nameof(groupId));

            var serviceResponse = new ServiceResponse<string>
            {
                Success = false
            };

            try
            {
                await _chatGroupRepository.DeleteAsync(groupId);

                serviceResponse.Success = true;
                serviceResponse.ResultCode = ResultCode.Success;
                serviceResponse.Data = "Group has been successfully deleted!";

                return serviceResponse;
            }
            catch (Exception ex)
            {
                LoggerFactory.DatabaseLogManager().Error($"ChattingService- DeleteGroupAsync error: {JsonConvert.SerializeObject(ex)}");
                serviceResponse.Success = false;
                serviceResponse.ResultCode = ResultCode.Exception;
                serviceResponse.Warnings.Add(ex.Message);
                return serviceResponse;
            }
        }

        #endregion
    }
}
