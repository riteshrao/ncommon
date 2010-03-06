namespace NCommon.NHibernate.Tests.Domain
{
    public class MonthlySalesSummary
    {
        public virtual int Year { get; set; }
        public virtual int Month { get; set; }
        public virtual int SalesPersonId { get; set; }
        public virtual string SalesPersonFirstName { get; set; }
        public virtual string SalesPersonLastName { get; set; }
        public virtual Money TotalSale { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;

            return obj.GetType() == typeof (MonthlySalesSummary) && this.Equals((MonthlySalesSummary) obj);
        }

        public virtual bool Equals(MonthlySalesSummary obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj.Year == this.Year && obj.Month == this.Month && obj.SalesPersonId == this.SalesPersonId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = this.Year;
                result = (result*397) ^ this.Month;
                result = (result*397) ^ this.SalesPersonId;
                return result;
            }
        }
    }
}
