using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace DeckManager
{
    public partial class Form1 : Form
    {
        CardDescript[] cd = new CardDescript[30];
        Image[] images = new Image[30];
        string path;
        public Form1(string path)
        {
            this.path = path;
            InitializeComponent();
        }

        private void LoadFromFile(String txt)
        {
            try
            {
                StreamReader sr = new StreamReader(txt);
                for (int i = 0; i < 30; i++)
                {
                    string[] datas = sr.ReadLine().Split('\t');
                    cd[i] = new CardDescript(int.Parse(datas[0]), int.Parse(datas[1]), int.Parse(datas[2]));
                }
                sr.Close();
                UpdateImages();
            }
            catch (Exception e)
            {
                MessageBox.Show("错误的文件格式"+e, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void UpdateImages()
        {
            for (int i = 0; i < 30; i++)
            {
                int cardId = cd[i].id;
                if (cardId < 52000000)
                    images[i] = Image.FromFile(String.Format("{0}/Monsters/{1}.JPG", System.Windows.Forms.Application.StartupPath, cd[i].id%1000000));
                else if (cardId < 53000000)
                    images[i] = Image.FromFile(String.Format("{0}/Weapon/{1}.JPG", System.Windows.Forms.Application.StartupPath, cd[i].id % 1000000));
                else
                    images[i] = Image.FromFile(String.Format("{0}/Spell/{1}.JPG", System.Windows.Forms.Application.StartupPath, cd[i].id % 1000000));
            }
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (images[29] != null)
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (cd[j * 6 + i].level != 0)
                        {
                            e.Graphics.DrawImage(images[j * 6 + i], i * 60, j * 60, 60, 60);
                        }
                    }
                }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.InitialDirectory = @"J:\Program\TaleofMonsters\DataResource\Deck";
            opf.Filter = "dek files (*.dek)|*.dek|All files (*.*)|*.*";
            opf.FilterIndex = 0;
            if (opf.ShowDialog() == DialogResult.OK)
            {
                path = opf.FileName;
                LoadFromFile(opf.FileName);
                panel1.Invalidate();
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            String version = FileVersionInfo.GetVersionInfo(System.Windows.Forms.Application.ExecutablePath).FileVersion;
            Text = String.Format("卡片组织器 v{0}", version);
            if (path != "null")
                LoadFromFile(path);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            LoadFromFile(path);
            panel1.Invalidate();
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            Process.Start("notepad.exe", path);
        }
    }
}