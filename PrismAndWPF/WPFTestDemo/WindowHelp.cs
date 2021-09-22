using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace WPFTestDemo
{
    public static class WindowHelp
    {
        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("User32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int oldStyle, int newStyle);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter,
                   int x, int y, int width, int height, uint flags);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hwnd, uint msg,
                   IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint colorKey, byte alpha, uint flags);


        const int GWL_STYLE = -16;
        const int GWL_EXSTYLE = -20;
        const int WS_EX_DLGMODALFRAME = 0x0001;
        const int SWP_NOSIZE = 0x0001;
        const int SWP_NOMOVE = 0x0002;
        const int SWP_NOZORDER = 0x0004;
        const int SWP_FRAMECHANGED = 0x0020;
        const uint WM_SETICON = 0x0080;

        const long WS_CAPTION = 0x00C00000L;

        const uint LWA_ALPHA = 0x00000002;
        const uint LWA_COLORKEY = 0x00000001;

        public static void RemoveIcon(Window window)
        {
            //获取窗体的句柄
            IntPtr hwnd = new WindowInteropHelper(window).Handle;

            //获取窗体样式
            long oldstyle = GetWindowLong(hwnd, GWL_STYLE);

            //更改窗体的样式为无边框窗体
            SetWindowLong(hwnd, GWL_STYLE, (int)(oldstyle & ~WS_CAPTION));

            //设置窗体为透明窗体
            SetLayeredWindowAttributes(hwnd, 0, 0, LWA_COLORKEY | LWA_ALPHA);

            //更新窗口的非客户区，以反映变化
            //SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE |
            //      SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
        }

    }
}
