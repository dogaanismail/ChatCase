using System;

namespace ChatCase.Domain.Dto.Response.Logging
{
    public class ActivityLogDto
    {
        public string Id { get; set; }

        public string EntityId { get; set; }

        public string EntityName { get; set; }

        public string AppUserId { get; set; }

        public string Comment { get; set; }

        public string IpAddress { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
