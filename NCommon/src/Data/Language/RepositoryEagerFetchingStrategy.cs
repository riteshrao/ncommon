#region license
//Copyright 2010 Ritesh Rao 

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NCommon.Data.Language
{
    /// <summary>
    /// Defines the root interface to specify eager fetching strategy for a <see cref="IRepository{TEntity}"/>
    /// </summary>
    /// <typeparam name="T">The entity for eager fetching strategy.</typeparam>
    public class RepositoryEagerFetchingStrategy<T>
    {
        IList<Expression> _paths = new List<Expression>();

        ///<summary>
        /// An array of <see cref="Expression"/> containing the eager fetching paths.
        ///</summary>
        public IEnumerable<Expression> Paths
        {
            get { return _paths.ToArray();}
        }

        ///<summary>
        /// Specify the path to eagerly fetch.
        ///</summary>
        ///<param name="path"></param>
        ///<typeparam name="TChild"></typeparam>
        ///<returns></returns>
        public EagerFetchingPath<TChild> Fetch<TChild>(Expression<Func<T, object>> path)
        {
            _paths.Add(path);
            return new EagerFetchingPath<TChild>(_paths);
        }
    }
}