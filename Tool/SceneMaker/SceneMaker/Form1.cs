using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace SceneMaker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            AllowDrop = true;
            DoubleBuffered = true;
            Width = 1152;
            Height = 720;
            Scene.Instance = new Scene(this, 1152 - 15, 720 - 35);
       //     Scene.Instance.ChangeMap("./Scene/default.txt", true);
       //     Scene.Instance.ChangeBg("./Scene/default.JPG");
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Scene.Instance.Paint(e.Graphics, 0);
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            FileInfo fi = new FileInfo(path);
            try
            {
                if (fi.Extension.ToLower() == ".jpg")
                {
                    Scene.Instance.ChangeBg(path);
                }
                else if (fi.Extension.ToLower() == ".txt")
                {
                    Scene.Instance.ChangeMap(path, true);
                }
                else
                {
                    throw new Exception("未知文件格式");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(path + " " + ex);
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            String version = FileVersionInfo.GetVersionInfo(System.Windows.Forms.Application.ExecutablePath).FileVersion;
            Text = String.Format("场景编辑器 v{0}", version);
        }
    }
}
