using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Transactions;
using NCommon.Extensions;

namespace NCommon.LinqToSql.Tests
{
    public class LinqToSqlTestData : IDisposable
    {
        readonly DataContext _context;
        readonly IList<Action<DataContext>> _entityDeleteActions;

        public LinqToSqlTestData(DataContext context)
        {
            _context = context;
            _entityDeleteActions = new List<Action<DataContext>>();
        }

        public T Context<T>() where T :DataContext
        {
            return (T) _context;
        }

        public IList<Action<DataContext>> EntityDeleteActions
        {
            get { return _entityDeleteActions; }
        }

        public void Batch(Action<LinqToSqlTestDataActions> action)
        {
            var dataActions = new LinqToSqlTestDataActions(this);
            action(dataActions);
            _context.SubmitChanges();
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