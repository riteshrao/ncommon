using System;
using System.Data;

namespace NCommon.Data.LinqToSql
{
    public class LinqToSqlUnitOfWorkSettings
    {
        public IsolationLevel DefaultIsolation { get; set; }
        public ILinqToSqlSessionResolver SessionResolver { get; set; }
    }
}