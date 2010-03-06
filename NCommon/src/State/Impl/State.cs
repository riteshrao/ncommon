using System;

namespace NCommon.State.Impl
{
    public class State : IState
    {
        readonly IApplicationState _applicationState;
        readonly ILocalState _localState;
        readonly ISessionState _sessionState;
        readonly ICacheState _cacheState;

        public State(
            IApplicationState applicationState, 
            ILocalState localState, 
            ISessionState sessionState, 
            ICacheState cacheState)
        {
            _applicationState = applicationState;
            _localState = localState;
            _sessionState = sessionState;
            _cacheState = cacheState;
        }

        public IApplicationState Application
        {
            get { return _applicationState; }
        }

        public ILocalState Local
        {
            get { return _localState; }
        }

        public ISessionState Session
        {
            get { return _sessionState; }
        }

        public ICacheState Cache
        {
            get { return _cacheState; }
        }
    }
}