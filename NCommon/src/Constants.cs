using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace NCommon
{
    public static class Constants
    {
        public static CultureInfo CurrentCulture
        {
            get
            {
                return CultureInfo.CurrentCulture;
            }
        }

        public const int IdOfUnsavedDomainObject = 0;
        public const int IdOfStubDomainObject = -999999999;  // used as the Id of Stub objects, should be renamed to IdOfStubObject
        public const int IndexOfStubDomainObject = -1000;  // a special Id to indicate a "Not-A-Domain-Object" (e.g., 

        // artificial object types such as: ObjectDescriptor, ObjectAndTypeElement, None-Entry Item (for combobox)
        public const int InvalidVersion = -1;

        public static readonly int IndexOfFirstNonStubObjectInDataList = 1;

        public static readonly char DefaultDelimiter = ',';
        public static readonly int DefaultPageSize = 10;
        public static readonly string DefaultSingletonPropertyName = "Singleton";
        public static readonly string DefaultVersionPropertyName = "ObjectVersion";
    }
}


