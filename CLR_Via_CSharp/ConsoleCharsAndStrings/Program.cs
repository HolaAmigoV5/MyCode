using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleCharsAndStrings
{
    public class Program
    {
        static void Main(string[] args)
        {
            CustomFormatter.Go();
            Console.ReadLine();
        }
    }

    internal static class CustomFormatter
    {
        public static void Go()
        {
            //StringBuilder sb = new StringBuilder();
            //sb.AppendFormat(new BoldInt32s(), "{0} {1} {2:M}", "Jeff", 123, DateTime.Now);
            //Console.WriteLine(sb);

            Timer t = new Timer(obj => { Console.WriteLine($"the time is {DateTime.Now}"); GC.Collect(); }, null, 0, 2000);
        }

        private sealed class BoldInt32s : IFormatProvider, ICustomFormatter
        {
            public string Format(string format, object arg, IFormatProvider formatProvider)
            {
                string s;
                IFormattable formattable = arg as IFormattable;
                if (formattable == null) s = arg.ToString();
                else s = formattable.ToString(format, formatProvider);
                if (arg.GetType() == typeof(int))
                    return "<B>" + s + "</B>";
                return s;
            }

            public object GetFormat(Type formatType)
            {
                if (formatType == typeof(ICustomFormatter)) return this;
                return Thread.CurrentThread.CurrentCulture.GetFormat(formatType);
            }
        }
    }
}
