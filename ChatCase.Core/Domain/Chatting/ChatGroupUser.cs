using ChatCase.Core.Domain.Identity;
using ChatCase.Core.Entities;

namespace ChatCase.Core.Domain.Chatting
{
    public class ChatGroupUser : BaseEntity
    {
        public string ChatGroupId { get; set; }
        public string MemberId { get; set; }
    }
}
