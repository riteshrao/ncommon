using System;
using NCommon.Context;

namespace NCommon.State.Impl
{
    public class DefaultLocalStateSelector : ILocalStateSelector
    {
        readonly IContext _context;

        public DefaultLocalStateSelector(IContext context)
        {
            _context = context;
        }

        public ILocalState Get()
        {
            if (_context.IsWcfApplication)
                return new WcfLocalState(_context);
            if (_context.IsWebApplication)
                return new HttpLocalState(_context);
            return new ThreadLocalState();
        }
    }
}