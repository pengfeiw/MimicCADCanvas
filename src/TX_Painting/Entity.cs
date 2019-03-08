using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Windows;

namespace Diagram
{
    [DataContract]
    public abstract class Entity
    {
        [DataMember]
        public string m_Name;
        [DataMember]
        public Color m_Color = Color.White;
        public abstract void paint(Graphics graphics, CoordTransform ctf, HashSet<string> names);
        public abstract RectangleF bound();
        //public abstract void SetHighlight(Graphics graphics);
        public static RectangleF bound(PointF[] points)
        {
            float minX = points.Min(pt => pt.X);
            float minY = points.Min(pt => pt.Y);
            float maxX = points.Max(pt => pt.X);
            float maxY = points.Max(pt => pt.Y);
            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
            //return new RectangleF(new PointF(minX, minY), maxX - minX, maxY - minY);
        }

        //Entity每次绘图前需要调整Y坐标（调用该函数）
        protected static void CoordYadjust(PointF []points)
        {
            for (int i = 0; i < points.Length; i++)
                points[i] = new PointF(points[i].X, -points[i].Y);
        }
    }

    //直线
    [DataContract]
    public class DLineStrip : Entity
    {
        [DataMember]
        public PointF[] m_Points;
        public DLineStrip() { }
        public DLineStrip(PointF point1, PointF point2) 
        {
            m_Points = new PointF[2] { point1, point2 };
            CoordYadjust(m_Points);
        }
        public DLineStrip(PointF[] pointArray) { 
            m_Points = pointArray;
            CoordYadjust(m_Points);
        }
        public override void paint(Graphics graphics, CoordTransform ctf, HashSet<string> names)
        {
            if (m_Points.Count() < 2)
                return;
            PointF[] pts = new PointF[m_Points.Length];
            for (int i = 0; i < m_Points.Count(); i++)
                pts[i] = ctf.WcsToDcs(m_Points[i]);
            Color color = m_Color;
            if (names.Contains(m_Name)) 
                color = Color.Red;
            Pen pen = new Pen(color);
            graphics.DrawLines(pen, pts);
        }
        public override RectangleF bound()
        {
            return Entity.bound(m_Points);
        }
    }

    //虚线
    public class DDottedLine : Entity
    {
        public PointF[] m_points;
        public DDottedLine(PointF point1, PointF point2)
        {
            PointF[] pointArray = new PointF[2] { point1, point2 };
            m_points = pointArray;
            CoordYadjust(m_points);
        }
        public DDottedLine(PointF[] pointArray)
        {
            m_points = pointArray;
            CoordYadjust(m_points);
        }
        public override void paint(Graphics graphics, CoordTransform ctf, HashSet<string> names)
        {
            if (m_points.Count() < 2)
                return;
            PointF[] pts = new PointF[m_points.Length];
            for (int i = 0; i < m_points.Length; i++)
                pts[i] = ctf.WcsToDcs(m_points[i]);
            Color color = m_Color;
            if (names.Contains(m_Name))
                color = Color.Red;
            Pen pen = new Pen(color);
            //pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            pen.DashPattern = new float[] { 25.0F, 10.0F };
            graphics.DrawLines(pen, pts);
        }
        public override RectangleF bound()
        {
            return Entity.bound(m_points);
        }
    }

    //中心线
    public class DCentLines : Entity
    {
        public PointF[] m_points;
        public DCentLines(PointF point1, PointF point2)
        {
            m_points = new PointF[] { point1, point2 };
            CoordYadjust(m_points);
        }
        public DCentLines(PointF[] pointArray) {
            m_points = pointArray;
            CoordYadjust(m_points);
        }

        public override void paint(Graphics graphics, CoordTransform ctf, HashSet<string> names)
        {
            if (m_points.Count() < 2)
                return;
            PointF[] pts = new PointF[m_points.Length];
            for (int i = 0; i < m_points.Length; i++)
                pts[i] = ctf.WcsToDcs(m_points[i]);
            Color color = m_Color;
            if (names.Contains(m_Name))
                color = Color.Red;
            Pen pen = new Pen(color);
            //pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            pen.DashPattern = new float[] { 55.0F, 15.0F, 15.0F, 15.0F};
            graphics.DrawLines(pen, pts);
        }

        public override RectangleF bound()
        {
            return Entity.bound(m_points);
        }
    }

    //curve
    [DataContract]
    public class DCurve : Entity
    {
        [DataMember]
        public PointF[] m_Points;
        public DCurve() { }
        public DCurve(PointF[] pointArray) { 
            m_Points = pointArray;
            CoordYadjust(m_Points);
        }
        public override void paint(Graphics graphics, CoordTransform ctf, HashSet<string> names)
        {
            if (m_Points.Count() < 2)
                return;
            PointF[] pts = new PointF[m_Points.Length];
            for (int i = 0; i < m_Points.Count(); i++)
                pts[i] = ctf.WcsToDcs(m_Points[i]);
            Color color = m_Color;
            if (names.Contains(m_Name)) 
                color = Color.Red;
            Pen pen = new Pen(color);
            graphics.DrawCurve(pen, pts);
        }
        public override RectangleF bound()
        {

            return Entity.bound(m_Points);
        }
    }

