using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Novacode;
using System.Windows.Media.Imaging;

namespace Diagram
{
    public partial class DiagramControl : Control
    {
        private HashSet<string> m_HightlightedNames = new HashSet<string>();
        public CoordTransform m_Ctf = new CoordTransform();
        public List<Entity> m_Entities = new List<Entity>();
        private Point m_LastPoint;
        public DiagramControl()
        {
            InitializeComponent();
            EnableDoubleBuffering();
        }

        //设置相应线条高亮，ent 是需要显示高亮的实体。
        public void SetHighlight(HashSet<string> names)
        {
            m_HightlightedNames.Clear();
            m_HightlightedNames.UnionWith(names);
            Invalidate();
        }

        public void SetHighlight(string name)
        {
            m_HightlightedNames.Clear();
            if (name.Length != 0)
                m_HightlightedNames.Add(name);
            Invalidate();
        }

        /// <summary>
        /// 写入json数据
        /// </summary>
        /// <param name="SavePath"></param>
        public void SaveAsJson(string SavePath)
        {
            var fs = new FileStream(SavePath, FileMode.Create);
            // Serializer the User object to the stream.  
            var ser = new DataContractJsonSerializer(typeof(List<Entity>), new Type[] { typeof(DLineStrip), typeof(DCircle), typeof(DCurve), typeof(DText), typeof(DArc), typeof(DPolyLine), typeof(DDimension)});
            ser.WriteObject(fs, m_Entities);
            fs.Close();
        }
        /// <summary>
        /// 读json数据
        /// </summary>
        /// <param name="LoadPath"></param>
        public void ReadFromJson(String LoadPath)
        {
            try
            {
                var fs = new FileStream(LoadPath, FileMode.Open);

                // Serializer the User object to the stream.  
                var ser = new DataContractJsonSerializer(typeof(List<Entity>), new Type[] { typeof(DLineStrip), typeof(DCircle), typeof(DCurve), typeof(DText), typeof(DArc), typeof(DPolyLine),typeof(DDimension)});
                m_Entities = (List<Entity>)ser.ReadObject(fs);
                fs.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("The file path is not exist or data corruption.");
            }
        }

        
        private RectangleF UnionBound()
        {
            if (m_Entities.Count() == 0)
                return new RectangleF();
            RectangleF rect = m_Entities[0].bound();
            for (int i = 1; i < m_Entities.Count(); i++)
                rect = RectangleF.Union(rect, m_Entities[i].bound());
            return rect;
        }
        /// <summary>
        /// 显示全图。
        /// </summary>
        public void ViewAll()
        {
            int margin/*pixel*/ = 5;
            RectangleF rect = UnionBound();
            m_Ctf.ViewAll(rect, this.ClientRectangle, margin);
        }

        /// <summary>
        /// 取消闪烁
        /// </summary>
        private void EnableDoubleBuffering()
        {
            // Set the value of the double-buffering style bits to true.
            this.SetStyle(ControlStyles.DoubleBuffer |
               ControlStyles.UserPaint |
               ControlStyles.AllPaintingInWmPaint,
               true);

            this.UpdateStyles();

        }       

        /// <summary>
        /// 截取控件图像，并存为word文档。
        /// </summary>
        /// <param name="DocX_path"></param>
        public void WndScreenShot(string DocX_path)
        {
            BackColor = Color.White;
            foreach (Entity ent in m_Entities)
                if (ent.m_Color == Color.White)
                    ent.m_Color = Color.Black;
            ViewAll();
            this.Invalidate();

            Bitmap bm = new Bitmap(Width, Height);
            DrawToBitmap(bm, ClientRectangle);
            MemoryStream memoryStream = new MemoryStream();
            bm.Save(memoryStream, ImageFormat.Png);
            if (!File.Exists(DocX_path))
            {
                using (DocX document = DocX.Create(DocX_path, DocumentTypes.Document))
                {
                    var image = document.AddImage(new MemoryStream(memoryStream.ToArray()));
                    var picture = image.CreatePicture(Height, Width);
                    var p = document.InsertParagraph();
                    p.AppendPicture(picture);
                    p.SpacingAfter(30);
                    document.Save();
                }
            }
            else
            {
                using (DocX document = DocX.Load(DocX_path))
                {
                    var image = document.AddImage(new MemoryStream(memoryStream.ToArray()));
                    var picture = image.CreatePicture(Height, Width);
                    var p = document.InsertParagraph();
                    p.AppendPicture(picture);
                    p.SpacingAfter(30);
                    document.Save();
                }
            }
            memoryStream.Close();
        }
        

        
        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics graphics = pe.Graphics;
            for (int i = 0; i < m_Entities.Count(); i++)
                m_Entities[i].paint(graphics, m_Ctf, m_HightlightedNames);
        }


        private void DiagramControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
                m_LastPoint = e.Location;
            this.Focus();
        }
        private void DiagramControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                m_Ctf.Pan(m_LastPoint, e.Location);
                m_LastPoint = e.Location;
                this.Invalidate();
            }
        }

        private void DiagramControl_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void DiagramControl_MouseWheel(object sender, MouseEventArgs e)
        {
            if(m_Ctf.Zoom(e.Location, e.Delta))
                this.Invalidate();
        }

        private void DiagramControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                //ViewAll();
                //m_Ctf.ViewAll(UnionBound(), this.ClientRectangle, 5);//new RectangleF(new PointF(0,0), this.Size)
                ViewAll();
                this.Invalidate();
            }
        } 
    }
}
