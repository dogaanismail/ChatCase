using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using System;

namespace ChatCase.Core.Domain.Identity
{
    [CollectionName("appuser")]
    public class AppUser : MongoIdentityUser<string>
    {
        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonElement(Order = 101)]
        public DateTime RegisteredDate { get; set; }
    }
}