    public enum TextAlignment
    {
        UpLeft, LeftCenter, DwnLeft, UpCenter, Center,
        DwnCenter, UpRight, RightCenter, DwnRight
    }
    //文本
    [DataContract]
    public class DText : Entity
    {
        [DataMember]
        public string m_text;
        [DataMember]
        public string m_font = "宋体";
        [DataMember]
        public PointF m_point;
        [DataMember]
        public float m_angle = 0;
        [DataMember]
        public float m_size = 10;
        [DataMember]
        public TextAlignment m_textAlignment = TextAlignment.Center; //默认居中对齐。
        public DText(string text, PointF point)
        {
            m_text = text;
            m_point = point;
            //CoordYadjust(m_point);
            m_point.Y = -m_point.Y;
        }
        public DText(string text, PointF point, float angle)
        {
            m_text = text;
            m_point = point;
            m_angle = angle;
            m_point.Y = -m_point.Y;
        }
        public DText(string text, PointF point, float angle, Diagram.TextAlignment textAlignment)
        {
            m_text = text;
            m_point = point;
            m_angle = angle;
            m_textAlignment = textAlignment;
            m_point.Y = -m_point.Y;
        }
        //当传入的dcsOrWcsPnt是设备坐标时，得到的就是设备坐标（UpLeftPnt), 传入的是世界坐标，得到的就是世界坐标。
        private PointF GetUpLeftPnt(TextAlignment textAlignment, SizeF textSize, PointF dcsOrWcsPnt)
        {
            PointF textPnt = new PointF();
            double cosAng = Math.Cos(m_angle * Math.PI / 180);
            double sinAng = Math.Sin(m_angle * Math.PI / 180);
            double diagonal = Math.Sqrt(textSize.Height * textSize.Height + textSize.Width * textSize.Width); //textSize对角线长度

            switch (textAlignment)
            {
                case TextAlignment.UpLeft:
                    textPnt = dcsOrWcsPnt;
                    break;
                case TextAlignment.LeftCenter:
                    textPnt.X = (float)(dcsOrWcsPnt.X + sinAng * textSize.Height * 0.5);
                    textPnt.Y = (float)(dcsOrWcsPnt.Y - cosAng * textSize.Height * 0.5);
                    break;
                case TextAlignment.DwnLeft:
                    textPnt.X = (float)(dcsOrWcsPnt.X + sinAng * textSize.Height);
                    textPnt.Y = (float)(dcsOrWcsPnt.Y - cosAng * textSize.Height);
                    break;
                case TextAlignment.UpCenter:
                    textPnt.X = (float)(dcsOrWcsPnt.X - cosAng * textSize.Width * 0.5);
                    textPnt.Y = (float)(dcsOrWcsPnt.Y - sinAng * textSize.Width * 0.5);
                    break;
                case TextAlignment.Center:
                    double temanglePi = Math.Atan(textSize.Height / textSize.Width);
                    double temangle = temanglePi / Math.PI * 180;
                    double ang = temangle + m_angle;
                    double temCosAng = Math.Cos(ang * Math.PI / 180);
                    double temSinAng = Math.Sin(ang * Math.PI / 180);
                    textPnt.X = (float)(dcsOrWcsPnt.X - 0.5 * diagonal * temCosAng);
                    textPnt.Y = (float)(dcsOrWcsPnt.Y - 0.5 * diagonal * temSinAng);
                    break;
                case TextAlignment.DwnCenter:
                    double temangle1Pi = Math.Atan(textSize.Height / (0.5 * textSize.Width));
                    double temangle1 = temangle1Pi * 180 / Math.PI;
                    double ang1 = temangle1 + m_angle;
                    double temCosAng1 = Math.Cos(ang1 * Math.PI / 180);
                    double temSinAng1 = Math.Sin(ang1 * Math.PI / 180);
                    double temLen1 = Math.Sqrt(textSize.Height * textSize.Height + 0.25 * textSize.Width * textSize.Width);
                    textPnt.X = (float)(dcsOrWcsPnt.X - temLen1 * temCosAng1);
                    textPnt.Y = (float)(dcsOrWcsPnt.Y - temLen1 * temSinAng1);
                    break;
                case TextAlignment.UpRight:
                    textPnt.X = (float)(dcsOrWcsPnt.X - cosAng * textSize.Width);
                    textPnt.Y = (float)(dcsOrWcsPnt.Y - sinAng * textSize.Width);
                    break;
                case TextAlignment.RightCenter:
                    double temangle2Pi = Math.Atan(textSize.Height * 0.5 / textSize.Width);
                    double temangle2 = temangle2Pi * 180 / Math.PI;
                    double ang2 = temangle2 + m_angle;
                    double temCosAng2 = Math.Cos(ang2 * Math.PI / 180);
                    double temSinAng2 = Math.Sin(ang2 * Math.PI / 180);
                    textPnt.X = (float)(dcsOrWcsPnt.X - textSize.Width * temCosAng2);
                    textPnt.Y = (float)(dcsOrWcsPnt.Y - textSize.Width * temSinAng2);
                    break;
                case TextAlignment.DwnRight:
                    double temangle3Pi = Math.Atan(textSize.Height / textSize.Width);
                    double temangle3 = temangle3Pi * 180 / Math.PI;
                    double ang3 = temangle3 + m_angle;
                    double temCosAng3 = Math.Cos(ang3 * Math.PI / 180);
                    double temSinAng3 = Math.Sin(ang3 * Math.PI / 180);
                    textPnt.X = (float)(dcsOrWcsPnt.X - diagonal * temCosAng3);
                    textPnt.Y = (float)(dcsOrWcsPnt.Y - diagonal * temSinAng3);
                    break;
            }
            return textPnt;
        }
        public override void paint(Graphics graphics, CoordTransform ctf, HashSet<string> names)
        {
            float size = (float)ctf.WlToDl(m_size);
            Font textFont = new Font(m_font, size);
            SizeF textSize = graphics.MeasureString(m_text, textFont);//文字的矩形框的size。
            //textSize.Height = (float)ctf.WlToDl(textSize.Height);
            //textSize.Width = (float)ctf.WlToDl(textSize.Width);
            PointF dcsPoint = ctf.WcsToDcs(m_point);
            PointF point = GetUpLeftPnt(m_textAlignment, textSize, dcsPoint);
            //PointF point = ctf.WcsToDcs(textPnt);

            Color color = m_Color;
            if (names.Contains(m_Name))
                color = Color.Red;
            Brush brush = new SolidBrush(color);
            graphics.TranslateTransform(point.X, point.Y);
            graphics.RotateTransform(m_angle);
            graphics.DrawString(m_text, textFont, brush, new PointF(0, 0));
            graphics.ResetTransform();
        }
        
        public override RectangleF bound()
        {
            Font font = new Font(m_font, m_size);
            Image fakeImage = new Bitmap(1, 1); //As we cannot use CreateGraphics() in a class library, so the fake image is used to load the Graphics.
            Graphics graphics = Graphics.FromImage(fakeImage);
            SizeF size = graphics.MeasureString(m_text, font);
            PointF temPnt = GetUpLeftPnt(m_textAlignment, size, m_point);
            //return new RectangleF(m_point, size);
            if (m_angle == 0)
                return new RectangleF(temPnt, size);
            else
            {
                double ang = m_angle * Math.PI / 180;
                PointF[] points = new PointF[4];
                double cosAng = Math.Cos(ang);
                double sinAng = Math.Sin(ang);
                points[0] = temPnt;
                points[1] = new PointF((float)(temPnt.X + cosAng * size.Width), (float)(temPnt.Y + sinAng * size.Width));
                double dx = sinAng * size.Height;
                double dy = cosAng * size.Height;
                points[2] = new PointF((float)(temPnt.X - dx), (float)(temPnt.Y + dy));
                points[3] = new PointF((float)(points[1].X -dx), (float)(points[1].Y + dy));
                return Entity.bound(points);
            }
        }
    }
    

