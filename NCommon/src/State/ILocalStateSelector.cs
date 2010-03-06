using System.Collections;

namespace NCommon.State
{
    public interface ILocalStateSelector
    {
        ILocalState Get();
    }
}