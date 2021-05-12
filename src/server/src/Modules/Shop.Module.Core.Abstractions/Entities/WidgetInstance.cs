using Shop.Infrastructure.Models;
using System;

namespace Shop.Module.Core.Entities
{
    public class WidgetInstance : EntityBase
    {
        public WidgetInstance()
        {
            CreatedOn = DateTime.Now;
            UpdatedOn = DateTime.Now;
        }

        public string Name { get; set; }

        public DateTime? PublishStart { get; set; }

        public DateTime? PublishEnd { get; set; }

        public int WidgetId { get; set; }

        public Widget Widget { get; set; }

        public int WidgetZoneId { get; set; }

        public WidgetZone WidgetZone { get; set; }

        public int DisplayOrder { get; set; }

        public string Data { get; set; }

        public string HtmlData { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        /// <summary>
        /// This property cannot be used to filter again DB because it don't exist in database
        /// </summary>
        public bool IsPublished
        {
            get
            {
                return PublishStart.HasValue && PublishStart.Value < DateTime.Now && (!PublishEnd.HasValue || PublishEnd.Value > DateTime.Now);
            }
        }
    }
}
