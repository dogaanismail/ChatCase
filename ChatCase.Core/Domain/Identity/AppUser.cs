using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace ChatCase.Core.Domain.Identity
{
    [CollectionName("appuser")]
    public class AppUser : MongoIdentityUser<string>
    {
    }
}
