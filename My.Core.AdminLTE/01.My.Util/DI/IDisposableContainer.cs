using System;

namespace My.Util
{
    public interface IDisposableContainer : IDisposable
    {
        void AddDisposableObj(IDisposable disposableObj);
    }
}
