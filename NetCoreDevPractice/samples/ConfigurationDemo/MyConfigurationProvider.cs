using Microsoft.Extensions.Configuration;
using System;
using System.Timers;

namespace ConfigurationDemo
{
    class MyConfigurationProvider:ConfigurationProvider
    {
        Timer timer;
        public MyConfigurationProvider():base()
        {
            timer = new Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 3000;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Load(true);
        }

        public override void Load()
        {
            //加载数据
            Load(false);
        }

        void Load(bool reload)
        {
            this.Data["lastTime"] = DateTime.Now.ToString();
            if (reload)
            {
                base.OnReload();
            }
        }
    }
}
