using Moq;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Entities;
using System.Linq;
using Xunit;

namespace Shop.Module.Core.Tests
{
    public class StateOrProvincesTests
    {
        [Fact]
        public void Test1()
        {
            var res = MakeRepository();
            var list = res.Object.Query().ToList();

            //Assert.Equal(true);
        }

        private Mock<IRepository<StateOrProvince>> MakeRepository()
        {
            var pageRepository = new Mock<IRepository<StateOrProvince>>();
            //pageRepository.Setup(x => x.Query()).Returns();

            //pageRepository.Setup(x => x.Query()).Returns(new List<StateOrProvince>() { new StateOrProvince() { Name = "Page" } }.AsQueryable());
            return pageRepository;
        }

        private UserAddress MakeShippingAddress()
        {
            var country = new Country() { Name = "France" };
            var stateOrProvince = new StateOrProvince { Name = "IDF", Country = country };

            var address = new Address
            {
                CountryId = 1,
                AddressLine1 = "115 Rue Marcel",
                Country = country,
                StateOrProvince = stateOrProvince,
            };

            var userAddress = new UserAddress { UserId = 1, Address = address };

            return userAddress;
        }

        //private DefaultShippingAddressViewComponent MakeMockedDefaultAddressViewComponent(UserAddress address, User user)
        //{
        //    var companyProducts = new List<UserAddress> { address }.AsQueryable();

        //    var mockWorkContext = new Mock<IWorkContext>();
        //    mockWorkContext.Setup(x => x.GetCurrentUserAsync()).Returns(Task.FromResult(user));

        //    var mockSet = new Mock<DbSet<UserAddress>>();
        //    mockSet.As<IAsyncEnumerable<UserAddress>>()
        //        .Setup(m => m.GetEnumerator())
        //        .Returns(new TestAsyncEnumerator<UserAddress>(companyProducts.GetEnumerator()));

        //    mockSet.As<IQueryable<UserAddress>>()
        //        .Setup(m => m.Provider)
        //        .Returns(new TestAsyncQueryProvider<UserAddress>(companyProducts.Provider));

        //    mockSet.As<IQueryable<UserAddress>>().Setup(m => m.Expression).Returns(companyProducts.Expression);
        //    mockSet.As<IQueryable<UserAddress>>().Setup(m => m.ElementType).Returns(companyProducts.ElementType);
        //    mockSet.As<IQueryable<UserAddress>>().Setup(m => m.GetEnumerator()).Returns(() => companyProducts.GetEnumerator());

        //    var contextOptions = new DbContextOptions<SimplDbContext>();
        //    var mockContext = new Mock<SimplDbContext>(contextOptions);
        //    mockContext.Setup(c => c.Set<UserAddress>()).Returns(mockSet.Object);

        //    var repository = new Repository<UserAddress>(mockContext.Object);
        //    mockWorkContext.Setup(x => x.GetCurrentUser()).Returns(Task.FromResult(user));

        //    var component = new DefaultShippingAddressViewComponent(repository, mockWorkContext.Object);
        //    return component;
        //}
    }
}