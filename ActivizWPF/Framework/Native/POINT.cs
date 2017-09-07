using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;

namespace ActivizWPF.Framework.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public static implicit operator Point(POINT point)
        {
            return new Point(point.X, point.Y);
        }
    }

}