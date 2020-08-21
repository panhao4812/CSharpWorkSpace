using Rhino;
using Rhino.Geometry;
using Rhino.DocObjects;
using Rhino.Collections;

using GH_IO;
using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace MaterialTools
{
    public class InitializeBitmap
    {
        public string str;
        public Bitmap bitmap0;//map
        public Bitmap bitmap1;//Transparent
        public Bitmap bitmap2;//bump
        public InitializeBitmap() {        
            Open();
        }
        public InitializeBitmap(int u,int v) {
            Create(u, v);
            str = @"C:\maps\temp1.jpg";
        }
        public void Open()
        {      
                try
                {
                    OpenFileDialog openFileDialog1 = new OpenFileDialog();
                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        Stream myStream = null;
                        if ((myStream = openFileDialog1.OpenFile()) != null)
                        {
                            bitmap1 = new Bitmap(myStream);
                            str = openFileDialog1.FileName;
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("There was an error opening the image file.");
                }           
        }   
        public void Create(int u, int v)
        {
            bitmap1 = new Bitmap(u, v);
            Graphics g = Graphics.FromImage(bitmap1);
            g.FillRectangle(new SolidBrush(Color.White), 0, 0, u, v);
            g.Dispose();
        }

        public void Save()
        {
            try
            {               
                bitmap1.Save(str);
            }
            catch (Exception)
            {
                MessageBox.Show("There was an error creating the image file.");
            }
        }
  

        public void CurveCP(Brep x, Curve y, int V, int U)
        {
            int u = bitmap1.Size.Width;
            int v = bitmap1.Size.Height;
            Graphics g = Graphics.FromImage(bitmap1);
            g.FillRectangle(new SolidBrush(Color.White), 0, 0, u, v);
            System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(Color.Black);
            float step1 = u / U;
            float step2 = v / V;
            for (float i = 25; i < u - 25; i += step1)
            {
                for (float j = 25; j < v - 25; j += step2)
                {
                    double Umin = x.Faces[0].Domain(0).Min;
                    double Umax = x.Faces[0].Domain(0).Max;
                    double Vmin = x.Faces[0].Domain(1).Min;
                    double Vmax = x.Faces[0].Domain(1).Max;
                    Point3d pos = x.Faces[0].PointAt(i / u * (Umax - Umin) + Umin, j / v * (Vmax - Vmin) + Vmin);
                    double t; float R;
                    if (y.ClosestPoint(pos, out t, 200))
                    {
                        double dis = y.PointAt(t).DistanceTo(pos);
                        dis /= 200;
                        R = (float)(1 / dis * 2);
                        if (R > 40) R = 40;               
                    }
                    else { R = 20; }
                    g.FillEllipse(myBrush, i - R, v - j - R, R * 2, R * 2);
                }
            }
            myBrush.Dispose();           
            g.Dispose();
        }

    }
    ///////////////////////////
}
