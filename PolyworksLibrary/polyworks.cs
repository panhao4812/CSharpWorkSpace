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
        public static string PrintMatric(Transform Matrix)
        {
            string str = "VERSION = 1" + "\r\n";
            str += "MATRIX =" + "\r\n";
            str += Matrix.M00.ToString() + "   " + Matrix.M00.ToString() + "   " + Matrix.M00.ToString() + "   " + Matrix.M00.ToString() + "\r\n";
            str += Matrix.M00.ToString() + "   " + Matrix.M00.ToString() + "   " + Matrix.M00.ToString() + "   " + Matrix.M00.ToString() + "\r\n";
            str += Matrix.M00.ToString() + "   " + Matrix.M00.ToString() + "   " + Matrix.M00.ToString() + "   " + Matrix.M00.ToString() + "\r\n";
            str += Matrix.M00.ToString() + "   " + Matrix.M00.ToString() + "   " + Matrix.M00.ToString() + "   " + Matrix.M00.ToString();
            return str;
        }
        public static string ReadMatric(string path ,out Transform xform )
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
    public class PolyworksMatrixTools
    {
        public PolyworksMatrixTools() { }
        public string DisPlayMatrix(double[] db)
        {
            string str = "";
            if (db.Length != 16) return str;
            str += db[0].ToString() + " " + db[1].ToString() + " " + db[2].ToString() + " " + db[3].ToString() + "\r\n";
            str += db[4].ToString() + " " + db[5].ToString() + " " + db[6].ToString() + " " + db[7].ToString() + "\r\n";
            str += db[8].ToString() + " " + db[9].ToString() + " " + db[10].ToString() + " " + db[11].ToString() + "\r\n";
            str += db[12].ToString() + " " + db[13].ToString() + " " + db[14].ToString() + " " + db[15].ToString();
            return str;
        }
        public string DisPlayMatrix(List<double> db)
        {
            return DisPlayMatrix(db.ToArray());
        }
        private double FileGetDouble(int index, System.Byte[] view)
        {
            System.Byte[] num1 = new System.Byte[16];
            char[] chs = new Char[16];
            for (int i = 0; i < 16; i++)
            {
                num1[i] = view[index + i];
            }
            chs = Encoding.UTF8.GetChars(num1);
            string str = new string(chs);
            return Convert.ToDouble(str);
        }
        private List<double> DataFromLine(string str)
        {
            List<double> output = new List<double>();
            Byte[] data1 = Encoding.UTF8.GetBytes(str);
            output.Add(FileGetDouble(0, data1));
            output.Add(FileGetDouble(0x0010 + 2, data1));
            output.Add(FileGetDouble(0x0020 + 4, data1));
            output.Add(FileGetDouble(0x0030 + 6, data1));
            return output;
        }
        public bool ReadMatrix(string path, out List<double> output)
        {
            output = new List<double>();
            try
            {
                StreamReader stream = new StreamReader(path, Encoding.UTF8);
                string str = "";
                while ((str = stream.ReadLine()) != null)
                {
                    if (str == "MATRIX	=")
                    {
                        List<double> data2 = new List<double>();
                        string data1 = stream.ReadLine();
                        data2.AddRange(DataFromLine(data1));
                        data1 = stream.ReadLine();
                        data2.AddRange(DataFromLine(data1));
                        data1 = stream.ReadLine();
                        data2.AddRange(DataFromLine(data1));
                        data1 = stream.ReadLine();
                        data2.AddRange(DataFromLine(data1));
                        output = data2;
                        break;
                    }
                }
                stream.Close();
                return true;
            }
            catch { return false; }

        }
        public bool ReadMatrix(string path, out Transform output)
        {
            bool rc;
            List<double> db;
            rc = ReadMatrix(path, out db);
            if (rc) { output = ReadMatrix(db); } else { output = new Transform(); }
            return rc;
        }
        public Transform ReadMatrix(List<double> db)
        {
            Transform tr = new Transform();
            tr.M00 = db[0]; tr.M01 = db[1]; tr.M02 = db[2]; tr.M03 = db[3];
            tr.M10 = db[4]; tr.M11 = db[5]; tr.M12 = db[6]; tr.M13 = db[7];
            tr.M20 = db[8]; tr.M21 = db[9]; tr.M22 = db[10]; tr.M23 = db[11];
            tr.M30 = db[12]; tr.M31 = db[13]; tr.M32 = db[14]; tr.M33 = db[15];
            return tr;
        }
        private string double2string9(double t)
        {
            string str = "-";
            if (t >= 0) { str = " "; }
            t = Math.Abs(t);
            string num1 = t.ToString("#.########e+000");
            if (num1.Length < 15)
            {
                double n2 = 15 - num1.Length;
                for (int i = 0; i < n2; i++)
                {
                    num1 = num1.Insert(num1.Length - 5, "0");
                }
            }
            str += num1;
            return str;
        }
        public string WriteMatrix(List<double> db)
        {
            return WriteMatrix(db.ToArray());
        }
        public string WriteMatrix(double[] db)
        {
            string str = "";
            str += double2string9(db[0]) + "  ";
            str += double2string9(db[1]) + "  ";
            str += double2string9(db[2]) + "  ";
            str += double2string9(db[3]) + "\r\n";
            str += double2string9(db[4]) + "  ";
            str += double2string9(db[5]) + "  ";
            str += double2string9(db[6]) + "  ";
            str += double2string9(db[7]) + "\r\n";
            str += double2string9(db[8]) + "  ";
            str += double2string9(db[9]) + "  ";
            str += double2string9(db[10]) + "  ";
            str += double2string9(db[11]) + "\r\n";
            str += double2string9(db[12]) + "  ";
            str += double2string9(db[13]) + "  ";
            str += double2string9(db[14]) + "  ";
            str += double2string9(db[15]);
            return str;
        }
        public string WriteXform(double[] db)
        {
            string str = "VERSION	=	1" + "\r\n";
            str += "MATRIX	=" + "\r\n";
            str += WriteMatrix(db);
            return str;
        }
        public string WriteXform(List<double> db)
        {
            return WriteXform(db.ToArray());
        }
        public string WriteView(List<double> db, double factor)
        {
            return WriteView(db.ToArray(), factor);
        }
        public string WriteView(double[] db, double factor)
        {
            string str = "VERSION	=	2" + "\r\n";
            str += "TYPE	=	Orthogonal";
            str += "SCALING_FACTOR	=	" + factor.ToString() + "\r\n";
            str += "MATRIX	=" + "\r\n";
            str += WriteMatrix(db);
            return str;
        }
    }
}
