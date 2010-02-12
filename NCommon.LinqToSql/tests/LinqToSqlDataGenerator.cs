using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Transactions;
using NCommon.Extensions;

namespace NCommon.LinqToSql.Tests
{
    public class LinqToSqlDataGenerator : IDisposable
    {
        readonly TestDataDataContext _context;
        readonly IList<Action<TestDataDataContext>> _entityDeleteActions;

        public LinqToSqlDataGenerator(TestDataDataContext context)
        {
            _context = context;
            _entityDeleteActions = new List<Action<TestDataDataContext>>();
        }

        public TestDataDataContext Context
        {
            get { return _context; }
        }

        public IList<Action<TestDataDataContext>> EntityDeleteActions
        {
            get { return _entityDeleteActions; }
        }

        public void Batch(Action<LinqToSqlDataActions> action)
        {
            var dataActions = new LinqToSqlDataActions(this);
            action(dataActions);
            Context.SubmitChanges();
        }

        public void Dispose()
        {
            if (_entityDeleteActions.Count <= 0) 
                return;

            _entityDeleteActions.ForEach(x => x(_context));
            _context.SubmitChanges();
            _context.Dispose();
        }
    }
}