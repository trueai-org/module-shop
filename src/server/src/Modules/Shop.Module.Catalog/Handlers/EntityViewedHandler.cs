using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Data;
using Shop.Module.Catalog.Entities;
using Shop.Module.Core.Events;
using Shop.Module.Core.Extensions;
using Shop.Module.Core.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shop.Module.Catalog.Handlers
{
    public class EntityViewedHandler : INotificationHandler<EntityViewed>
    {
        private readonly IRepository<ProductRecentlyViewed> _recentlyViewedProductRepository;
        private readonly IWorkContext _workContext;

        public EntityViewedHandler(
            IRepository<ProductRecentlyViewed> recentlyViewedProductRepository,
            IWorkContext workcontext)
        {
            _recentlyViewedProductRepository = recentlyViewedProductRepository;
            _workContext = workcontext;
        }

        public async Task Handle(EntityViewed notification, CancellationToken cancellationToken)
        {
            if (notification.EntityTypeWithId == EntityTypeWithId.Product)
            {
                var model = await _recentlyViewedProductRepository.Query()
                    .FirstOrDefaultAsync(x => x.ProductId == notification.EntityId && x.CustomerId == notification.UserId);

                if (model == null)
                {
                    model = new ProductRecentlyViewed
                    {
                        CustomerId = notification.UserId,
                        ProductId = notification.EntityId,
                    };
                    _recentlyViewedProductRepository.Add(model);
                }
                model.ViewedCount++;
                model.LatestViewedOn = DateTime.Now;
                _recentlyViewedProductRepository.SaveChanges();
            }
        }
    }
}
