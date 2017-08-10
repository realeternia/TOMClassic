using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BattleSceneEditor
{
    public partial class Form1 : Form
    {
        private string path;

        private BattleMap map;
        private List<BattleMapUnitInfo> addonUnits = new List<BattleMapUnitInfo>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            LoadFromFile(path);
        }

        private void LoadFromFile(String txt)
        {
            path = txt;
            try
            {
                map = BattleMap.GetMapFromFile(txt);
                Invalidate();
            }
            catch (Exception e)
            {
                MessageBox.Show("错误的文件格式" + e, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (map.XCount <= 0)
                return;

            int cellWidth = 900/map.XCount;
            int cellHeight = 400 / map.YCount;

            Font ft = new Font("宋体", 9);
            for (int i = 0; i < map.XCount; i++)
            {
                for (int j = 0; j < map.YCount; j++)
                {
                    var cell = map.Cells[i, j];
                    e.Graphics.FillRectangle(cell == 9 ? Brushes.Green : Brushes.Black, i* cellWidth, j * cellHeight, cellWidth, cellHeight);
                    e.Graphics.DrawRectangle(Pens.DarkRed, i * cellWidth, j * cellHeight, cellWidth, cellHeight);
                    e.Graphics.DrawString(string.Format("{0}:{1}",i,j), ft, Brushes.DarkGray, i * cellWidth, j * cellHeight);
                }
            }
            ft.Dispose();

            string pathParent = "../../PicResource/";
            foreach (var unit in map.LeftUnits)
            {
                var img = Image.FromFile(string.Format("{0}Monsters/{1}.JPG", pathParent, 10002));
                e.Graphics.DrawImage(img, unit.X * cellWidth, unit.Y * cellHeight, cellWidth, cellHeight);
                img.Dispose();
            }
            foreach (var unit in map.RightUnits)
            {
                var img = Image.FromFile(string.Format("{0}Monsters/{1}.JPG", pathParent, 10002));
                e.Graphics.DrawImage(img, unit.X * cellWidth, unit.Y * cellHeight, cellWidth, cellHeight);
                img.Dispose();
            }
            foreach (var unit in addonUnits)
            {
                var img = Image.FromFile(string.Format("{0}Monsters/{1}.JPG", pathParent, unit.UnitId % 1000000));
                e.Graphics.DrawImage(img, unit.X * cellWidth, unit.Y * cellHeight, cellWidth, cellHeight);
                img.Dispose();

                var p = new Pen(unit.Color);
                e.Graphics.DrawRectangle(p, unit.X * cellWidth, unit.Y * cellHeight, cellWidth, cellHeight);
                p.Dispose();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            addonUnits.Clear();
            try
            {
                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    var items = textBox1.Text.Split(';');
                    for (int i = 0; i < items.Length; i+=3)
                    {
                        var data = new BattleMapUnitInfo();
                        data.UnitId = int.Parse(items[i]);
                        data.X = int.Parse(items[i+1]);
                        data.Y = int.Parse(items[i+2]);
                        data.Color = Color.Red;
                        addonUnits.Add(data);
                    }
                }
                if (!string.IsNullOrEmpty(textBox2.Text))
                {
                    var items = textBox2.Text.Split(';');
                    for (int i = 0; i < items.Length; i += 3)
                    {
                        var data = new BattleMapUnitInfo();
                        data.UnitId = int.Parse(items[i]);
                        data.X = int.Parse(items[i + 1]);
                        data.Y = int.Parse(items[i + 2]);
                        data.Color = Color.Blue;
                        addonUnits.Add(data);
                    }
                }
                if (!string.IsNullOrEmpty(textBox3.Text))
                {
                    var items = textBox3.Text.Split(';');
                    for (int i = 0; i < items.Length; i += 3)
                    {
                        var data = new BattleMapUnitInfo();
                        data.UnitId = int.Parse(items[i]);
                        data.X = int.Parse(items[i + 1]);
                        data.Y = int.Parse(items[i + 2]);
                        data.Color = Color.Yellow;
                        addonUnits.Add(data);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误的文件格式" + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Invalidate();
        }
    }
}
