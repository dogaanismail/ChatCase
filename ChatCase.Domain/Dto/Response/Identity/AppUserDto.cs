using System;

namespace ChatCase.Domain.Dto.Response.Identity
{
    public class AppUserDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime RegisteredDate { get; set; }
    }
}
