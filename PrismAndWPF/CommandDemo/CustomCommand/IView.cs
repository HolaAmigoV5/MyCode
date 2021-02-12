using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandDemo.CustomCommand
{
    public interface IView
    {
        //属性
        bool IsChanged { get; set; }

        //方法
        void SetBinding();
        void Refresh();
        void Clear();
        void Save();
    }
}
