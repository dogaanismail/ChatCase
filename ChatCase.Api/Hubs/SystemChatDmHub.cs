using ChatCase.Business.Interfaces.Chatting;
using ChatCase.Domain.Dto.Request.Chatting;
using ChatCase.Framework.Hubs;
using System.Threading.Tasks;

namespace ChatCase.Api.Hubs
{
    public class SystemChatDmHub : ChatGroupHub
    {
        public SystemChatDmHub(IChattingService chattingService) 
            : base(chattingService)
        {
        }

        public override Task JoinGroup(string groupName)
        {
            return base.JoinGroup(groupName);
        }

        public override Task LeaveGroup(string groupName)
        {
            return base.LeaveGroup(groupName);
        }

        public override Task<Task> Send(ChatGroupMessageRequest messageRequest)
        {
            return base.Send(messageRequest);
        }
    }
}
