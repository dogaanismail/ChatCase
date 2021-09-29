using ChatCase.Domain.Enumerations;
using System.Collections.Generic;

namespace ChatCase.Domain.Dto.Request.Chatting
{
    public class ChatGroupCreateRequest
    {
        public string Name { get; set; }
        public string GroupFlag { get; set; }
        public ChatGroupType ChatGroupType { get; set; }
        public List<string> AppUserIds { get; set; }
    }
}
