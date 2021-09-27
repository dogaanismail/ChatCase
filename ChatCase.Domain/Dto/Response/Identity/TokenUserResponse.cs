using System;

namespace ChatCase.Domain.Dto.Response.Identity
{
    public class TokenUserResponse
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime RegisteredDate { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public long Expires { get; set; }
    }
}
