using System;
using System.Collections.Generic;
using NCommon.Extensions;

namespace NCommon.Data.EntityFramework.Tests
{
    public class EFDataGenerator : IDisposable
    {
        readonly TestModel _context;
        readonly IList<Action<TestModel>> _entityDeleteActions;

        public EFDataGenerator(TestModel context)
        {
            _context = context;
            _entityDeleteActions = new List<Action<TestModel>>();
        }

        public TestModel Context
        {
            get { return _context; }
        }

        public IList<Action<TestModel>> EntityDeleteActions
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