    //圆
    [DataContract]
    public class DCircle : Entity
    {
        [DataMember]
        public PointF m_Center;
        [DataMember]
        public float m_Radius;
        public DCircle(PointF cen, float r)
        {
            m_Center = cen;
            m_Radius = r;
            m_Center.Y = -m_Center.Y;
        }
        public override void paint(Graphics graphics, CoordTransform ctf, HashSet<string> names)
        {
            PointF dcsCenter = ctf.WcsToDcs(m_Center);
            double dcsRadius = ctf.WlToDl(m_Radius);
            Color color = m_Color;
            if (names.Contains(m_Name))
                color = Color.Red;
            Pen pen = new Pen(color);
            PointF leftTop = dcsCenter;
            leftTop.X -= (float)dcsRadius;
            leftTop.Y -= (float)dcsRadius;
            graphics.DrawEllipse(pen, leftTop.X, leftTop.Y, (float)(2 * dcsRadius), (float)(2 * dcsRadius));
        }

        public override RectangleF bound()
        {
            return new RectangleF(m_Center.X - m_Radius, m_Center.Y - m_Radius, 2 * m_Radius, 2 * m_Radius);
        }
    }

    //圆弧
    [DataContract]
    public class DArc : Entity
    {
        [DataMember]
        public PointF m_center;
        [DataMember]
        public float m_radiusX;
        [DataMember]
        public float m_radiusY;
        [DataMember]
        public float m_startAngle;
        [DataMember]
        public float m_sweepAngle;
        public DArc(PointF center, float radius, float startAngle, float sweepAngle)
        {
            m_center = center;
            m_radiusX = radius;
            m_radiusY = radius;
            m_startAngle = startAngle;
            m_sweepAngle = sweepAngle;
            m_center.Y = -m_center.Y;
        }

        public DArc(PointF center, float radiusX, float radiusY, float startAngle, float sweepAngle)
        {
            m_center = center;
            m_radiusX = radiusX;
            m_radiusY = radiusY;
            m_startAngle = startAngle;
            m_sweepAngle = sweepAngle;
            m_center.Y = -m_center.Y;
        }

        public override void paint(Graphics graphics, CoordTransform ctf, HashSet<string> names)
        {
            PointF center = ctf.WcsToDcs(m_center);
            float radiusX = (float)ctf.WlToDl(m_radiusX);
            float radiusY = (float)ctf.WlToDl(m_radiusY);
            Color color = m_Color;
            RectangleF rect = new RectangleF(PointF.Subtract(center, new SizeF(radiusX, radiusY)), new SizeF(2 * radiusX, 2 * radiusY));
            if (names.Contains(m_Name))
                color = Color.Red;
            graphics.DrawArc(new Pen(color), rect, m_startAngle, m_sweepAngle);
        }

        public override RectangleF bound()
        {
            PointF point1 = new PointF();
            PointF point2 = new PointF();
            if (m_startAngle == 0)
                point1 = PointF.Add(m_center, new SizeF(m_radiusX, 0));
            else
            {
                float tanStartAngle = (float)Math.Tan(m_startAngle / 180 * Math.PI);
                float X = (float)(1 / (Math.Sqrt(1 / (m_radiusX * m_radiusX) + tanStartAngle * tanStartAngle / (m_radiusY * m_radiusY))));
                float Y = tanStartAngle * X;
                point1.X = X + m_center.X;
                point1.Y = Y + m_center.Y;
            }
            if (m_sweepAngle == 0)
                point2 = PointF.Add(m_center, new SizeF(m_radiusX, 0));
            else
            {
                float EndAngle = m_startAngle + m_sweepAngle;
                float tanEndAngle = (float)Math.Tan(EndAngle / 180 * Math.PI);
                float X = (float)(1 / (Math.Sqrt(1 / (m_radiusX * m_radiusX) + tanEndAngle * tanEndAngle / (m_radiusY * m_radiusY))));
                point2.X = X + m_center.X;
                float Y = tanEndAngle * X;
                point2.Y = Y + m_center.Y;
            }
            PointF[] points= new PointF[2]{point1, point2};

            return Entity.bound(points);
        }
    }

    //椭圆
    [DataContract]
    public class DEllipse : Entity 
    {
        [DataMember]
        public PointF m_center;
        [DataMember]
        public double m_angle = 0; //旋转角度(以度为单位)
        [DataMember]
        public double m_xlength;
        [DataMember]
        public double m_ylength;

        public DEllipse(PointF center, double xlength, double ylength)
        {
            m_center = center;
            m_xlength = xlength;
            m_ylength = ylength;

            m_center.Y = -m_center.Y;
        }
        public DEllipse(PointF center, double angle, double xlength, double ylength)
        {
            m_center = center;
            m_angle = angle;
            m_xlength = xlength;
            m_ylength = ylength;

            m_center.Y = -m_center.Y;
        }
        public override void paint(Graphics graphics, CoordTransform ctf, HashSet<string> names)
        {
            PointF center = ctf.WcsToDcs(m_center);
            double xlength = ctf.WlToDl(m_xlength);
            double ylength = ctf.WlToDl(m_ylength);
            Color color = m_Color;
            if (names.Contains(m_Name))
                color = Color.Red;
            Pen pen = new Pen(color);
            graphics.TranslateTransform((float)(center.X), (float)(center.Y));
            graphics.RotateTransform((float)m_angle);
            RectangleF rec = new RectangleF((float)( -0.5 * xlength), (float)(-0.5 * ylength), (float)xlength, (float)ylength);
            graphics.DrawEllipse(pen, rec);
            graphics.ResetTransform();
        }

        public override RectangleF bound()
        {
            if(m_angle == 0)
                return new RectangleF(PointF.Add(m_center, new SizeF((float)(-0.5 * m_xlength), (float)(-0.5 * m_ylength))),new SizeF((float)m_xlength, (float)m_ylength));
            else
            {
                double temang = m_angle * Math.PI / 180;
                double  halfDiagonal = Math.Sqrt(0.25 * m_xlength * m_xlength + 0.25 * m_ylength * m_ylength);
                double temAng = Math.Atan(0.5 * m_ylength / (0.5 * m_xlength)) * Math.PI / 180;
                double ang = temang - temAng;
                double cosAng = Math.Cos(ang);
                double sinAng = Math.Sin(ang);
                double tanAng = Math.Tan(ang);
                double ang2 = Math.PI * 0.5 - temang - temAng;
                double cosAng2 = Math.Cos(ang2);
                double sinAng2 = Math.Sin(ang2);
                double tanAng2 = Math.Tan(ang2);
                PointF p1 = new PointF((float)(m_center.X + cosAng * halfDiagonal), (float)(m_center.Y + sinAng * halfDiagonal));
                PointF p2 = new PointF((float)(m_center.X + sinAng2 * halfDiagonal), (float)(m_center.Y + cosAng2 * halfDiagonal));
                PointF p3 = new PointF((float)(m_center.X - cosAng * halfDiagonal), (float)(m_center.Y - sinAng * halfDiagonal));
                PointF p4 = new PointF((float)(m_center.X - sinAng2 * halfDiagonal), (float)(m_center.Y - cosAng2 * halfDiagonal));
                PointF []points = new PointF[4]{p1, p2, p3, p4};
                return Entity.bound(points);
            }
        }
    }

