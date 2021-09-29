using System.Collections.Generic;

namespace ChatCase.Domain.Dto.Request.Chatting
{
    public class ChatGroupCreateRequest
    {
        public string Name { get; set; }
        public string GroupFlag { get; set; }
        public List<string> AppUserIds { get; set; }
    }
}
