using NCommon.Data.NHibernate.Tests.OrdersDomain;

namespace NCommon.Data.NHibernate.Tests
{
    public class OrderItemsProductStrategy : IFetchingStrategy<Order, NHRepositoryTests>
    {
        ///<summary>
        /// Instructs the instance to define the fetching strategy on the repository instance.
        ///</summary>
        ///<param name="repository"></param>
        public void Define(IRepository<Order> repository)
        {
            repository.With<OrderItem>(x => x.Product);
        }
    }
}