    //多义线结点类型
    public struct polNode
    {
        public PointF m_point;
        /// <summary>
        /// 凸度[正为右,负为左]   [当凸度为正值时，圆弧为向直线顺指针转方向凸起;当凸度为负值时，圆弧为向直线逆指针转方向凸起]   弦高和两点直线长一半的比值
        /// </summary>
        public double m_bulge;             //凸度[正为右,负为左]   [当凸度为正值时，圆弧为向直线顺指针转方向凸起;当凸度为负值时，圆弧为向直线逆指针转方向凸起]  弦高和两点直线长一半的比值
        //public double m_startWidth;     //控制接下来的一段线的起始宽度
        //public double m_endWidth;     //控制接下来的以短线的终止宽度
        public polNode(PointF point, double bulge)
        {
            m_point = point;
            m_bulge = bulge;
            //m_startWidth = 0;
            //m_endWidth = 0;
        }
        //public polNode(PointF point, double bulge, double startWidth, double endWidth)
        //{
        //    m_point = point;
        //    m_bulge = bulge;
        //    m_startWidth = startWidth;
        //    m_endWidth = endWidth;
        //}
    }
    //多义线
    [DataContract]
    public class DPolyLine : Entity
    {
        [DataMember]
        public polNode[] m_plNode;
        [DataMember]
        public bool m_isClosed = false;
        //private PointF points;
        public DPolyLine() { }
        public DPolyLine(polNode[] NodeArray)
        {
            m_plNode = NodeArray;
            for (int i = 0; i < m_plNode.Length; i++)
                m_plNode[i].m_point.Y = -m_plNode[i].m_point.Y;
        }
        public DPolyLine(polNode[] NodeArray, bool IsColsed)
        {
            m_plNode = NodeArray;
            for (int i = 0; i < m_plNode.Length; i++)
                m_plNode[i].m_point.Y = -m_plNode[i].m_point.Y;
            m_isClosed = IsColsed;
        }
        private Vector RotateVector(Vector oriVector, double degree)
        {
            double angle = Math.Atan(oriVector.Y / oriVector.X);
            double length = Math.Sqrt(oriVector.X * oriVector.X + oriVector.Y * oriVector.Y);
            double X = Math.Cos(angle + degree) * length;
            double Y = Math.Sin(angle + degree) * length;
            return new Vector(X, Y);
        }

        private PointF GetTenNode(polNode p1, polNode p2)
        {
            PointF point;
            double bulge = p1.m_bulge;
            if (bulge == 0)
            {
                point = new PointF((p1.m_point.X + p2.m_point.X) / 2, (p1.m_point.Y + p2.m_point.Y) / 2);
                return point;
            }
            else
            {
                double degree = Math.Atan(bulge);
                double DisP1ToP2 = Math.Sqrt((p2.m_point.X - p1.m_point.X) * (p2.m_point.X - p1.m_point.X) + (p2.m_point.Y - p1.m_point.Y) * (p2.m_point.Y - p1.m_point.Y));
                double length = 0.5* DisP1ToP2 / (Math.Cos(degree));
                Vector P1ToP2_vector = new Vector((p2.m_point.X - p1.m_point.X), (p2.m_point.Y - p1.m_point.Y));
                Vector oriVector = new Vector((length / DisP1ToP2 * P1ToP2_vector.X), (length / DisP1ToP2 * P1ToP2_vector.Y));
                Vector vector = RotateVector(oriVector, degree);
                PointF mid_point = new PointF((float)(p1.m_point.X + vector.X), (float)(p1.m_point.Y + vector.Y));
                return mid_point;
            }
        }

        public override void paint(Graphics graphics, CoordTransform ctf, HashSet<string> names)
        {
            if (m_plNode.Count() < 2)
                return;
            Color color = m_Color;
            if (names.Contains(m_Name))
                color = Color.Red;
            Pen pen = new Pen(color);
            for (int i = 0; i < m_plNode.Length - 1; i++)
            {

                if (m_plNode[i].m_bulge == 0)
                {
                    PointF point1 = ctf.WcsToDcs(m_plNode[i].m_point);
                    PointF point2 = ctf.WcsToDcs(m_plNode[i + 1].m_point);
                    graphics.DrawLine(pen, point1, point2);
                }
                else
                {
                    PointF mid_point = GetTenNode(m_plNode[i], m_plNode[i + 1]);
                    PointF[] points = new PointF[3] { ctf.WcsToDcs(m_plNode[i].m_point), ctf.WcsToDcs(mid_point), ctf.WcsToDcs(m_plNode[i + 1].m_point) };
                    graphics.DrawCurve(pen, points);
                }
            }
            //如果多义线是闭合的
            if (m_isClosed)
            {
                if (m_plNode[m_plNode.Length - 1].m_bulge == 0)
                {
                    PointF point1 = ctf.WcsToDcs(m_plNode[m_plNode.Length - 1].m_point);
                    PointF point2 = ctf.WcsToDcs(m_plNode[0].m_point);
                    graphics.DrawLine(pen, point1, point2);
                }
                else
                {
                    PointF mid_point = GetTenNode(m_plNode[m_plNode.Length - 1], m_plNode[0]);
                    PointF[] points = new PointF[3] { ctf.WcsToDcs(m_plNode[m_plNode.Length - 1].m_point), ctf.WcsToDcs(mid_point), ctf.WcsToDcs(m_plNode[0].m_point) };
                    graphics.DrawCurve(pen, points);
                }
            }
        }
        public override RectangleF bound()
        {
            if (m_plNode.Count() < 2)
                return new RectangleF();
            int midPntLen = m_isClosed ? m_plNode.Length : m_plNode.Length - 1;
            PointF[] midPoints = new PointF[midPntLen];
            for (int i = 0; i < m_plNode.Length - 1; i++)
                midPoints[i] = GetTenNode(m_plNode[i], m_plNode[i + 1]);
            if (m_isClosed)
                midPoints[midPoints.Length - 1] = GetTenNode(m_plNode[m_plNode.Length - 1], m_plNode[0]);
            PointF[] points = new PointF[midPoints.Length + m_plNode.Length];
            for (int i = 0; i < (points.Length / 2); i++)
            {
                points[2 * i] = m_plNode[i].m_point;
                points[2 * i + 1] = midPoints[i];
            }
            points[points.Length - 1] = m_plNode[m_plNode.Length - 1].m_point;
            return Entity.bound(points);
        }
    }

