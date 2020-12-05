using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting;
using System.Threading;

namespace ConsoleAppDomains
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Marshling();
            AppDomainResourceMonitoring();
            Console.ReadLine();
        }

        private static void Marshling()
        {
            //获取AppDomain引用
            AppDomain adCallingThreadDomain = Thread.GetDomain();

            string callingDomainName = adCallingThreadDomain.FriendlyName;
            Console.WriteLine("Default AppDomain's friendly name={0}", callingDomainName);

            //获取并显示我们的AppDomain中包含的“Main”方法的程序集
            string exeAssembly = Assembly.GetEntryAssembly().FullName;
            Console.WriteLine("Main assembly={0}", exeAssembly);

            //***DEMO 1：使用按引用封送进行跨AppDomain通信***
            Console.WriteLine("{0} Demo #1", Environment.NewLine);

            //定义局部变量来引用一个AppDomain
            AppDomain ad2 = AppDomain.CreateDomain("AD #2", null, null);

            //将我们的程序集加载到新AppDomain中，构造一个对象，把它封送回我们的AppDomain（实际得到的是一个代理的引用）
            MarshalByRefType mbrt = (MarshalByRefType)ad2.CreateInstanceAndUnwrap(exeAssembly, "ConsoleAppDomains.MarshalByRefType");
            Console.WriteLine("Type={0}", mbrt.GetType());

            //证明得到的是一个代理对象的引用
            Console.WriteLine("Is proxy={0}", RemotingServices.IsTransparentProxy(mbrt));

            //代理类型上调用一个方法，代理使线程切换到拥有对象的那个AppDomain，并在真实的对象上调用这个方法。
            mbrt.SomeMethod();

            //卸载新的AppDomain
            AppDomain.Unload(ad2);

            try
            {
                //在代理上调用一个方法。AppDomain无效，造成抛出异常
                mbrt.SomeMethod();
                Console.WriteLine("Successful call.");
            }
            catch (AppDomainUnloadedException)
            {
                Console.WriteLine("Failed call.");
            }

            //***DEMO 2：使用Marshal-by-Value进行跨AppDomain通信***
            Console.WriteLine("{0} Demo #2", Environment.NewLine);

            //新建一个AppDomain
            ad2 = AppDomain.CreateDomain("AD #2", null, null);
            mbrt = (MarshalByRefType)ad2.CreateInstanceAndUnwrap(exeAssembly, "ConsoleAppDomains.MarshalByRefType");
            MarshalByValType mbvt = mbrt.MethodWithReturn();
            Console.WriteLine("Is proxy={0}", RemotingServices.IsTransparentProxy(mbvt));

            Console.WriteLine("Returned object created " + mbvt.ToString());

            AppDomain.Unload(ad2);

            try
            {
                Console.WriteLine("Returned object created " + mbvt.ToString());
                Console.WriteLine("Success call.");
            }
            catch (AppDomainUnloadedException)
            {
                Console.WriteLine("Failed call.");
            }


            //***DEMO 3:使用不可封送的类型进行跨AppDomain通信**
            Console.WriteLine("{0} Demo #3", Environment.NewLine);

            //新建一个AppDomain
            ad2 = AppDomain.CreateDomain("AD #2", null, null);
            mbrt = (MarshalByRefType)ad2.CreateInstanceAndUnwrap(exeAssembly, "ConsoleAppDomains.MarshalByRefType");

            NoMarshalableType nmt = mbrt.MethodArgAndReturn(callingDomainName);

        }

        private static void AppDomainResourceMonitoring()
        {
            using (new AppDomainMonitorDelta(null))
            {
                //分配在回收时能存活的约10MB
                var list = new List<object>();
                for (int i = 0; i < 1000; i++)
                {
                    list.Add(new Byte[10000]);
                }

                //分配在回收时存活不了的约20MB
                for (int i = 0; i < 2000; i++)
                {
                    new Byte[10000].GetType();
                }

                //保持CPU工作约5秒
                long stop = Environment.TickCount + 5000;
                while (Environment.TickCount < stop) ;
            }
        }
    }

    #region 跨AppDomain封送
    //该类的实例可跨越AppDomain的边界“按引用封送”
    public sealed class MarshalByRefType : MarshalByRefObject
    {
        public MarshalByRefType()
        {
            Console.WriteLine("{0} ctor running in {1}", this.GetType().ToString(), Thread.GetDomain().FriendlyName);
        }

        public void SomeMethod()
        {
            Console.WriteLine("Executing in " + Thread.GetDomain().FriendlyName);
        }

        public MarshalByValType MethodWithReturn()
        {
            Console.WriteLine("Executing in " + Thread.GetDomain().FriendlyName);
            MarshalByValType t = new MarshalByValType();
            return t;
        }

        public NoMarshalableType MethodArgAndReturn(string callingDomainName)
        {
            Console.WriteLine("Calling from '{0}' to '{1}'.", callingDomainName, Thread.GetDomain().FriendlyName);
            NoMarshalableType t = new NoMarshalableType();
            return t;
        }
    }

    [Serializable]
    public sealed class MarshalByValType : object
    {
        private readonly DateTime m_creationTime = DateTime.Now;  //注意：DateTime是可序列化的

        public MarshalByValType()
        {
            Console.WriteLine("{0} ctor running in {1}, Created on {2:D}",
                this.GetType().ToString(), Thread.GetDomain().FriendlyName, m_creationTime);
        }

        public override string ToString()
        {
            return m_creationTime.ToLongDateString();
        }
    }

    //该类的实例不能跨AppDomain边界进行封送
    [Serializable]
    public sealed class NoMarshalableType : object
    {
        public NoMarshalableType()
        {
            Console.WriteLine("Executing in " + Thread.GetDomain().FriendlyName);
        }
    }
    #endregion

    public sealed class AppDomainMonitorDelta : IDisposable
    {
        private AppDomain m_appDomain;
        private TimeSpan m_thisADCpu;
        private long m_thisADMemoryInUse;
        private long m_thisADMemoryAllocated;


        static AppDomainMonitorDelta()
        {
            //确定打开了AppDomain监视
            AppDomain.MonitoringIsEnabled = true;
        }

        public AppDomainMonitorDelta(AppDomain ad)
        {
            m_appDomain = ad ?? AppDomain.CurrentDomain;
            m_thisADCpu = m_appDomain.MonitoringTotalProcessorTime;
            m_thisADMemoryInUse = m_appDomain.MonitoringSurvivedMemorySize;
            m_thisADMemoryAllocated = m_appDomain.MonitoringTotalAllocatedMemorySize;
        }

        public void Dispose()
        {
            GC.Collect();
            Console.WriteLine("FriendlyName = {0}, CPU = {1}ms", m_appDomain.FriendlyName,
                (m_appDomain.MonitoringTotalProcessorTime - m_thisADCpu).TotalMilliseconds);

            Console.WriteLine(" Allocated {0:N0} bytes of which {1:N0} survived GCs",
                m_appDomain.MonitoringTotalAllocatedMemorySize - m_thisADMemoryAllocated,
                m_appDomain.MonitoringSurvivedMemorySize - m_thisADMemoryInUse);
        }
    }
}
