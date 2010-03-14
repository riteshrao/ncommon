using System;
using System.Data;

namespace NCommon.Data.LinqToSql
{
    /// <summary>
    /// Contains settings for Linq To Sql <see cref="IUnitOfWork"/> instances.
    /// </summary>
    public class LinqToSqlUnitOfWorkSettings
    {
        public IsolationLevel DefaultIsolation { get; set; }
        public ILinqToSqlSessionResolver SessionResolver { get; set; }
    }
}