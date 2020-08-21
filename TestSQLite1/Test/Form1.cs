using BIMLab.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestSQLite1
{
    public partial class Form1 : Form
    {
 
        public Form1()
        {
            InitializeComponent();

            string filepath = "D:\\Revit" + "\\testtable.db";
            string dbfile="Data Source=" + filepath + ";Pooling=true;FailIfMissing=false";
            SQLiteHelper2.SetConnectionString(dbfile);//设定dbfile
            string commandText = "select username, password from table1";
            DataSet ds = SQLiteHelper2.ExecuteDataSet(commandText, null);//使用SQLite指令
            DataTable dt = ds.Tables[0];
            #region test///////////////////////////////
            string output = string.Empty;
            textBox2.Text = ds.Tables[0].Rows[0].ItemArray.Length.ToString();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                output += ds.Tables[0].Rows[i].ItemArray[0].ToString() + " "+
                    ds.Tables[0].Rows[i].ItemArray[1].ToString() + "\r\n";
            }
            this.textBox1.Text = output;
            #endregion ///////////////////////        
            TreeNode node = new TreeNode("username");
            node.Tag = "0";
            foreach (DataRow row in dt.Rows)
            {
                TreeNode node1 = new TreeNode(row[0].ToString());
                node1.Tag = "01";
                node.Nodes.Add(node1);//添加子节点
            }
            node.Expand();//展开状态
                this.treeView1.Nodes.Add(node);
        }

      

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //选择treeView里面的选项后发生的事件
            string filepath = "D:\\Revit" + "\\testtable.db";
            string dbfile = "Data Source=" + filepath + ";Pooling=true;FailIfMissing=false";
            SQLiteHelper2.SetConnectionString(dbfile);//设定dbfile

            string code = this.treeView1.SelectedNode.Text.ToString();//选中的节点
            textBox2.Text = code;

            string commandText = "select username, password from table1 where username = '"+code+"'";
            DataSet ds = SQLiteHelper2.ExecuteDataSet(commandText, null);//使用SQLite指令
            DataTable dt = ds.Tables[0];
            this.dataGridView1.DataSource = dt;
        }

     
    }
}
