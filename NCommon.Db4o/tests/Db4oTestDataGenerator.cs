using System;
using System.Collections.Generic;
using Db4objects.Db4o;
using NCommon.Extensions;

namespace NCommon.Data.Db4o.Tests
{
    public class Db4oTestDataGenerator : IDisposable
    {
        readonly IObjectContainer _container;
        readonly IList<object> _entitiesPersisted = new List<object>();

        public Db4oTestDataGenerator(IObjectContainer container)
        {
            _container = container;
        }

        public IObjectContainer Container
        {
            get { return _container; }
        }

        public IList<object >EntitiesPersisted
        {
            get { return _entitiesPersisted; }
        }

        public void Batch(Action<Db4oTestDataActions> actions)
        {
            try
            {
                var batchActions = new Db4oTestDataActions(this);
                actions(batchActions);
                _container.Commit();
            }
            catch
            {
                _container.Rollback();
                throw;
            }
        }

        public void Dispose()
        {
            if (_entitiesPersisted.Count <= 0) 
                return;

            _entitiesPersisted.ForEach(Container.Delete);
            Container.Commit();
            Container.Close();
            Container.Dispose();
        }
    }
}