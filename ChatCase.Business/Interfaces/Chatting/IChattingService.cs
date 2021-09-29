using ChatCase.Domain.Common;
using ChatCase.Domain.Dto.Request.Chatting;
using ChatCase.Domain.Dto.Response.Chatting;
using System.Threading.Tasks;

namespace ChatCase.Business.Interfaces.Chatting
{
    public interface IChattingService
    {
        /// <summary>
        /// Deletes a user from a group
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task DeleteUserFromGroupAsync(string groupName, string userName);

        /// <summary>
        /// Returns the group exists or not
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        Task<bool> GroupExistsAsync(string groupName);

        /// <summary>
        /// Returns the group exists or not
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        Task<bool> GroupExistsByIdAsync(string groupId);

        /// <summary>
        /// Creates a group
        /// </summary>
        /// <param name="createRequest"></param>
        /// <returns></returns>
        Task<ServiceResponse<string>> CreateGroupAsync(ChatGroupCreateRequest createRequest);

        /// <summary>
        /// Creates a message
        /// </summary>
        /// <param name="messageRequest"></param>
        /// <returns></returns>
        Task<ServiceResponse<string>> CreateMessageAsync(ChatGroupMessageRequest messageRequest);

        /// <summary>
        /// Gets messages by group name
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        Task<MessageDto> GetMessagesByGroupNameAsync(string groupName);

        /// <summary>
        /// Deletes a group by Id
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<ServiceResponse<string>> DeleteGroupAsync(string groupId);
    }
}