    public enum TextPos { Inside, OutSide };
    //标注(其中文字对齐方式默认为DwnCenter)
    [DataContract]
    public class DDimension : Entity
    {
        //[DataMember]
        //public double m_obliquePI = Math.PI;
        [DataMember]
        public PointF m_dimPnt1;
        [DataMember]
        public PointF m_dimPnt2;
        [DataMember]
        public PointF m_dimLinePnt;
        //public double m_rotation = 0;
        [DataMember]
        public TextPos m_textPos = TextPos.Inside;
        [DataMember]
        private double m_textAngle;
        [DataMember]
        public double m_textSize = 20;
        [DataMember]
        public string m_text;
        [DataMember]
        public string m_font = "宋体";
        [DataMember]
        private double m_extlen = 40;
        [DataMember]
        private double m_insideDis = 5;
        [DataMember]
        private double m_outsideDis = 10;

        public DDimension(PointF pnt1, PointF pnt2, string text)
        {
            m_dimPnt1 = pnt1;
            m_dimPnt2 = pnt2;
            m_dimLinePnt = new PointF((float)(0.5 * (pnt1.X + pnt2.X)), (float)(0.5 * (pnt1.Y + pnt2.Y)));
            m_text = text;

            m_dimPnt1.Y = -m_dimPnt1.Y;
            m_dimPnt2.Y = -m_dimPnt2.Y;
            m_dimLinePnt.Y = -m_dimLinePnt.Y;
            //textPos = TextPos.Inside;
        }
        public DDimension(PointF pnt1, PointF pnt2, PointF dimLinePnt, string text)
        {
            m_dimPnt1 = pnt1;
            m_dimPnt2 = pnt2;
            m_dimLinePnt = dimLinePnt;
            m_text = text;

            m_dimPnt1.Y = -m_dimPnt1.Y;
            m_dimPnt2.Y = -m_dimPnt2.Y;
            m_dimLinePnt.Y = -m_dimLinePnt.Y;
        }
        public DDimension(PointF pnt1, PointF pnt2, PointF dimLinePnt, string text, TextPos textPos)
        {
            m_dimPnt1 = pnt1;
            m_dimPnt2 = pnt2;
            m_dimLinePnt = dimLinePnt;
            m_textPos = textPos;
            m_text = text;

            m_dimPnt1.Y = -m_dimPnt1.Y;
            m_dimPnt2.Y = -m_dimPnt2.Y;
            m_dimLinePnt.Y = -m_dimLinePnt.Y;
        }

        //旋转矩阵
        private static Vector rotate(Vector vec, double anglePi)
        {
            double vecX = vec.X;
            double vecY = vec.Y;
            double resX = vecX * Math.Cos(anglePi) - vecY * Math.Sin(anglePi);
            double resY = vecX * Math.Sin(anglePi) + vecY * Math.Cos(anglePi);
            return new Vector(resX, resY);
        }

        //点到直线的距离，直线由point1和point2确定。
        private static double PointToLine(PointF point1, PointF point2, PointF point3)
        {
            double A = point2.Y - point1.Y;
            double B = point1.X - point2.X;
            double C = point2.X * point1.Y - point1.X * point2.Y;
            double d = Math.Abs((A * point3.X + B * point3.Y + C) / (Math.Sqrt(A * A + B * B)));
            return d;
        }
        //点到直线的垂足
        private PointF GetFootPnt(PointF point1, PointF point2, PointF point3)
        {
            double A = point2.Y - point1.Y;
            double B = point1.X - point2.X;
            double C = point2.X * point1.Y - point1.X * point2.Y;
            double pntX = (B * B * point3.X - A * B * point3.Y - A * C) / (A * A + B * B);
            double pntY = (A * A * point3.Y - A * B * point3.X - B * C) / (A * A + B * B);
            return new PointF((float)pntX, (float)pntY);
        }
        //       points[0]        points[1]
        //           |                | /_______extlen
        //           |   intersectPnt2|
        //points[2]----------------------points[3]
        //           |intersectPnt1   |
        //           |                |
        //           |                |
        //  points[4]|                |points[5]
        private PointF[] GetDefinePnts(PointF dimPnt1, PointF dimPnt2, PointF intersectPnt1, PointF intersectPnt2, double extlen)
        {
            PointF[] resPnts = new PointF[6];
            Vector offsetVec = new Vector(intersectPnt1.X - dimPnt1.X, intersectPnt1.Y - dimPnt1.Y);
            Vector pnt1Topnt2 = new Vector(intersectPnt2.X - intersectPnt1.X, intersectPnt2.Y - intersectPnt1.Y);
            pnt1Topnt2.Normalize();
            offsetVec.Normalize();
            //double extlen = 20;
            resPnts[0] = new PointF((float)(intersectPnt1.X + offsetVec.X * extlen), (float)(intersectPnt1.Y + offsetVec.Y * extlen));
            resPnts[1] = new PointF((float)(intersectPnt2.X + offsetVec.X * extlen), (float)(intersectPnt2.Y + offsetVec.Y * extlen));
            resPnts[2] = new PointF((float)(intersectPnt1.X - pnt1Topnt2.X * extlen), (float)(intersectPnt1.Y - pnt1Topnt2.Y * extlen));
            resPnts[3] = new PointF((float)(intersectPnt2.X + pnt1Topnt2.X * extlen), (float)(intersectPnt2.Y + pnt1Topnt2.Y * extlen));
            resPnts[4] = dimPnt1;
            resPnts[5] = dimPnt2;
            return resPnts;
        }
        public override void paint(Graphics graphics, CoordTransform ctf, HashSet<string> names)
        {
            double insideDis = ctf.WlToDl(m_insideDis);
            double outsideDis = ctf.WlToDl(m_outsideDis);
            double extlen = ctf.WlToDl(m_extlen); //标注3根线交点处延长的距离。
            if (m_dimPnt1.Equals(m_dimPnt2))
                return;
            PointF dimPnt1 = ctf.WcsToDcs(m_dimPnt1);
            PointF dimPnt2 = ctf.WcsToDcs(m_dimPnt2);
            PointF dimLinePnt = ctf.WcsToDcs(m_dimLinePnt);
            double textSize = ctf.WlToDl(m_textSize);
            //处理当dimlinePnt在dimPnt1和dimPnt2线上时的情况，处理的不好。
            if (PointToLine(dimPnt1, dimPnt2, dimLinePnt) == 0)
            {
                if (dimPnt1.X != dimPnt2.X)
                    dimLinePnt = new PointF(dimLinePnt.X, (float)(dimLinePnt.Y + ctf.WlToDl(30)));
                else if (dimPnt1.Y != dimPnt2.Y)
                    dimLinePnt = new PointF(dimLinePnt.X + (float)(ctf.WlToDl(30)), dimLinePnt.Y);
            }
            Color color = m_Color;
            if (names.Contains(m_Name))
                color = Color.Red;
            double offsetDist = PointToLine(dimPnt1, dimPnt2, dimLinePnt);
            PointF footPnt = GetFootPnt(dimPnt1, dimPnt2, dimLinePnt); //dimLinePnt到dimPnt1和dimPnt2确定的直线的垂足点。
            Vector offsetVec = new Vector(dimLinePnt.X - footPnt.X, dimLinePnt.Y - footPnt.Y);
            if (offsetVec != new Vector(0, 0))
                offsetVec.Normalize();
            //PointF intersectPnt1 = Vector.Add(Vector.Multiply(offsetDist, dirVec), footPnt);
            PointF intersectPnt1 = new PointF((float)(dimPnt1.X + offsetDist * offsetVec.X), (float)(dimPnt1.Y + offsetDist * offsetVec.Y));
            PointF intersectPnt2 = new PointF((float)(dimPnt2.X + offsetDist * offsetVec.X), (float)(dimPnt2.Y + offsetDist * offsetVec.Y));
            PointF[] definePnts = GetDefinePnts(dimPnt1, dimPnt2, intersectPnt1, intersectPnt2, extlen);
            PointF midPnt = new PointF((float)(0.5 * (intersectPnt1.X + intersectPnt2.X)), (float)(0.5 * (intersectPnt1.Y + intersectPnt2.Y)));
            PointF textPoint;
            if (m_textPos == TextPos.Inside)
                textPoint = new PointF((float)(midPnt.X + offsetVec.X * insideDis), (float)(midPnt.Y + offsetVec.Y * insideDis));
            else
                textPoint = new PointF((float)(midPnt.X + offsetVec.X * outsideDis), (float)(midPnt.Y + offsetVec.Y * outsideDis));
            Vector dimPntVec = new Vector(dimPnt2.X - dimPnt1.X, dimPnt2.Y - dimPnt1.Y);
            //if (dimPnt1.Y == dimPnt2.Y)
            //    m_textAngle = 0;
            //else
            //    m_textAngle = dimPnt1.Y > dimPnt2.Y ? Vector.AngleBetween(dimPntVec, new Vector(1, 0)) : -Vector.AngleBetween(dimPntVec, new Vector(1, 0));
            m_textAngle = -Vector.AngleBetween(dimPntVec, new Vector(1, 0));

            Pen pen = new Pen(color);
            graphics.DrawLine(pen, definePnts[0], definePnts[4]);
            graphics.DrawLine(pen, definePnts[1], definePnts[5]);
            graphics.DrawLine(pen, definePnts[2], definePnts[3]);

            //构造DText对象。之前代码有无作用的，以后考虑改掉。
            PointF tem_point = ctf.DcsToWcs(textPoint);
            tem_point.Y = -tem_point.Y;
            DText dimText = new DText(m_text, tem_point, (float)m_textAngle, TextAlignment.DwnCenter);
            dimText.m_Color = color;
            dimText.m_Name = m_Name;
            dimText.m_size = (float)ctf.DlToWl(textSize);
            dimText.paint(graphics, ctf, names);

            //graphics.TranslateTransform(textPoint.X, textPoint.Y);
            //graphics.RotateTransform((float)m_textAngle);
            //graphics.DrawString(m_text, new Font(m_font, (float)textSize), new SolidBrush(color), new PointF(0, 0));
            //graphics.ResetTransform();

        }

