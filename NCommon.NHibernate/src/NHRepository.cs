#region license
//Copyright 2008 Ritesh Rao 

//Licensed under the Apache License, Version 2.0 (the "License"); 
//you may not use this file except in compliance with the License. 
//You may obtain a copy of the License at 

//http://www.apache.org/licenses/LICENSE-2.0 

//Unless required by applicable law or agreed to in writing, software 
//distributed under the License is distributed on an "AS IS" BASIS, 
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//See the License for the specific language governing permissions and 
//limitations under the License. 
#endregion

using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using NCommon.Extensions;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Transform;

namespace NCommon.Data.NHibernate
{
    /// <summary>
    /// Inherits from the <see cref="RepositoryBase{TEntity}"/> class to provide an implementation of a
    /// repository that uses NHibernate.
    /// </summary>
    public class NHRepository<TEntity> : RepositoryBase<TEntity>
    {
        //int _batchSize = -1;
        //bool _enableCached;
        //string _cachedQueryName;
        ISession _privateSession;

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="NHRepository{TEntity}"/> class.
        /// </summary>
        public NHRepository ()
        {
            Initialize();           
        }
        
         /// <summary>
        /// Default Init.
        /// </summary>
        protected virtual void Initialize()
        {
            if (ServiceLocator.Current == null)
                return;

            var sessions = ServiceLocator.Current.GetAllInstances<ISession>();
            if (sessions != null && sessions.Count() > 0)
                _privateSession = sessions.FirstOrDefault();
        }         

        /// <summary>
        /// Gets the <see cref="ISession"/> instnace that is used by the repository.
        /// </summary>
        private ISession Session
        {
            get
            {
                return _privateSession ?? UnitOfWork<NHUnitOfWork>().GetSession<TEntity>();
            }
        }

        /// <summary>
        /// Gets the <see cref="IQueryable{TEntity}"/> used by the <see cref="RepositoryBase{TEntity}"/> 
        /// to execute Linq queries.
        /// </summary>
        /// <value>A <see cref="IQueryable{TEntity}"/> instance.</value>
        /// <remarks>
        /// Inheritors of this base class should return a valid non-null <see cref="IQueryable{TEntity}"/> instance.
        /// </remarks>
        protected override IQueryable<TEntity> RepositoryQuery
        {
            get
            {
                return Session.Query<TEntity>();
            }
        }

        /// <summary>
        /// Adds a transient instance of <see cref="TEntity"/> to be tracked
        /// and persisted by the repository.
        /// </summary>
        /// <param name="entity"></param>
        /// <remarks>
        /// The Add method replaces the existing <see cref="RepositoryBase{TEntity}.Save"/> method, which will
        /// eventually be removed from the public API.
        /// </remarks>
        public override void Add(TEntity entity)
        {
            Session.SaveOrUpdate(entity);
        }

        /// <summary>
        /// Marks the entity instance to be deleted from the store.
        /// </summary>
        /// <param name="entity">An instance of <typeparamref name="TEntity"/> that should be deleted.</param>
        public override void Delete(TEntity entity)
        {
            Session.Delete(entity);
        }

        /// <summary>
        /// Detaches a instance from the repository.
        /// </summary>
        /// <param name="entity">The entity instance, currently being tracked via the repository, to detach.</param>
        public override void Detach(TEntity entity)
        {
            Session.Evict(entity);
        }

        /// <summary>
        /// Attaches a detached entity, previously detached via the <see cref="IRepository{TEntity}.Detach"/> method.
        /// </summary>
        /// <param name="entity">The entity instance to attach back to the repository.</param>
        public override void Attach(TEntity entity)
        {
            Session.Update(entity);
        }

        /// <summary>
        /// Refreshes a entity instance.
        /// </summary>
        /// <param name="entity">The entity to refresh.</param>
        public override void Refresh(TEntity entity)
        {
            Session.Refresh(entity, LockMode.None);
        }
    }
}
