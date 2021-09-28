using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace ChatCase.Core.Domain.Identity
{
    [CollectionName("approle")]
    public class AppRole : MongoIdentityRole<string>
    {
    }
}
