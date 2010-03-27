using System;
using System.Collections.Generic;
using System.Data.Objects;
using NCommon.Extensions;

namespace NCommon.Data.EntityFramework.Tests
{
    public class EFTestData : IDisposable
    {
        readonly ObjectContext _context;
        readonly IList<Action<ObjectContext>> _entityDeleteActions;

        public EFTestData(ObjectContext context)
        {
            _context = context;
            _entityDeleteActions = new List<Action<ObjectContext>>();
        }

        public T Context<T>() where T : ObjectContext
        {
            return (T) _context;
        }

        public IList<Action<ObjectContext>> EntityDeleteActions
        {
            get { return _entityDeleteActions; }
        }

        public void Batch(Action<EFTestDataActions> action)
        {
            var dataActions = new EFTestDataActions(this);
            action(dataActions);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            if (_entityDeleteActions.Count <= 0) 
                return;

            _entityDeleteActions.ForEach(x => x(_context));
            _context.SaveChanges();
            _context.Dispose();
        }
    }
}