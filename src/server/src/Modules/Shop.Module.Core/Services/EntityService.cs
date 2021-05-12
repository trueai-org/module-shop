using MediatR;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Events;
using Shop.Module.Core.Models;
using Shop.Module.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Core.Services
{
    public class EntityService : IEntityService
    {
        private readonly IRepository<Entity> _entityRepository;
        private readonly IMediator _mediator;

        public EntityService(IRepository<Entity> entityRepository, IMediator mediator)
        {
            _entityRepository = entityRepository;
            _mediator = mediator;
        }

        public string ToSafeSlug(string slug, int entityId, EntityTypeWithId entityTypeId)
        {
            if (string.IsNullOrWhiteSpace(slug))
                throw new ArgumentNullException(nameof(slug));

            var i = 2;
            while (true)
            {
                var entity = _entityRepository.Query().FirstOrDefault(x => x.Slug == slug);
                if (entity != null && !(entity.EntityId == entityId && entity.EntityTypeId == (int)entityTypeId))
                {
                    slug = string.Format("{0}-{1}", slug, i);
                    i++;
                }
                else
                {
                    break;
                }
            }
            return slug;
        }

        public Entity Get(int entityId, EntityTypeWithId entityTypeId)
        {
            return _entityRepository.Query().FirstOrDefault(x => x.EntityId == entityId && x.EntityTypeId == (int)entityTypeId);
        }

        public void Add(string name, string slug, int entityId, EntityTypeWithId entityTypeId)
        {
            var entity = new Entity
            {
                Name = name,
                Slug = slug,
                EntityId = entityId,
                EntityTypeId = (int)entityTypeId
            };
            _entityRepository.Add(entity);
        }

        public void Update(string newName, string newSlug, int entityId, EntityTypeWithId entityTypeId)
        {
            var entity = _entityRepository.Query().First(x => x.EntityId == entityId && x.EntityTypeId == (int)entityTypeId);
            entity.Name = newName;
            entity.Slug = newSlug;
        }

        public async Task Remove(int entityId, EntityTypeWithId entityTypeId)
        {
            var entity = _entityRepository.Query().FirstOrDefault(x => x.EntityId == entityId && x.EntityTypeId == (int)entityTypeId);
            if (entity != null)
            {
                await _mediator.Publish(new EntityDeleting { EntityId = entity.Id });
                _entityRepository.Remove(entity);
            }
        }
    }
}
