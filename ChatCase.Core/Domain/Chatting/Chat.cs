using ChatCase.Core.Domain.Identity;
using ChatCase.Core.Entities;

namespace ChatCase.Core.Domain.Chatting
{
    public class Chat : BaseEntity
    {
        public string Text { get; set; }
        public string SenderId { get; set; }
        public string ChatGroupId { get; set; }
        public virtual AppUser Sender { get; set; }
        public virtual ChatGroup ChatGroup { get; set; }
    }
}
