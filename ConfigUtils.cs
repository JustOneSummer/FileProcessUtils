using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace FileProcessUtils
{
    class ConfigUtils
    {
        /// <summary>
        /// 获取配置文件目录
        /// </summary>
        /// <returns></returns>
        public static String GetFileConfig()
        {
            string path = System.Environment.CurrentDirectory + "/directory.txt";
            if(!new FileInfo(path).Exists)
            {
                SetConfig("");
            }
            StreamReader reader = new StreamReader(path);
            string value = reader.ReadLine();
            reader.Close();
            if (string.IsNullOrEmpty(value))
            {
                MessageBox.Show("请点击菜单的编辑下面的设定管理目录设定文件夹目录!");
                return System.Environment.CurrentDirectory;
            }
            return value.Trim();
        }

        /// <summary>
        /// 设定管理目录
        /// </summary>
        /// <param name="path"></param>
        public static void SetConfig(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = System.Environment.CurrentDirectory;
            }
            string pathFile = System.Environment.CurrentDirectory + "/directory.txt";
            StreamWriter writerName = new StreamWriter(pathFile);
            writerName.WriteLine(path);
            writerName.Flush();
            writerName.Close();
        }
    }
}
