using System.Windows;
using System.Windows.Controls;
using Microsoft.Toolkit.Mvvm.Input;

namespace Wby.Demo.PC.Template
{
    public class DataPagerModel : Control
    {
        /// <summary>
        /// 页大小
        /// </summary>
        public string PageSize
        {
            get { return (string)GetValue(PageSizeProperty); }
            set { SetValue(PageSizeProperty, value); }
        }

        
        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register("PageSize", typeof(string), typeof(DataPagerModel), new PropertyMetadata(""));

        /// <summary>
        /// 总数量
        /// </summary>

        public string TotalCount
        {
            get { return (string)GetValue(TotalCountProperty); }
            set { SetValue(TotalCountProperty, value); }
        }

       
        public static readonly DependencyProperty TotalCountProperty =
            DependencyProperty.Register("TotalCount", typeof(string), typeof(DataPagerModel), new PropertyMetadata(""));


        /// <summary>
        /// 页索引
        /// </summary>
        public string PageIndex
        {
            get { return (string)GetValue(PageIndexProperty); }
            set { SetValue(PageIndexProperty, value); }
        }

        
        public static readonly DependencyProperty PageIndexProperty =
            DependencyProperty.Register("PageIndex", typeof(string), typeof(DataPagerModel), new PropertyMetadata(""));

        /// <summary>
        /// 页总数
        /// </summary>
        public string PageCount
        {
            get { return (string)GetValue(PageCountProperty); }
            set { SetValue(PageCountProperty, value); }
        }

       
        public static readonly DependencyProperty PageCountProperty =
            DependencyProperty.Register("PageCount", typeof(string), typeof(DataPagerModel), new PropertyMetadata(""));


        public RelayCommand GoHomePageCommand
        {
            get { return (RelayCommand)GetValue(GoHomePageCommandProperty); }
            set { SetValue(GoHomePageCommandProperty, value); }
        }

        public static readonly DependencyProperty GoHomePageCommandProperty =
            DependencyProperty.Register("GoHomePageCommand", typeof(RelayCommand), typeof(DataPagerModel));



        public RelayCommand GoPrePageCommand
        {
            get { return (RelayCommand)GetValue(GoPrePageCommandProperty); }
            set { SetValue(GoPrePageCommandProperty, value); }
        }

        
        public static readonly DependencyProperty GoPrePageCommandProperty =
            DependencyProperty.Register("GoPrePageCommand", typeof(RelayCommand), typeof(DataPagerModel));


        public RelayCommand GoNextPageCommand
        {
            get { return (RelayCommand)GetValue(GoNextPageCommandProperty); }
            set { SetValue(GoNextPageCommandProperty, value); }
        }

        
        public static readonly DependencyProperty GoNextPageCommandProperty =
            DependencyProperty.Register("GoNextPageCommand", typeof(RelayCommand), typeof(DataPagerModel));


        public RelayCommand GoEndPageCommand
        {
            get { return (RelayCommand)GetValue(GoEndPageCommandProperty); }
            set { SetValue(GoEndPageCommandProperty, value); }
        }


        public static readonly DependencyProperty GoEndPageCommandProperty =
            DependencyProperty.Register("GoEndPageCommand", typeof(RelayCommand), typeof(DataPagerModel));


    }
}
