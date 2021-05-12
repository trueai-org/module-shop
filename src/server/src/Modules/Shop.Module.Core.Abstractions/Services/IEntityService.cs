using Shop.Module.Core.Entities;
using Shop.Module.Core.Models;
using System.Threading.Tasks;

namespace Shop.Module.Core.Services
{
    public interface IEntityService
    {
        string ToSafeSlug(string slug, int entityId, EntityTypeWithId entityTypeId);

        Entity Get(int entityId, EntityTypeWithId entityTypeId);

        void Add(string name, string slug, int entityId, EntityTypeWithId entityTypeId);

        void Update(string newName, string newSlug, int entityId, EntityTypeWithId entityTypeId);

        Task Remove(int entityId, EntityTypeWithId entityTypeId);
    }
}
