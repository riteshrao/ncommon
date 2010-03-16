using NCommon.Data.EntityFramework.Tests.OrdersDomain;

namespace NCommon.Data.EntityFramework.Tests
{
    public class OrderItemsProductStrategy : IFetchingStrategy<Order, EFRepositoryTests>
    {
        #region Implementation of IFetchingStrategy<Order,NHUnitOfWorkTests>
        ///<summary>
        /// Instructs the instance to define the fetching strategy on the repository instance.
        ///</summary>
        ///<param name="repository"></param>
        public void Define(IRepository<Order> repository)
        {
            repository.With<OrderItem>(x => x.Product);
        }
        #endregion
    }
}