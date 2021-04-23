using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace KUKALibrary
{
    public class kuka
    {
        public kuka() { }
        public void ReadFileFromRobot(string scrF,string dataF,out List<Plane> Planes, out List<double> Vels)
        {
            FileOpenData(scrF);
            FileOpenData(dataF);
            List<Plane> plsout = new List<Plane>();
            List<double> velsout = new List<double>();
            for (int i = 0; i < names2.Count; i++)
            {
                string str = names2[i];
                if (str == "HOME" && i != 0 && i != names2.Count - 1)
                {
                    plsout.Add(CartToPLN(890, 0, 1080 + 50, 0, 90, 0));
                    velsout.Add(vels[i]);
                }
                else
                {
                    str = "X" + str;
                    for (int j = 0; j < names.Count; j++)
                    {
                        if (names[j] == str)
                        {
                            plsout.Add(pls[j]);
                            velsout.Add(vels[i]);
                            break;
                        }
                    }
                }
            }
            Planes = plsout;
            Vels = velsout;
        }
        List<Plane> pls = new List<Plane>();
        List<string> names = new List<string>();
        List<string> names2 = new List<string>();
        List<double> vels = new List<double>();
        public string FileOpenData(string path)
        {
            pls.Clear(); names.Clear();
            if (!File.Exists(path)) return "null file";
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open);
                StreamReader srd = new StreamReader(fs);
                while (srd.Peek() != -1)
                {
                    string str = srd.ReadLine();
                    string[] chara = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (chara.Length > 1)
                    {
                        //DECL E6POS XP1
                        string[] title = chara[0].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        if (title[0] == "DECL" && title[1] == "E6POS")
                        {
                            names.Add(title[2]);
                            pls.Add(STRtoPlane(str));
                        }
                    }
                }
                srd.Close();
            }
            catch (Exception ex)
            {
                return (ex.ToString());
            }
            return "Success!";
        }
        public string FileOpenScr(string path)
        {
            names2.Clear(); vels.Clear();
            if (!File.Exists(path)) return "null file";
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open);
                StreamReader srd = new StreamReader(fs);
                while (srd.Peek() != -1)
                {
                    string str = srd.ReadLine();
                    string[] title = str.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (title.Length > 3)
                    {
                        // Print(str);
                        //;FOLD SPTP P2 Vel=50 % PDAT2 Tool[1] Base[0] ;%{PE}
                        if (title[0] == ";FOLD" && title[1] == "SPTP")
                        {
                            names2.Add(title[2]);
                            string vel = title[3].Remove(0, 4);
                            vels.Add(Convert.ToDouble(vel));
                        }
                    }
                }
                srd.Close();
            }
            catch (Exception ex)
            {
                return (ex.ToString());
            }
            return "Success!";
        }
        public string FileOpenData()
        {
            String path = "";
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                path = ofd.FileName;
            }
            else
            {
                return "";
            }
            return FileOpenData(path);
        }
        public string FileOpenScr()
        {
            String path = "";
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                path = ofd.FileName;
            }
            else
            {
                return "";
            }
            return FileOpenScr(path);
        }
        private Plane STRtoPlane(string x)
        {
            string _A = "", _B = "", _C = "", _D = "", _E = "", _F = "";
            string str = x.Split('{')[1];
            str = str.Split('}')[0];
            string[] strs = str.Split(',');
            for (int i = 0; i < strs.Length; i++)
            {
                string str1 = strs[i];
                if (str1.Contains("X "))
                {
                    _A = str1.Remove(0, 2);
                }
                else if (str1.Contains("Y "))
                {
                    _B = str1.Remove(0, 2);
                }
                else if (str1.Contains("Z "))
                {
                    _C = str1.Remove(0, 2);
                }
                else if (str1.Contains("A "))
                {
                    _D = str1.Remove(0, 2);
                }
                else if (str1.Contains("B "))
                {
                    _E = str1.Remove(0, 2);
                }
                else if (str1.Contains("C "))
                {
                    _F = str1.Remove(0, 2);
                }
            }
            //Print(_A + " " + _B + " " + _C + " " + _D + " " + _E + " " + _F);
            return CartToPLN(
              Convert.ToDouble(_A), Convert.ToDouble(_B), Convert.ToDouble(_C),
              Convert.ToDouble(_D), Convert.ToDouble(_E), Convert.ToDouble(_F));
        }
        public Plane CartToPLN(double x_val, double y_val, double z_val, double a_val, double b_val, double c_val)
        {//Default XYZABC of TCP is 890, 0, 1080 0 90 0.
            Plane plane = new Plane(new Point3d(890, 0, 1080), new Vector3d(1, 0, 0), new Vector3d(0, 1, 0));
            plane.Origin = new Point3d(x_val, y_val, z_val);
            plane.Rotate((Math.PI * a_val) / 180.0, plane.ZAxis);
            plane.Rotate((Math.PI * b_val) / 180.0, plane.YAxis);
            plane.Rotate((Math.PI * c_val) / 180.0, plane.XAxis);
            plane.Rotate((Math.PI * -90) / 180.0, plane.YAxis);
            return plane;
        }
        public Plane CartToPLN_Z(double x_val, double y_val, double z_val, double a_val, double b_val, double c_val)
        {
            /*
                KUKAprcCore.Geometry.Plane pln = KUKAprcCore.PRC_Methods.AuxMethods.PRC_CartToPLN(new KUKAprcCore.PRC_Classes.PRC_CartesianVals(
                  x_val, y_val, z_val, a_val, b_val, c_val, 0));
                return new Plane(
                  new Point3d(pln.OriginX, pln.OriginY, pln.OriginZ),
                  new Vector3d((double) pln.XAxis.X, (double) pln.XAxis.Y, (double) pln.XAxis.Z),
                  new Vector3d((double) pln.YAxis.X, (double) pln.YAxis.Y, (double) pln.YAxis.Z));
            */
            Plane plane = new Plane(new Point3d(x_val, y_val, z_val), new Vector3d(1, 0.0, 0), new Vector3d(0.0, 1.0, 0.0));
            plane.Rotate((Math.PI * a_val) / 180.0, plane.ZAxis);
            plane.Rotate((Math.PI * b_val) / 180.0, plane.YAxis);
            plane.Rotate((Math.PI * c_val) / 180.0, plane.XAxis);
            return plane;
        }
        public List<double> PLNtoXYZABC(Plane plnin, bool useXAxis)
        {
            Plane plane = plnin;
            List<double> output = new List<double>();
            output.Add(plane.OriginX);
            output.Add(plane.OriginY);
            output.Add(plane.OriginZ);
            Vector3d vectord = plane.XAxis; vectord.Unitize();
            Vector3d vectord2 = plane.YAxis; vectord2.Unitize();
            Vector3d vectord3 = plane.ZAxis; vectord3.Unitize();
            double y = vectord3.Y;
            double z = vectord3.Z;
            double num3 = vectord2.Z;
            double num4 = vectord.Y;
            double x = vectord.X;
            double d = -270.0 + (57.295779513082323 * Math.Acos((y * -1.0) / Math.Sqrt(1.0 - z * z)));
            double num7 = 90.0 - (57.295779513082323 * Math.Acos(z));
            double num8 = 90.0 - (57.295779513082323 * Math.Acos(num3 / Math.Sqrt(1.0 - z * z)));
            double num9 = num3 / Math.Sqrt(1.0 - z * z);
            double num10 = (y * -1.0) / Math.Sqrt(1.0 - z * z);
            if (vectord3.X <= 0.0) { d = -180.0 - d; }
            if (vectord.Z < 0.0) { num8 = 180.0 - num8; }
            if ((z == 1.0) | (z == -1.0))
            {
                if (z > 0.0) { num8 = 57.295779513082323 * Math.Atan2(-num4, x); }
                else if (z < 0.0) { num8 = -57.295779513082323 * Math.Atan2(num4, -x); }
            }
            if (double.IsNaN(num8) & double.IsNaN(d))
            {
                if (num10 >= 1.0) { d = 90.0; }
                else if (num10 <= -1.0) { d = -90.0; }
                if (num9 >= 1.0) { num8 = 90.0; }
                else if (num9 <= -1.0) { num8 = -90.0; }
            }
            if (double.IsNaN(d))
            {
                if (num10 >= 1.0) { d = 90.0; }
                else if (num10 <= -1.0) { d = -90.0; }
                else { d = 0.0; }
            }
            if (double.IsNaN(num8))
            {
                if (num9 >= 1.0) { num8 = 90.0; }
                else { num8 = -90.0; }
            }
            if (d < -180.0) { d += 360.0; }
            if (num8 > 180.0) { num8 -= 360.0; }
            output.Add(d);
            output.Add(num7);
            output.Add(num8);
            return output;
        }
    }
}
