using ChatCase.Business.Interfaces.Chatting;
using ChatCase.Domain.Dto.Request.Chatting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ChatCase.Framework.Hubs
{
    [Authorize]
    public abstract class ChatGroupHub : Hub
    {
        protected readonly IChattingService _chattingService;

        protected ChatGroupHub(IChattingService chattingService)
        {
            _chattingService = chattingService;
        }

        public virtual async Task<Task> Send(ChatGroupMessageRequest messageRequest)
        {
            if (!await _chattingService.GroupExistsAsync(messageRequest.ChatGroupName))
                throw new System.Exception("cannot send a news item to a group which does not exist.");

            await _chattingService.CreateMessageAsync(messageRequest);
            return Clients.Group(messageRequest.ChatGroupName).SendAsync("Send", messageRequest);
        }

        public virtual async Task JoinGroup(string groupName)
        {
            if (!await _chattingService.GroupExistsAsync(groupName))
                throw new System.Exception("cannot join a group which does not exist.");

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("JoinGroup", groupName);

            var history = _chattingService.GetMessagesByGroupNameAsync(groupName);
            await Clients.Client(Context.ConnectionId).SendAsync("History", history);
        }

        public virtual async Task LeaveGroup(string groupName)
        {
            if (!await _chattingService.GroupExistsAsync(groupName))
                throw new System.Exception("cannot leave a group which does not exist.");

            await Clients.Group(groupName).SendAsync("LeaveGroup", groupName);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
