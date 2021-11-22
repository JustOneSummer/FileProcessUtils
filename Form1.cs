using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace FileProcessUtils
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static string CONFIG_PATH = ConfigUtils.GetFileConfig();
        public static DataTable table = new DataTable();
        public static Dictionary<int, FileInfo> SELECT_MAP = new Dictionary<int, FileInfo>();
        public static List<FileInfo> SELECT_LIST = new List<FileInfo>();


        private void 设定管理目录ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        /// <summary>
        /// 查询文件信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSearch_Click(object sender, EventArgs e)
        {
            Query(this.textBoxSearch.Text.Trim());
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="queryName"></param>
        public void Query(string queryName)
        {
            DirectoryInfo info = new DirectoryInfo(CONFIG_PATH);
            if (string.IsNullOrEmpty(queryName))
            {
                queryName = "*";
            }
            else
            {
                queryName = "*" + queryName + "*";
            }

            FileInfo[] fileInfos = info.GetFiles(queryName, SearchOption.AllDirectories);
            if (fileInfos.Length <= 0)
            {
                MessageBox.Show("未找到:" + queryName);
            }
            else
            {
                //载入数据表
                dataViewLoad(fileInfos);
            }
        }

        /// <summary>
        /// 初始化加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            DataGridViewColumn checkCol = new DataGridViewCheckBoxColumn();
            checkCol.Name = "选择";
            this.dataGridViewTable.Columns.Add(checkCol);
            table.Columns.Add(new DataColumn("序号"));
            table.Columns.Add(new DataColumn("文件名"));
            table.Columns.Add(new DataColumn("文件类型"));
            table.Columns.Add(new DataColumn("修改日期"));
            table.Columns.Add(new DataColumn("路径全名称"));
            this.dataGridViewTable.DataSource = table;
            this.dataGridViewTable.Columns[1].ReadOnly = true;
            this.dataGridViewTable.Columns[2].ReadOnly = true;
            this.dataGridViewTable.Columns[3].ReadOnly = true;
            this.dataGridViewTable.Columns[4].ReadOnly = true;
            this.dataGridViewTable.Columns[5].ReadOnly = true;
            this.dataGridViewTable.AllowUserToAddRows = false;
            this.dataGridViewTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Query("");
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        /// <param name="infos"></param>
        public void dataViewLoad(FileInfo[] infos)
        {
            table.Clear();
            SELECT_MAP.Clear();
            //加载文件信息
            int di = 1;
            foreach (FileInfo info in infos)
            {
                if (info.Exists)
                {
                    DataRow dr = table.NewRow();
                    string name = info.Name;
                    if (name.LastIndexOf(".") <= -1)
                    {
                        continue;
                    }
                    dr[0] = di;
                    dr[1] = name.Substring(0,name.LastIndexOf("."));
                    dr[2] = name.Substring(name.LastIndexOf("."));
                    dr[3] = info.CreationTime.ToString("yyyy-MM-dd HH:mm:ss");
                    dr[4] = info.FullName;
                    table.Rows.Add(dr);
                    SELECT_MAP.Add(di,info);
                    di++;
                }
            }
            this.dataGridViewTable.DataSource = table;
        }

        /// <summary>
        /// 获取选中的信息
        /// </summary>
        public void selectDataView()
        {
            SELECT_LIST.Clear();
            //首先进行第一次循环 获取选中的行数
            for (int i = 0; i < this.dataGridViewTable.Rows.Count; i++)
            {
                //如果被选中
                if ((bool)this.dataGridViewTable.Rows[i].Cells[0].EditedFormattedValue == true)
                {
                  string s =  this.dataGridViewTable.Rows[i].Cells[1].Value.ToString();
                  FileInfo info = null;
                  SELECT_MAP.TryGetValue(int.Parse(s),out info);
                  SELECT_LIST.Add(info);
                }
            }
        }

        /// <summary>
        /// 双击时设定管理目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 设定管理目录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dilog = new FolderBrowserDialog();
            dilog.Description = "请选择文件夹";
            if (dilog.ShowDialog() == DialogResult.OK || dilog.ShowDialog() == DialogResult.Yes)
            {
                ConfigUtils.SetConfig(dilog.SelectedPath);
            }
            CONFIG_PATH = ConfigUtils.GetFileConfig();
            Query("");
        }

        /// <summary>
        /// 导出文件，显示选择的文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDaoChuWenJian_Click(object sender, EventArgs e)
        {
            selectDataView();
            if (SELECT_LIST.Count <= 0)
            {
                MessageBox.Show("请选择文件");
                return;
            }
            StringBuilder builder = new StringBuilder("将导出以下文件\r\n");
            foreach(FileInfo info in SELECT_LIST)
            {
                builder.Append(info.Name).Append("\r\n");
            }
            DialogResult dialogResult = MessageBox.Show(builder.ToString(), "确认框", MessageBoxButtons.OKCancel, MessageBoxIcon.None);
            if(dialogResult == DialogResult.OK)
            {
                FolderBrowserDialog dilog = new FolderBrowserDialog();
                dilog.Description = "请选择文件夹";
                if (dilog.ShowDialog() == DialogResult.OK || dilog.ShowDialog() == DialogResult.Yes)
                {
                   string p = dilog.SelectedPath;
                   foreach(FileInfo info in SELECT_LIST)
                    {
                        File.Copy(info.FullName,p+"/"+info.Name,false);
                    }
                    MessageBox.Show("导出完成");
                }
            }
        }

        private void buttonReName_Click(object sender, EventArgs e)
        {
            selectDataView();
            if (SELECT_LIST.Count <= 0)
            {
                MessageBox.Show("请选择文件");
                return;
            }
            //打开新窗口修改
            FormFileNameRe re = new FormFileNameRe(this);
            re.ShowDialog();
        }

        private void 导入excelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("功能暂时未完成");
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("西行寺雨季制作\r\nhttps://shinoaki.com");
        }
    }
}
