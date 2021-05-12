using Shop.Infrastructure.Models;
using Shop.Module.Core.Models;
using System;

namespace Shop.Module.Core.Entities
{
    public class UserAddress : EntityBase
    {
        public int UserId { get; set; }

        public User User { get; set; }

        public int AddressId { get; set; }

        public Address Address { get; set; }

        public AddressType AddressType { get; set; }

        public DateTime? LastUsedOn { get; set; }

        public bool IsDeleted { get; set; }
    }
}
