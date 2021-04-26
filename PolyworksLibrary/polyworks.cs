using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyworksLibrary
{
    public class polyworks
    {
        public string PrintMatric(Transform Matrix)
        {
            string str = "VERSION = 1" + "\r\n";
            str += "MATRIX =" + "\r\n";
            str += Matrix.M00.ToString() + "   " + Matrix.M00.ToString() + "   " + Matrix.M00.ToString() + "   " + Matrix.M00.ToString() + "\r\n";
            str += Matrix.M00.ToString() + "   " + Matrix.M00.ToString() + "   " + Matrix.M00.ToString() + "   " + Matrix.M00.ToString() + "\r\n";
            str += Matrix.M00.ToString() + "   " + Matrix.M00.ToString() + "   " + Matrix.M00.ToString() + "   " + Matrix.M00.ToString() + "\r\n";
            str += Matrix.M00.ToString() + "   " + Matrix.M00.ToString() + "   " + Matrix.M00.ToString() + "   " + Matrix.M00.ToString();
            return str;
        }
        public string ReadMatric(string path ,out Transform xform )
        {
            xform = Transform.Identity;
            List<string> data = new List<string>();
            if (!File.Exists(path)) return "null file";
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open);
                StreamReader srd = new StreamReader(fs);
                while (srd.Peek() != -1)
                {
                    string str = srd.ReadLine();
                    string[] chara = str.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (chara.Length ==4)
                    {
                        data.AddRange(chara);
                    }
                }
                srd.Close();
                xform.M00 = Convert.ToDouble(data[0]);
                xform.M01 = Convert.ToDouble(data[1]);
                xform.M02 = Convert.ToDouble(data[2]);
                xform.M03 = Convert.ToDouble(data[3]);

                xform.M10 = Convert.ToDouble(data[4]);
                xform.M11 = Convert.ToDouble(data[5]);
                xform.M12 = Convert.ToDouble(data[6]);
                xform.M13 = Convert.ToDouble(data[7]);

                xform.M20 = Convert.ToDouble(data[8]);
                xform.M21 = Convert.ToDouble(data[9]);
                xform.M22 = Convert.ToDouble(data[10]);
                xform.M23 = Convert.ToDouble(data[11]);

                xform.M30 = Convert.ToDouble(data[12]);
                xform.M31 = Convert.ToDouble(data[13]);
                xform.M32 = Convert.ToDouble(data[14]);
                xform.M33 = Convert.ToDouble(data[15]);
            }
            catch (Exception ex)
            {
                return (ex.ToString());
            }
            return "Success!";
        }
    }
}
