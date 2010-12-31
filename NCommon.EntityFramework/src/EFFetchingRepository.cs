using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NCommon.Specifications;

namespace NCommon.Data.EntityFramework
{
    public class EFFetchingRepository<TEntity, TReleated> : RepositoryWrapperBase<EFRepository<TEntity>, TEntity>, IEFFetchingRepository<TEntity, TReleated> where TEntity : class
    {
        readonly string _fetchingPath;

        public EFFetchingRepository(EFRepository<TEntity> repository, string fetchingPath) : base(repository)
        {
            _fetchingPath = fetchingPath;
        }

        public string FetchingPath
        {
            get { return _fetchingPath; }
        }
    }
}