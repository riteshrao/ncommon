using NCommon.LinqToSql.Tests;

namespace NCommon.Data.LinqToSql.Tests
{
    public class OrderItemsProductStrategy : IFetchingStrategy<Order, LinqToSqlRepositoryTests>
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