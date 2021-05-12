using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Data;
using Shop.Module.Feedbacks.Entities;

namespace Shop.Module.Feedbacks.Data
{
    public class FeedbackCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Feedback>().HasQueryFilter(c => !c.IsDeleted);
        }
    }
}
