using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace EffectPlayer
{
    public partial class Form1 : Form
    {
        class SelectItem
        {
            public string Path;
            public string Image;
        }

        delegate void SetSelectCallback();
        List<Frame> frames = new List<Frame>();
        System.Threading.Timer t;
        String path = "";

        List<SelectItem> items = new List<SelectItem>();
        private int drawSize = 60;
        private int target = -1;

        public Form1()
        {
            InitializeComponent();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            if (listBox1.SelectedIndex < frames.Count) {
                for (int i = 0; i < frames[listBox1.SelectedIndex].Units.Length; i++)
                {
                    ListViewItem lv = new ListViewItem(frames[listBox1.SelectedIndex].Units[i].frameid.ToString());
                    lv.SubItems.Add(frames[listBox1.SelectedIndex].Units[i].x.ToString());
                    lv.SubItems.Add(frames[listBox1.SelectedIndex].Units[i].y.ToString());
                    lv.SubItems.Add(frames[listBox1.SelectedIndex].Units[i].width.ToString());
                    lv.SubItems.Add(frames[listBox1.SelectedIndex].Units[i].height.ToString());
                    listView1.Items.Add(lv);
                }
            }
            panel1.Invalidate();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (listBox1.SelectedIndex < frames.Count && listBox1.SelectedIndex >= 0)
            {
                for (int i = 0; i < frames[listBox1.SelectedIndex].Units.Length; i++)
                {
                    FrameUnit fu = frames[listBox1.SelectedIndex].Units[i];
                    try
                    {
                        Image image = Image.FromFile(String.Format(@"../../PicResource/Effect/{0}.PNG", fu.frameid));
                        if(fu.parm!=0)
                        {
                            switch (fu.parm)
                            {
                                case 1: image.RotateFlip(RotateFlipType.Rotate90FlipNone); break;
                                case 2: image.RotateFlip(RotateFlipType.Rotate180FlipNone); break;
                                case 3: image.RotateFlip(RotateFlipType.Rotate270FlipNone); break;
                                case 4: image.RotateFlip(RotateFlipType.RotateNoneFlipX); break;
                                case 5: image.RotateFlip(RotateFlipType.Rotate90FlipX); break;
                                case 6: image.RotateFlip(RotateFlipType.Rotate180FlipX); break;
                                case 7: image.RotateFlip(RotateFlipType.Rotate270FlipX); break;
                                default: ImagePixelTool.Effect((Bitmap)image, (ImagePixelEffects)(fu.parm / 10), fu.parm % 10); break;
                            }

                        }
                        e.Graphics.DrawImage(image, fu.x, fu.y, fu.width, fu.height);
                        image.Dispose();
                    }
                    catch { }
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (t==null)
            {
                t = new System.Threading.Timer(new TimerCallback(timerDo), null, 100, 100);
            }
        }

        private void timerDo(object o) {

            SetSelect();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (t != null)
            {
                t.Dispose();
                t = null;
            }
        }

        private void SetSelect()
        {
            if (listBox1.InvokeRequired)
            {
                SetSelectCallback d = new SetSelectCallback(SetSelect);
                this.Invoke(d, new object[] {  });
            }
            else
            {
                int p = listBox1.SelectedIndex + 1;
                if (p >= listBox1.Items.Count)
                    p = 0;
                listBox1.SelectedIndex = p;
            }
        }

        private void listBoxFiles_SelectedIndexChanged()
        {
            if (target >= 0 && items.Count > target)
            {
                StreamReader sr = new StreamReader(path + "/" + items[target].Path);
                sr.ReadLine();
                int frameCount = int.Parse(sr.ReadLine());
                frames.Clear();
                listBox1.Items.Clear();
                for (int i = 0; i < frameCount; i++)
                {
                    int frameUnitCount = int.Parse(sr.ReadLine());
                    Frame frame = new Frame();
                    frame.Units = new FrameUnit[frameUnitCount];
                    listBox1.Items.Add(listBox1.Items.Count + 1);
                    for (int j = 0; j < frameUnitCount; j++)
                    {
                        String read = sr.ReadLine();
                        String[] arrays = read.Split('\t');
                        FrameUnit fu = new FrameUnit();
                        fu.frameid = int.Parse(arrays[0]);
                        fu.x = int.Parse(arrays[1]);
                        fu.y = int.Parse(arrays[2]);
                        fu.width = int.Parse(arrays[3]);
                        fu.height = int.Parse(arrays[4]);
                        if (arrays.Length >= 6)
                        {
                            fu.parm = int.Parse(arrays[5]);
                        }
                        frame.Units[j] = fu;
                    }
                    frames.Add(frame);
                }
                sr.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (target >= 0 && items.Count > target)
            {
                string fname = path + "/" + items[target].Path;
                Process.Start("notepad", fname);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            String version = FileVersionInfo.GetVersionInfo(System.Windows.Forms.Application.ExecutablePath).FileVersion;
            Text = String.Format("特效查看器 v{0}", version);

            path = @"../../DataResource\Effect";
            items.Clear();
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                if (file.Extension == ".eff")
                {
                    var itm = new SelectItem();
                    itm.Path = file.Name;
                    StreamReader sr = new StreamReader(path + "/" + itm.Path);
                    sr.ReadLine();
                    sr.ReadLine();
                    sr.ReadLine();
                    itm.Image = sr.ReadLine().Split('\t')[0];
                    sr.Close();
                    items.Add(itm);
                }
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            int xoff = 0, yoff = 0;
            int index = 0;
            foreach (var selectItem in items)
            {
                if (index == target)
                {
                    Brush sb = new SolidBrush(Color.FromArgb(100, Color.Yellow));
                    e.Graphics.FillRectangle(sb, xoff, yoff, drawSize, drawSize);
                    sb.Dispose();
                }

                e.Graphics.DrawRectangle(Pens.White, xoff, yoff, drawSize, drawSize);
                Image image = Image.FromFile(String.Format(@"../../PicResource/Effect/{0}.PNG", selectItem.Image));
                e.Graphics.DrawImage(image, xoff, yoff, drawSize, drawSize);
                image.Dispose();

                Font ft = new Font("宋体", 9);
                e.Graphics.DrawString(selectItem.Path.Replace(".eff", ""), ft, Brushes.White, xoff, yoff);
                ft.Dispose();

                xoff += drawSize;
                if (xoff > panel2.Width- drawSize)
                {
                    xoff = 0;
                    yoff += drawSize;
                }
                index++;
            }
        }

        private void panel2_MouseClick(object sender, MouseEventArgs e)
        {
            target = e.X/drawSize + (e.Y/drawSize)*(panel2.Width/drawSize);
            if (target >= 0 && items.Count > target)
            {
                label1.Text = items[target].Path;
            }
            listBoxFiles_SelectedIndexChanged();
            panel2.Invalidate();
        }
    }
}