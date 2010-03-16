using System;
using System.Collections.Generic;
using System.Data.Objects;
using NCommon.Extensions;

namespace NCommon.Data.EntityFramework.Tests
{
    public class EFDataGenerator : IDisposable
    {
        readonly ObjectContext _context;
        readonly IList<Action<ObjectContext>> _entityDeleteActions;

        public EFDataGenerator(ObjectContext context)
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

        public void Batch(Action<EFDataGeneratorActions> action)
        {
            var dataActions = new EFDataGeneratorActions(this);
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