        public override RectangleF bound()
        {
            //标注线的包围框
            if (m_dimPnt1.Equals(m_dimPnt2))
                return new RectangleF();
            //处理当dimlinePnt在dimPnt1和dimPnt2线上时的情况，处理的不好。
            if (PointToLine(m_dimPnt1, m_dimPnt2, m_dimLinePnt) == 0)
            {
                if (m_dimPnt1.X != m_dimPnt2.X)
                    m_dimLinePnt = new PointF(m_dimLinePnt.X, (float)(m_dimLinePnt.Y + 30));
                else if (m_dimPnt1.Y != m_dimPnt2.Y)
                    m_dimLinePnt = new PointF(m_dimLinePnt.X + (float)(30), m_dimLinePnt.Y);
            }

            double offsetDist = PointToLine(m_dimPnt1, m_dimPnt2, m_dimLinePnt);
            PointF footPnt = GetFootPnt(m_dimPnt1, m_dimPnt2, m_dimLinePnt); //dimLinePnt到dimPnt1和dimPnt2确定的直线的垂足点。
            Vector offsetVec = new Vector(m_dimLinePnt.X - footPnt.X, m_dimLinePnt.Y - footPnt.Y);
            if (offsetVec != new Vector(0, 0))
                offsetVec.Normalize();
            //PointF intersectPnt1 = Vector.Add(Vector.Multiply(offsetDist, dirVec), footPnt);
            PointF intersectPnt1 = new PointF((float)(m_dimPnt1.X + offsetDist * offsetVec.X), (float)(m_dimPnt1.Y + offsetDist * offsetVec.Y));
            PointF intersectPnt2 = new PointF((float)(m_dimPnt2.X + offsetDist * offsetVec.X), (float)(m_dimPnt2.Y + offsetDist * offsetVec.Y));
            PointF[] definePnts = GetDefinePnts(m_dimPnt1, m_dimPnt2, intersectPnt1, intersectPnt2, m_extlen);
            RectangleF dimLineRec = bound(definePnts);

            //文字包围框
            //double temangle1Pi = Math.Atan(textSize.Height / (0.5 * textSize.Width));
            //double temangle1 = temangle1Pi * 180 / Math.PI;
            //double ang1 = temangle1 + m_angle;
            //double temCosAng1 = Math.Cos(ang1 * Math.PI / 180);
            //double temSinAng1 = Math.Sin(ang1 * Math.PI / 180);
            //double temLen1 = Math.Sqrt(textSize.Height * textSize.Height + 0.25 * textSize.Width * textSize.Width);
            //textPnt.X = (float)(dcsOrWcsPnt.X - temLen1 * temCosAng1);
            //textPnt.Y = (float)(dcsOrWcsPnt.Y - temLen1 * temSinAng1);

            PointF midPnt = new PointF((float)(0.5 * (intersectPnt1.X + intersectPnt2.X)), (float)(0.5 * (intersectPnt1.Y + intersectPnt2.Y)));
            PointF textPoint;
            if (m_textPos == TextPos.Inside)
                textPoint = new PointF((float)(midPnt.X + offsetVec.X * m_insideDis), (float)(midPnt.Y + offsetVec.Y * m_insideDis));
            else
                textPoint = new PointF((float)(midPnt.X + offsetVec.X * m_outsideDis), (float)(midPnt.Y + offsetVec.Y * m_outsideDis));
            Vector dimPntVec = new Vector(m_dimPnt2.X - m_dimPnt1.X, m_dimPnt2.Y - m_dimPnt1.Y);
            if (m_dimPnt1.Y == m_dimPnt2.Y)
                m_textAngle = 0;
            else
                m_textAngle = m_dimPnt1.Y > m_dimPnt2.Y ? Vector.AngleBetween(dimPntVec, new Vector(1, 0)) : -Vector.AngleBetween(dimPntVec, new Vector(1, 0));
            Font font = new Font(m_font, (float)m_textSize);
            Image fakeImage = new Bitmap(1, 1); //As we cannot use CreateGraphics() in a class library, so the fake image is used to load the Graphics.
            Graphics graphics = Graphics.FromImage(fakeImage);
            SizeF size = graphics.MeasureString(m_text, font);

            double temanglePi = Math.Atan(size.Height / (0.5 * size.Width));
            double temangle = temanglePi * 180 / Math.PI;
            double ang = temangle + m_textAngle;
            double temCosAng = Math.Cos(ang * Math.PI / 180);
            double temSinAng = Math.Sin(ang * Math.PI / 180);
            double temLen = Math.Sqrt(size.Height * size.Height + 0.25 * size.Width * size.Width);
            textPoint.X = (float)(textPoint.X - temLen * temCosAng);
            textPoint.Y = (float)(textPoint.Y - temLen * temSinAng);

            ////以下是新添加代码，使标注文字对齐方式按照DwnRight对齐方式。
            ////SizeF textSize = graphics.MeasureString(m_text, font);
            //double temanglePi = Math.Atan(size.Height / (0.5 * size.Width));
            //double temangle = temanglePi * 180 / Math.PI;
            //double ang = temangle + m_textAngle;
            //double temCosAng = Math.Cos(ang * Math.PI / 180);
            //double temSinAng = Math.Sin(ang * Math.PI / 180);
            //double temLen = Math.Sqrt(size.Height * size.Height + 0.25 * size.Width * size.Width);
            //textPoint.X = (float)(textPoint.X - temLen * temCosAng);
            //textPoint.Y = (float)(textPoint.Y - temLen * temSinAng);

            RectangleF textRec;
            if (m_textAngle == 0)
                textRec = new RectangleF(textPoint, size);
            else
            {
                double angPi = m_textAngle * Math.PI / 180;
                PointF[] points = new PointF[4];
                double cosAng = Math.Cos(angPi);
                double sinAng = Math.Sin(angPi);
                points[0] = textPoint;
                points[1] = new PointF((float)(textPoint.X + cosAng * size.Width), (float)(textPoint.Y + sinAng * size.Width));
                double dx = sinAng * size.Height;
                double dy = cosAng * size.Height;
                points[2] = new PointF((float)(textPoint.X - dx), (float)(textPoint.Y + dy));
                points[3] = new PointF((float)(points[1].X - dx), (float)(points[1].Y + dy));
                textRec = bound(points);
            }

            RectangleF resRec = RectangleF.Union(dimLineRec, textRec);
            return resRec;

            //Font font = new Font(m_font, m_size);
            //Image fakeImage = new Bitmap(1, 1); //As we cannot use CreateGraphics() in a class library, so the fake image is used to load the Graphics.
            //Graphics graphics = Graphics.FromImage(fakeImage);
            //SizeF size = graphics.MeasureString(m_text, font);
            ////return new RectangleF(m_point, size);
            //if (m_angle == 0)
            //    return new RectangleF(m_point, size);
            //else
            //{
            //    double ang = m_angle * Math.PI / 180;
            //    PointF[] points = new PointF[4];
            //    double cosAng = Math.Cos(ang);
            //    double sinAng = Math.Sin(ang);
            //    points[0] = m_point;
            //    points[1] = new PointF((float)(m_point.X + cosAng * size.Width), (float)(m_point.Y + sinAng * size.Width));
            //    double dx = sinAng * size.Height;
            //    double dy = cosAng * size.Height;
            //    points[2] = new PointF((float)(m_point.X - dx), (float)(m_point.Y + dy));
            //    points[3] = new PointF((float)(points[1].X - dx), (float)(points[1].Y + dy));
            //    return Entity.bound(points);
            //}

        }
        #region OLD CODE
        //[DataMember]
        //public PointF m_point1;
        //[DataMember]
        //public PointF m_point2;
        //[DataMember]
        //public PointF m_point3;
        //[DataMember]
        //public double m_dis1;
        //[DataMember]
        //public double m_dis2;
        //[DataMember]
        //public PointF m_textPoint;
        //[DataMember]
        //public string m_text;
        //[DataMember]
        //public double m_textSize;
        ////public bool m_decTextLoca;
        //// textLoca可能用的上。
        ////bool值用于确定m_textPoint是否由用户给出，true时由用户给的点确定，false时由程序自动计算得到。
        //public DDimension(bool decTextLoca,PointF textLoca, PointF point1, PointF point2, PointF point3, double dis1, double dis2, string text, double textSize)
        //{
        //    m_point1 = point1;
        //    m_point2 = point2;
        //    m_point3 = point3;

