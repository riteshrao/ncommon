using System;

namespace MvcStore.Models
{
    public abstract class Entity
    {
        Guid _id;
        int? _oldHashCode;

        public virtual Guid Id
        {
            get { return _id; }
            protected set { _id = value; }
        }

        public virtual bool IsTransient
        {
            get { return Equals(Id, default(Guid)); }
        }

        public override string ToString()
        {
            if (IsTransient)
                return string.Format("Transient instance of {0}", GetType().Name);
            return string.Format("{0} with Id of {1}", GetType().Name, Id);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Entity;
            if (other == null)
                return false;
            if (other.IsTransient && this.IsTransient)
                return ReferenceEquals(other, this);
            return other.Id.Equals(this.Id);
        }

        public override int GetHashCode()
        {
            if (_oldHashCode.HasValue)
                return _oldHashCode.Value;
            if (IsTransient)
                return (_oldHashCode = base.GetHashCode()).Value;
            return Id.GetHashCode();
        }

        public static bool operator ==(Entity x, Entity y)
        {
            return Equals(x, y);
        }

        public static bool operator !=(Entity x, Entity y)
        {
            return !(x == y);
        }
    }
}