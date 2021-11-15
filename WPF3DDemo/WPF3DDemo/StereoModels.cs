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

        /// <summary>
        /// 画圆锥
        /// </summary>
        /// <param name="height"></param>
        /// <param name="circlePoint"></param>
        /// <param name="normalVec"></param>
        /// <param name="radius"></param>
        /// <param name="faceColor"></param>
        /// <param name="backColor"></param>
        /// <returns></returns>
        public static Model3D DrawCone(double height, Vector3D circlePoint, Vector3D normalVec, double radius, Brush faceColor, Brush backColor)
        {
            int n = 36;
            List<Point3D> plist = GetCirclePlist(n, circlePoint, radius);
            List<int> nlist = GetCircleNlist(n);
            MeshGeometry3D mgd = new() { Positions = new Point3DCollection(plist), TriangleIndices = new Int32Collection(nlist) };
            GeometryModel3D gmd = new() { Geometry = mgd, Material = new DiffuseMaterial() { Brush= faceColor },BackMaterial = new DiffuseMaterial() { Brush= backColor } };
            Model3DGroup mdg = new();
            mdg.Children.Add(gmd);

            var plistNew = plist;
            plistNew.RemoveAt(0);
            plistNew.Insert(0, new Point3D(circlePoint.X, circlePoint.Y, circlePoint.Z + height));
            MeshGeometry3D mgdTop = new() { Positions = new Point3DCollection(plistNew), TriangleIndices = new Int32Collection(nlist) };
            GeometryModel3D gmdTop = new() { Geometry = mgdTop, Material = new DiffuseMaterial() { Brush = faceColor }, BackMaterial = new DiffuseMaterial() { Brush = backColor } };
            mdg.Children.Add(gmdTop);
            Model3D res = mdg;
            return res;
        }

        /// <summary>
        /// 画圆柱
        /// </summary>
        /// <param name="height"></param>
        /// <param name="circlePoint"></param>
        /// <param name="normalVec"></param>
        /// <param name="radius"></param>
        /// <param name="faceColor"></param>
        /// <param name="backColor"></param>
        /// <returns></returns>
        public static Model3D DrawCylinder(double height, Vector3D circlePoint, Vector3D normalVec, double radius, Brush upColor, Brush downColor, Brush sideColor)
        {
            int n = 36;
            Vector3D myNormalVec = new(0, 0, 1);
            Vector3D upCirclePoint = new(circlePoint.X, circlePoint.Y, circlePoint.Z + height);
            Model3DGroup mdg = new();
            //画底圆
            mdg.Children.Add(DrawCircle(circlePoint, myNormalVec, radius, Brushes.Black, downColor));
            var plistUp = GetCirclePlist(n, upCirclePoint, radius);
            plistUp.RemoveAt(0);
            var plistDown = GetCirclePlist(n, circlePoint, radius);
            plistDown.RemoveAt(0);
            plistUp.AddRange(plistDown);

            // 画侧面
            MeshGeometry3D mgd = new MeshGeometry3D()
            {
                Positions = new Point3DCollection(plistUp),
                TriangleIndices = new Int32Collection(GetCylinderSideIndices(n)),
            };
            GeometryModel3D gmd = new() { Geometry = mgd, Material = new DiffuseMaterial(sideColor) };
            mdg.Children.Add(gmd);

            //画顶圆
            mdg.Children.Add(DrawCircle(upCirclePoint, myNormalVec, radius, upColor, Brushes.Black));

            return mdg;
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

        static List<int> GetCylinderSideIndices(int n)
        {
            List<int> vs = new();
            for (int i = 0; i < n - 1; i++)
            {
                vs.Add(i);
                vs.Add(i + n);
                vs.Add(i + n + 1);

                vs.Add(i);
                vs.Add(i + n + 1);
                vs.Add(i + 1);
            }

            vs.Add(n - 1);
            vs.Add(2 * n - 1);
            vs.Add(n);

            vs.Add(n - 1);
            vs.Add(n);
            vs.Add(0);

            return vs;
        }
    }
}