        //    m_point1.Y = -m_point1.Y;
        //    m_point2.Y = -m_point2.Y;
        //    m_point3.Y = -m_point3.Y;
        //    //m_decTextLoca = decTextLoca;
        //    m_dis1 = dis1;
        //    m_dis2 = dis2;
        //    m_text = text;
        //    m_textSize = textSize;
        //    if (!decTextLoca)
        //    {
        //        PointF mid_p1Top2 = new PointF((float)(0.5*(m_point1.X + m_point2.X)), (float)(0.5*(m_point1.Y + m_point2.Y)));
        //        Vector vect1 = new Vector(m_point3.X - mid_p1Top2.X, m_point3.Y - mid_p1Top2.Y);
        //        Vector vect_1 = vect1;
        //        vect_1.Normalize();
        //        Vector vect2 = 25 * vect_1;
        //        Vector vect = vect1 + vect2;
        //        m_textPoint = new PointF((float)(mid_p1Top2.X + vect.X), (float)(mid_p1Top2.Y + vect.Y));
        //    }
        //    else m_textPoint = textLoca;
        //}

        //public override void paint(Graphics graphics, CoordTransform ctf, HashSet<string> names)
        //{
        //    //相关点。
        //    PointF mid_p1Top2 = new PointF((float)(0.5 * (m_point1.X + m_point2.X)), (float)(0.5 * (m_point1.Y + m_point2.Y)));
        //    Vector vect1 = new Vector(m_point3.X - mid_p1Top2.X, m_point3.Y - mid_p1Top2.Y);
        //    Vector vect_1 = vect1;
        //    vect_1.Normalize();
        //    Vector y_vect = vect1 + m_dis1 * vect_1;
        //    PointF point1 = new PointF((float)(m_point1.X + y_vect.X), (float)(m_point1.Y + y_vect.Y));
        //    PointF point2 = new PointF((float)(m_point2.X + y_vect.X), (float)(m_point2.Y + y_vect.Y));
        //    Vector vect2 = new Vector(m_point2.X - m_point1.X, m_point2.Y - m_point1.Y);
        //    Vector vect_2 = vect2;
        //    vect_2.Normalize();
        //    Vector x_vect1 = 0.5 * vect2 + m_dis2 * vect_2;
        //    Vector x_vect2 = x_vect1;
        //    x_vect2.Negate();
        //    PointF point4 = new PointF((float)(m_point3.X + x_vect2.X), (float)(m_point3.Y + x_vect2.Y));
        //    PointF point5 = new PointF((float)(m_point3.X + x_vect1.X), (float)(m_point3.Y + x_vect1.Y));

