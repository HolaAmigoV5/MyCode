using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
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

        #region 绘制三角锥(四面体)
        public static Viewport3D TransparentScene()
        {
            // Define the camera
            PerspectiveCamera qCamera = new PerspectiveCamera();
            qCamera.Position = new Point3D(0, .25, 2.25);
            qCamera.LookDirection = new Vector3D(0, -.05, -1);
            qCamera.UpDirection = new Vector3D(0, 1, 0);
            qCamera.FieldOfView = 100;

            // Define a lighting model
            DirectionalLight qLight = new DirectionalLight();
            qLight.Color = Colors.White;
            qLight.Direction = new Vector3D(-0.5, -0.25, -0.5);

            // Define the animated rotation transformation
            RotateTransform3D qRotation =
                new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 1));
            DoubleAnimation qAnimation = new DoubleAnimation();
            qAnimation.From = 1;
            qAnimation.To = 361;
            qAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(5000));
            qAnimation.RepeatBehavior = RepeatBehavior.Forever;
            qRotation.Rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, qAnimation);

            // Define the geometry
            const double kdSqrt2 = 1.4142135623730950488016887242097;
            const double kdSqrt6 = 2.4494897427831780981972840747059;
            // Create a collection of vertex positions
            Point3D[] qaV = new Point3D[4]{
                new Point3D(0.0, 1.0, 0.0),
                new Point3D(2.0 * kdSqrt2 / 3.0, -1.0 / 3.0, 0.0),
                new Point3D(-kdSqrt2 / 3.0, -1.0 / 3.0, -kdSqrt6 / 3.0),
                new Point3D(-kdSqrt2 / 3.0, -1.0 / 3.0, kdSqrt6 / 3.0)};
            Point3DCollection qPoints = new Point3DCollection();
            // Designate Vertices
            // My Scheme (0, 1, 2), (1, 0, 3), (2, 3, 0), (3, 2, 1)
            for (int i = 0; i < 12; ++i)
            {
                if (i / 3 % 2 == 0)
                {
                    qPoints.Add(qaV[i % 4]);
                }
                else
                {
                    qPoints.Add(qaV[i * 3 % 4]);
                }
            }
            // Designate Triangles
            Int32Collection qTriangles = new Int32Collection();
            for (int i = 0; i < 12; ++i)
            {
                qTriangles.Add(i);
            }
            Int32Collection qBackTriangles = new Int32Collection();
            // Designate Back Triangles in the opposite orientation
            for (int i = 0; i < 12; ++i)
            {
                qBackTriangles.Add(3 * (i / 3) + (2 * (i % 3) % 3));
            }

            // Inner Tetrahedron: Define the mesh, material and transformation.
            MeshGeometry3D qFrontMesh = new MeshGeometry3D();
            qFrontMesh.Positions = qPoints;
            qFrontMesh.TriangleIndices = qTriangles;
            GeometryModel3D qInnerGeometry = new GeometryModel3D();
            qInnerGeometry.Geometry = qFrontMesh;
            // *** Material ***
            DiffuseMaterial qDiffGreen =
                new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(255, 0, 128, 0)));
            SpecularMaterial qSpecWhite = new
                SpecularMaterial(new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)), 30.0);
            MaterialGroup qInnerMaterial = new MaterialGroup();
            qInnerMaterial.Children.Add(qDiffGreen);
            qInnerMaterial.Children.Add(qSpecWhite);
            qInnerGeometry.Material = qInnerMaterial;
            // *** Transformation ***
            ScaleTransform3D qScale = new ScaleTransform3D(new Vector3D(.5, .5, .5));
            Transform3DGroup myTransformGroup = new Transform3DGroup();
            myTransformGroup.Children.Add(qRotation);
            myTransformGroup.Children.Add(qScale);
            qInnerGeometry.Transform = myTransformGroup;

            // Outer Tetrahedron (semi-transparent) : Define the mesh, material and transformation.
            GeometryModel3D qOuterGeometry = new GeometryModel3D();
            qOuterGeometry.Geometry = qFrontMesh;
            // *** Material ***
            DiffuseMaterial qDiffTransYellow =
                new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(64, 255, 255, 0)));
            SpecularMaterial qSpecTransWhite =
                new SpecularMaterial(new SolidColorBrush(Color.FromArgb(128, 255, 255, 255)), 30.0);
            MaterialGroup qOuterMaterial = new MaterialGroup();
            qOuterMaterial.Children.Add(qDiffTransYellow);
            qOuterMaterial.Children.Add(qSpecTransWhite);
            qOuterGeometry.Material = qOuterMaterial;
            // *** Transformation ***
            qOuterGeometry.Transform = qRotation;

            // Outer Tetrahedron (solid back) : Define the mesh, material and transformation.
            MeshGeometry3D qBackMesh = new MeshGeometry3D();
            qBackMesh.Positions = qPoints;
            qBackMesh.TriangleIndices = qBackTriangles;
            GeometryModel3D qBackGeometry = new GeometryModel3D();
            qBackGeometry.Geometry = qBackMesh;
            // *** Material ***
            DiffuseMaterial qDiffBrown =
                new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(255, 200, 175, 0)));
            qBackGeometry.Material = qDiffBrown;
            // *** Transformation ***
            qBackGeometry.Transform = qRotation;

            // Collect the components
            Model3DGroup qModelGroup = new Model3DGroup();
            qModelGroup.Children.Add(qLight);
            qModelGroup.Children.Add(qBackGeometry);
            qModelGroup.Children.Add(qInnerGeometry);
            qModelGroup.Children.Add(qOuterGeometry);
            ModelVisual3D qVisual = new ModelVisual3D();
            qVisual.Content = qModelGroup;
            Viewport3D qViewport = new Viewport3D();
            qViewport.Children.Add(qVisual);
            qViewport.Camera = qCamera;

            return qViewport;
        }
        #endregion
    }
}
