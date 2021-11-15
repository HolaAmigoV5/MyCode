using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WPF3DDemo
{
    internal static class StereoModels
    {
        public static Model3D DrawCircle(Vector3D circlePoint, Vector3D normalVec, double radius, Brush foreColor, Brush backColor)
        {
            List<Point3D> plist = GetCirclePlist(36, circlePoint, radius);
            List<int> nlist = GetCircleNlist(36);
            MeshGeometry3D mgd = new() { Positions = new Point3DCollection(plist), TriangleIndices = new Int32Collection(nlist) };
            GeometryModel3D gmd = new()
            {
                Geometry = mgd,
                Material = new DiffuseMaterial() { Brush = foreColor },
                BackMaterial = new DiffuseMaterial() { Brush = backColor }
            };
            Model3D res = gmd;
            return res;
        }

        static List<Point3D> GetCirclePlist(int n, Vector3D circlePoint, double radius)
        {
            double angle = 2 * Math.PI / n;
            List<Point3D> pl = new();
            pl.Add(new Point3D(circlePoint.X, circlePoint.Y, circlePoint.Z));
            for (int i = 0; i < n; i++)
            {
                pl.Add(new Point3D(circlePoint.X + radius * Math.Cos(i * angle), circlePoint.Y + radius * Math.Sin(i * angle), circlePoint.Z));
            }
            return pl;
        }

        static List<int> GetCircleNlist(int n)
        {
            List<int> pl = new();
            for (int i = 1; i < n; i++)
            {
                pl.Add(i);
                pl.Add(i + 1);
                pl.Add(0);

            }
            pl.Add(n);
            pl.Add(1);
            pl.Add(0);

            return pl;
        }
    }
}
