using System;
using System.Windows.Media.Media3D;

namespace WPF3DDemo
{
    internal static class Point3DCollectionExtension
    {
        public static Point3DCollection AppendPoint3DCollection(this Point3DCollection point3Ds, Point3DCollection addPoint3ds)
        {
            if (point3Ds == null)
                throw new ArgumentNullException("point3Ds不能为空");

            foreach (var item in addPoint3ds)
            {
                point3Ds.Add(item);
            }
            return point3Ds;
        }
    }
}
