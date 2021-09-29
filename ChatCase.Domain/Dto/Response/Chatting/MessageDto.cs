namespace ChatCase.Domain.Dto.Response.Chatting
{
    public class MessageDto
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string SenderId { get; set; }
        public string ChatGroupId { get; set; }
    }
}
