using AspNetCore.Identity.Mongo.Model;
using MongoDbGenericRepository.Attributes;

namespace ChatCase.Core.Domain.Identity
{
    [CollectionName("approle")]
    public class AppRole : MongoRole<string>
    {
    }
}
