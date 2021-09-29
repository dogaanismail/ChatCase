namespace ChatCase.Domain.Dto.Request.Chatting
{
    public class ChatGroupMessageRequest
    {
        public string Text { get; set; }
        public string ChatGroupId { get; set; }
        public string ChatGroupName { get; set; }
        public string SenderId { get; set; }
    }
}
