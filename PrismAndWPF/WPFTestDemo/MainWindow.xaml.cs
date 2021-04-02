using System;
using System.Collections.Generic;
using System.Windows;

namespace WPFTestDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btn_click(object sender, RoutedEventArgs e)
        {
            //List<int> list = new List<int>(100);
            //for (int i = 1; i <= 100; i++)
            //{
            //    list.Add(i);
            //}

            //var num = GetNum(list, 4);
            //MessageBox.Show(num.ToString());

            var b = new B();
        }

        private int GetNum(List<int> list, int num)
        {
            int maxIndex = list.Count - 1;
            int currentIndex = 0;

            while (list.Count>1)
            {
                for(int i=1; i<=4; i++)
                {
                    if (currentIndex > maxIndex)
                        currentIndex = 0;
                    if (i == 4)
                    {
                        list.RemoveAt(currentIndex);
                        maxIndex--;
                    }
                    else
                        currentIndex++;
                }
            }
            return list[0];
        }
        
    }

    class A
    {
        public A()
        {
            PrintFields();
        }
        public virtual void PrintFields() { }
    }

    class B : A
    {
        int x = 1;
        int y;
        public B() { y = 21; }
        public override void PrintFields()
        {
            x++;
            y++;
            MessageBox.Show($"x={x}, y={y}");
        }
    }
}
