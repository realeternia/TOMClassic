using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace VFSViewer
{
    public partial class Form1 : Form
    {
        private Image cachImg;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var targetFile = @"F:\TOMClassic\Trunk\bin\Debug\PicResource.vfs";
            NLVFS.NLVFS.LoadVfsFile(targetFile);
            foreach (var path in NLVFS.NLVFS.GetPathList())
            {
                listBox1.Items.Add(path);
            }
            Text = string.Format("总计条目 {0}", listBox1.Items.Count);
            listBox1.SelectedIndex = 0;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            MemoryStream ms = new MemoryStream(NLVFS.NLVFS.LoadFile(listBox1.SelectedItem.ToString()));
            cachImg = System.Drawing.Image.FromStream(ms);
            splitContainer1.Panel2.Invalidate();
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {
            if (cachImg != null)
            {
                e.Graphics.DrawImageUnscaled(cachImg, 0, 0);

            }
        }

    }
}
