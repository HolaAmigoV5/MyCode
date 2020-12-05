using Microsoft.Extensions.FileProviders;
using System;

namespace FileProviderDemo
{
    public class Program
    {
        static void Main(string[] args)
        {
            //物理文件提供程序:读取物理文件
            IFileProvider provider1 = new PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory);
            //var contents = provider1.GetDirectoryContents("/"); //获取目录下所有内容

            //foreach (var item in contents)
            //{
            //    Console.WriteLine(item.Name);
            //}

            //嵌入式文件提供程序：读取嵌入式文件
            IFileProvider provider2 = new EmbeddedFileProvider(typeof(Program).Assembly);
            //var html = provider2.GetFileInfo("emb.html");

            //组合文件提供程序
            IFileProvider provider = new CompositeFileProvider(provider1, provider2);
            var contents2 = provider.GetDirectoryContents("/");
            foreach (var item in contents2)
            {
                Console.WriteLine(item.Name);
            }


            Console.ReadLine();
        }
    }
}
