namespace NCommon.NHibernate.Tests.Domain
{
    public class MoneyUserType : CompositeUserTypeBase<Money>
    {
        #region .ctor
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public MoneyUserType()
        {
            MapProperty(prop => prop.Currency);
            MapProperty(prop => prop.Amount);
        }
        #endregion

        
        #region Overrides of CompositeUserTypeBase<Money>
        /// <summary>
        /// Inherits must build up the underlying type and return it.
        /// </summary>
        /// <param name="propertyValues">An array of objects that contain the values retrieved from the database.</param>
        /// <returns></returns>
        protected override Money CreateInstance(object[] propertyValues)
        {
            return new Money()
            {
                Currency = propertyValues[0].ToString(),
                Amount = (decimal)propertyValues[1]
            };
        }

        /// <summary>
        /// Performs a deep copy of a source entity.
        /// </summary>
        /// <param name="source">The source entity whose deep copy should be returned.</param>
        /// <returns>T</returns>
        /// <remarks>
        /// Inheritors must return a cloned or deep copied instance of the provided entity. If 
        /// </remarks>
        protected override Money PerformDeepCopy(Money source)
        {
            return source == null ? null : new Money
            {
                Currency = source.Currency,
                Amount = source.Amount
            };
        }

        /// <summary>
        /// Are objects of this type mutable?
        /// </summary>
        public override bool IsMutable
        {
            get { return true; }
        }
        #endregion
    }
}