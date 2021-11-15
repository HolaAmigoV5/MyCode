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

        #region 画同心圆柱
        /// <summary>
        /// 画同心圆柱
        /// </summary>
        /// <returns></returns>
        public static Model3D DrawArcCylinder(Vector3D circlePoint, double radiusIn, double radiusOut, double height, int n, int nStart, int nEnd,
            Brush upColor, Brush downColor, Brush arcInColor, Brush arcOutColor)
        {
            Model3DGroup mdg = new();
            var upCirclePoint = new Vector3D(circlePoint.X, circlePoint.Y, circlePoint.Z + height);

            // 画上同心圆
            mdg.Children.Add(DrawHollowCircle(upCirclePoint, radiusIn, radiusOut, upColor, upColor, n, nStart, nEnd));

            // 画下同心圆
            mdg.Children.Add(DrawHollowCircle(circlePoint, radiusIn, radiusOut, downColor, downColor, n, nStart, nEnd));

            // 画内侧面
            mdg.Children.Add(DrawCylinderArc(height, circlePoint, radiusIn, arcInColor, arcInColor, n, nStart, nEnd));

            // 画外侧面
            mdg.Children.Add(DrawCylinderArc(height, circlePoint, radiusOut, arcOutColor, arcInColor, n, nStart, nEnd));

            return mdg;
        }

        /// <summary>
        /// 画同心圆
        /// </summary>
        /// <param name="circlePoint">圆心位置</param>
        /// <param name="radiusIn">内圆半径</param>
        /// <param name="radiusOut">外圆半径</param>
        /// <param name="foreColor">前面颜色</param>
        /// <param name="backColor">后面颜色</param>
        /// <param name="n">点数量</param>
        /// <param name="nStart">开始位置</param>
        /// <param name="nEnd">结束位置</param>
        /// <returns></returns>
        private static GeometryModel3D DrawHollowCircle(Vector3D circlePoint, double radiusIn, double radiusOut, Brush foreColor, Brush backColor, int n, int nStart, int nEnd)
        {
            //Point3DCollection positions = new();
            //// 内圆点的集合
            //positions.AppendPoint3DCollection(GetHollowCirclePlist(n, circlePoint, radiusIn));
            //// 外圆点的集合
            //positions.AppendPoint3DCollection(GetHollowCirclePlist(n, circlePoint, radiusOut));

            var positions = GetHollowCirclePlist(n, circlePoint, radiusIn).AppendPoint3DCollection(GetHollowCirclePlist(n, circlePoint, radiusOut));
            return DrawGeometry3D(positions, GetArcIndices(n, nStart, nEnd), foreColor, backColor);
        }

        /// <summary>
        /// 画同心圆侧曲面
        /// </summary>
        /// <param name="height">高度</param>
        /// <param name="circlePoint">原点</param>
        /// <param name="radius">半径</param>
        /// <param name="foreColor">前面颜色</param>
        /// <param name="backColor">背面颜色</param>
        /// <param name="n">点数量</param>
        /// <param name="nStart">开始位置</param>
        /// <param name="nEnd">结束位置</param>
        /// <returns></returns>
        public static GeometryModel3D DrawCylinderArc(double height, Vector3D circlePoint, double radius, Brush foreColor, Brush backColor, int n, int nStart, int nEnd)
        {
            //List<Point3D> pl = new();
            //pl.AddRange(GetHollowCirclePlist(n, new Vector3D(circlePoint.X, circlePoint.Y, circlePoint.Z + height), radius));
            //pl.AddRange(GetHollowCirclePlist(n, circlePoint, radius));

            var positions = GetHollowCirclePlist(n, new Vector3D(circlePoint.X, circlePoint.Y, circlePoint.Z + height), radius).AppendPoint3DCollection(GetHollowCirclePlist(n, circlePoint, radius));
            return DrawGeometry3D(positions, GetArcIndices(n, nStart, nEnd), foreColor, backColor);
        }

        /// <summary>
        /// 画圆需要的点的集合（不包含圆心点）
        /// </summary>
        /// <param name="n">点的个数</param>
        /// <param name="circlePoint">圆心</param>
        /// <param name="radius">半径</param>
        /// <returns></returns>
        private static Point3DCollection GetHollowCirclePlist(int n, Vector3D circlePoint, double radius)
        {
            double angle = 2 * Math.PI / n;
            Point3DCollection pl = new();
            for (int i = 0; i < n; i++)
            {
                pl.Add(new Point3D(circlePoint.X + radius * Math.Cos(i * angle), circlePoint.Y + radius * Math.Sin(i * angle), circlePoint.Z));
            }
            return pl;
        }

        /// <summary>
        /// 扇形三角形绘制顺序
        /// </summary>
        /// <param name="n">个数</param>
        /// <param name="nStart">开始位置</param>
        /// <param name="nEnd">结束位置</param>
        /// <returns></returns>
        private static Int32Collection GetArcIndices(int n, int nStart, int nEnd)
        {
            Int32Collection triangleL = new();
            int i;
            for (i = nStart; i < nEnd; i++)
            {
                if (i == n - 1)
                    break;
                triangleL.Add(i);
                triangleL.Add(n + i);
                triangleL.Add(n + i + 1);

                triangleL.Add(i);
                triangleL.Add(n + i + 1);
                triangleL.Add(i + 1);
            }

            if (i == n - 1)
            {
                triangleL.Add(n - 1);
                triangleL.Add(2 * n - 1);
                triangleL.Add(n);

                triangleL.Add(n - 1);
                triangleL.Add(n);
                triangleL.Add(0);
            }
            return triangleL;
        }

        private static GeometryModel3D DrawGeometry3D(Point3DCollection positions, Int32Collection tIndices, Brush foreColor, Brush backColor)
        {
            GeometryModel3D gmd = new()
            {
                Geometry = new MeshGeometry3D() { Positions = positions, TriangleIndices = tIndices },
                Material = new DiffuseMaterial() { Brush = foreColor },
                BackMaterial = new DiffuseMaterial() { Brush = backColor }
            };
            return gmd;
        }
        #endregion
    }
}
