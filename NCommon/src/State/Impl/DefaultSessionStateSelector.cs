using System;
using NCommon.Context;

namespace NCommon.State.Impl
{
    public class DefaultSessionStateSelector : ISessionStateSelector
    {
        readonly IContext _context;

        public DefaultSessionStateSelector(IContext context)
        {
            _context = context;
        }

        public ISessionState Get()
        {
            if (_context.IsWcfApplication)
            {
                if (_context.IsAspNetCompatEnabled)
                    return new HttpSessionState(_context);
                return new WcfSessionState(_context);
            }
            if (_context.IsWebApplication)
                return new HttpSessionState(_context);
            return null;
        }
    }
}