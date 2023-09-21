using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UpdateSystemInfo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> sysInfo = new Dictionary<string, string>() { { "SystemName", textBox1.Text } , { "Project", textBox2.Text },
            { "FV1", textBox3.Text }, { "FV2", textBox4.Text }, { "TSM", textBox5.Text }};
            
            List<string> stringlist = new List<string>();
            string FilePath = @"F:\IPLabInstallation\SQUIREIP.txt";
            string[] configFile = File.ReadAllLines(FilePath);
            string key, value;
            foreach (string line in configFile)
            {
                string[] linesp = line.Split(':');
                key = linesp[0].Trim();
                
                foreach (KeyValuePair<string, string> item in sysInfo)
                {
                    if (item.Key.Contains(key))
                    {
                        value = linesp[1].Trim();
                        stringlist.Add(Regex.Replace(line, value, item.Value));
                        break;
                    }
                }
            }

            File.WriteAllLines(FilePath, stringlist);
            Application.Exit();
        }
    }
}
