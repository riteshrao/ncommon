using System.Collections;

namespace NCommon.State
{
    public interface ISessionStateSelector
    {
        ISessionState Get();
    }
}