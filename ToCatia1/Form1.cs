using System;
using System.Windows.Forms;

namespace ToCatia1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ///* 
            ///
            CATIA_VBA catia = new CATIA_VBA();
           string refname = "PC";
            string refpath = "C:\\Users\\Administrator\\Desktop\\catia\\tutorial\\triangleFrame2.CATPart";
            string[] inputpt = { "Point.1", "Point.2", "Point.3" };
            string[] refpoint = { "Point.1", "Point.2", "Point.3" };
            catia.AddPowerCopyNPoint(refname, refpath, inputpt, refpoint);
            //*/

        }


      
    }
}
