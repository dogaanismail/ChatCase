using ChatCase.Core.Entities;
using System.Collections.Generic;

namespace ChatCase.Core.Domain.Chatting
{
    public class ChatGroup : BaseEntity
    {
        public ChatGroup()
        {
            Chats = new HashSet<Chat>();
            ChatGroupMembers = new HashSet<ChatGroupUser>();
        }

        public string Name { get; set; }
        public string GroupFlag { get; set; }
        public virtual ICollection<Chat> Chats { get; set; }
        public virtual ICollection<ChatGroupUser> ChatGroupMembers { get; set; }
    }
}
