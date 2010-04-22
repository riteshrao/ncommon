using NCommon.Data;
using NCommon.State;
using Rhino.Mocks;
using Microsoft.Practices.ServiceLocation;

namespace NCommon.Testing
{
    /// <summary>
    /// Configures NCommon for unit tests.
    /// </summary>
    public static class Configure
    {
        ///<summary>
        /// Configures NCommon using the specified mocked <see cref="IServiceLocator"/> instance.
        ///</summary>
        ///<param name="mockLocator">The <see cref="IServiceLocator"/> instance.</param>
        public static void Using(IServiceLocator mockLocator)
        {
            mockLocator.Stub(x => x.GetInstance<IState>()).Return(new FakeState());
            UnitOfWorkManager.SetTransactionManagerProvider(() => MockRepository.GenerateStub<ITransactionManager>());
        }
    }
}
