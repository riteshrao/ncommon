namespace NCommon.NHibernate.Tests.Domain
{
    public class Money
    {
        #region properties
        public decimal Amount { get; set; }
        public string Currency { get; set; } 
        #endregion

        #region methods
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == typeof(Money) && this.Equals((Money)obj);
        }

        public virtual bool Equals(Money obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj.Amount == this.Amount && Equals(obj.Currency, this.Currency);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Amount.GetHashCode() * 397) ^ (this.Currency != null ? this.Currency.GetHashCode() : 0);
            }
        } 
        #endregion
    }
}
