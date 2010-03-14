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

using NCommon.Context;

namespace NCommon.State.Impl
{
    /// <summary>
    /// Default implementation of <see cref="ILocalStateSelector"/>.
    /// </summary>
    public class DefaultLocalStateSelector : ILocalStateSelector
    {
        readonly IContext _context;

        /// <summary>
        /// Default Constructor.
        /// Creates an instance of <see cref="DefaultLocalStateSelector"/> class.
        /// </summary>
        /// <param name="context">An instance of <see cref="IContext"/>.</param>
        public DefaultLocalStateSelector(IContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the <see cref="ILocalState"/> instance to use.
        /// </summary>
        /// <returns></returns>
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