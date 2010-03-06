using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace NCommon.Tests
{
    public class CrossThreadTests : IDisposable
    {
        Exception _lastException;
        readonly TimeSpan _timeout = TimeSpan.FromSeconds(5);
        readonly IList<WaitHandle> _waitHandles = new List<WaitHandle>();

        public CrossThreadTests() {}

        public CrossThreadTests(TimeSpan timeout)
        {
            _timeout = timeout;
        }

        public void Test(Action testDelegate)
        {
            var waitHandle = new AutoResetEvent(false);
            ThreadPool.QueueUserWorkItem(x =>
            {
                try
                {
                    testDelegate();
                }
                catch (Exception ex)
                {
                    _lastException = ex;
                }
                finally
                {
                    waitHandle.Set();
                }
            });
            _waitHandles.Add(waitHandle);
        }
        
        public void Dispose()
        {
            WaitHandle.WaitAll(_waitHandles.ToArray(), _timeout);
            if (_lastException != null)
            {
                //Preserving stack trace and throwing:
                var remoteStackTrace = typeof (Exception).GetField("_remoteStackTraceString",
                                                                   BindingFlags.Instance | BindingFlags.NonPublic);
                remoteStackTrace.SetValue(_lastException, _lastException.StackTrace + Environment.NewLine);
                throw _lastException;
            }
        }
    }
}