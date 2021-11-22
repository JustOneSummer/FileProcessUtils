using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileProcessUtils
{
    public partial class FormFileNameRe : Form
    {
        private Form1 form;
        public FormFileNameRe(Form1 form)
        {
            InitializeComponent();
            this.form = form;
        }
        public DataTable table = new DataTable();
        public Dictionary<int, FileInfo> SELECT_MAP = new Dictionary<int, FileInfo>();

        /// <summary>
        /// 初始化加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormFileNameRe_Load(object sender, EventArgs e)
        {
            table.Columns.Add(new DataColumn("序号"));
            table.Columns.Add(new DataColumn("文件名"));
            table.Columns.Add(new DataColumn("新文件名"));
            this.dataGridView1.DataSource = table;
            this.dataGridView1.Columns[0].ReadOnly = true;
            this.dataGridView1.Columns[1].ReadOnly = true;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            int di = 1;
            foreach (FileInfo info in Form1.SELECT_LIST)
            {
                if (info.Exists)
                {
                    SELECT_MAP.Add(di,info);
                    DataRow dr = table.NewRow();
                    string name = info.Name;
                    if (name.LastIndexOf(".") <= -1)
                    {
                        continue;
                    }
                    dr[0] = di;
                    dr[1] = name.Substring(0, name.LastIndexOf("."));
                    dr[2] = "";
                    table.Rows.Add(dr);
                    di++;
                }
            }
            this.dataGridView1.DataSource = table;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 开始修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //首先进行第一次循环 获取选中的行数
            for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            {
                string index = this.dataGridView1.Rows[i].Cells[0].Value.ToString();
                string name = this.dataGridView1.Rows[i].Cells[1].Value.ToString();
                string newName = this.dataGridView1.Rows[i].Cells[2].Value.ToString();
                if (!string.IsNullOrEmpty(newName))
                {
                    FileInfo info;
                    SELECT_MAP.TryGetValue(int.Parse(index), out info);
                    //改名
                    string fullName = info.FullName;
                    string pathNew = fullName.Substring(0, fullName.LastIndexOf("\\"));
                    newName = newName+info.Name.Substring(info.Name.LastIndexOf("."));
                    Directory.Move(fullName, pathNew + "/" + newName);
                }
            }
            MessageBox.Show("修改完成");
            this.form.Query("");
            this.Close();
        }
    }
}