        //    PointF n_point1 = ctf.WcsToDcs(point1);
        //    PointF n_point2 = ctf.WcsToDcs(point2);
        //    //PointF n_m_point3 = ctf.WcsToDcs(m_point3);
        //    PointF n_m_point1 = ctf.WcsToDcs(m_point1);
        //    PointF n_m_point2 = ctf.WcsToDcs(m_point2);
        //    PointF n_point4 = ctf.WcsToDcs(point4);
        //    PointF n_point5 = ctf.WcsToDcs(point5);
        //    Color color = m_Color;
        //    if (names.Contains(m_Name))
        //        color = Color.Red;
        //    Pen pen = new Pen(color);
        //    graphics.DrawLine(pen, n_point1, n_m_point1);
        //    graphics.DrawLine(pen, n_point2, n_m_point2);
        //    graphics.DrawLine(pen, n_point4, n_point5);
        //    //写text。
        //    Font font = new Font("宋体", (float)m_textSize);
        //    Image fakeImage = new Bitmap(1, 1); //As we cannot use CreateGraphics() in a class library, so the fake image is used to load the Graphics.
        //    Graphics gra = Graphics.FromImage(fakeImage);
        //    SizeF size = gra.MeasureString(m_text, font);
        //    Vector vect3 = vect2;
        //    vect3.Negate();
        //    vect3.Normalize();
        //    Vector xy_vect = vect3 * size.Width * 0.5;
        //    PointF textlocation = new PointF((float)(m_textPoint.X + xy_vect.X), (float)(m_textPoint.Y + xy_vect.Y));
        //    double text_angle = Math.Atan(vect2.Y / vect2.X);
        //    textlocation = ctf.WcsToDcs(textlocation);
        //    double textSize = ctf.WlToDl(m_textSize);
        //    graphics.TranslateTransform(textlocation.X, textlocation.Y);
        //    graphics.RotateTransform((float)(180 * text_angle / Math.PI));
        //    graphics.DrawString(m_text, new Font("宋体", (float)textSize), new SolidBrush(color), new PointF(0, 0));
        //    graphics.ResetTransform();
        //}

        //public override RectangleF bound()
        //{
        //    PointF mid_p1Top2 = new PointF((float)(0.5 * (m_point1.X + m_point2.X)), (float)(0.5 * (m_point1.Y + m_point2.Y)));
        //    Vector vect1 = new Vector(m_point3.X - mid_p1Top2.X, m_point3.Y - mid_p1Top2.Y);
        //    Vector vect_1 = vect1;
        //    vect_1.Normalize();
        //    Vector y_vect = vect1 + m_dis1 * vect_1;
        //    PointF point1 = new PointF((float)(m_point1.X + y_vect.X), (float)(m_point1.Y + y_vect.Y));
        //    PointF point2 = new PointF((float)(m_point2.X + y_vect.X), (float)(m_point2.Y + y_vect.Y));
        //    Vector vect2 = new Vector(m_point2.X - m_point1.X, m_point2.Y - m_point1.Y);
        //    Vector vect_2 = vect2;
        //    vect_2.Normalize();
        //    Vector x_vect1 = 0.5 * vect2 + m_dis2 * vect_2;
        //    Vector x_vect2 = x_vect1;
        //    x_vect2.Negate();
        //    PointF point4 = new PointF((float)(m_point3.X + x_vect2.X), (float)(m_point3.Y + x_vect2.Y));
        //    PointF point5 = new PointF((float)(m_point3.X + x_vect1.X), (float)(m_point3.Y + x_vect1.Y));
        //    PointF[] points1 = new PointF[] { m_point1, m_point2, point1, point2, point4, point5 };
        //    RectangleF rect1 = Entity.bound(points1);//标注线的包围框。

        //    Font font = new Font("宋体", (float)m_textSize);
        //    Image fakeImage = new Bitmap(1, 1); //As we cannot use CreateGraphics() in a class library, so the fake image is used to load the Graphics.
        //    Graphics gra = Graphics.FromImage(fakeImage);
        //    SizeF size = gra.MeasureString(m_text, font);
        //    Vector vect3 = vect2;
        //    vect3.Negate();
        //    vect3.Normalize();
        //    Vector xy_vect = vect3 * size.Width * 0.5;
        //    //Vector xy_vect = new Vector(vect3.X - size.Width, vect3.Y - size.Height);
        //    PointF textlocation = new PointF((float)(m_textPoint.X + xy_vect.X), (float)(m_textPoint.Y + xy_vect.Y));
        //    double text_angle = Math.Atan(vect2.Y / vect2.X);
        //    RectangleF rect2;//标注文字包围框。
        //    if (text_angle == 0)
        //        rect2 = new RectangleF(textlocation, size);
        //    else
        //    {
        //        double ang = text_angle * Math.PI / 180;
        //        PointF[] points2 = new PointF[4];
        //        double cosAng = Math.Cos(ang);
        //        double sinAng = Math.Sin(ang);
        //        points2[0] = textlocation;
        //        points2[1] = new PointF((float)(textlocation.X + cosAng * size.Width), (float)(textlocation.Y + sinAng * size.Width));
        //        double dx = sinAng * size.Height;
        //        double dy = cosAng * size.Height;
        //        points2[2] = new PointF((float)(textlocation.X - dx), (float)(textlocation.Y + dy));
        //        points2[3] = new PointF((float)(points2[1].X - dx), (float)(points2[1].Y + dy));
        //        rect2 = Entity.bound(points2);
        //    }
        //    return RectangleF.Union(rect1, rect2);
        //}
        #endregion
    }
}
