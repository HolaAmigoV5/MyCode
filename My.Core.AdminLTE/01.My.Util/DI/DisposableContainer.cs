using System;
using System.Collections.Generic;
using System.Text;

namespace My.Util.DI
{
    /// <summary>
    /// 描述：资源释放
    /// 作者：wby 2019/9/20 9:48:49
    /// </summary>
    public class DisposableContainer : IDisposableContainer
    {
        SynchronizedCollection<IDisposable> _objList = new SynchronizedCollection<IDisposable>();

        public void AddDisposableObj(IDisposable disposableObj)
        {
            if (!_objList.Contains(disposableObj))
                _objList.Add(disposableObj);
        }

        public void Dispose()
        {
            _objList.ForEach(x =>
            {
                try
                {
                    x.Dispose();
                }
                catch (Exception ex)
                {
                    throw ex.InnerException;
                }
            });

            _objList.Dispose();
            _objList = null;
        }
    }
}
