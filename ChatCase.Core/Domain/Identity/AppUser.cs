using AspNetCore.Identity.Mongo.Model;
using MongoDbGenericRepository.Attributes;

namespace ChatCase.Core.Domain.Identity
{
    [CollectionName("appuser")]
    public class AppUser : MongoUser<string>
    {
    }
}
