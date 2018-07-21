using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BattleSceneEditor
{
    public partial class Form1 : Form
    {
        private string path;

        private BattleMapInfo map;
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
                map = BattleMapInfo.GetMapFromFile(txt);
                splitContainer1.Panel1.Invalidate();
            }
            catch (Exception e)
            {
                MessageBox.Show("错误的文件格式" + e, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            splitContainer1.Panel1.Invalidate();
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {
            if (map.XCount <= 0)
                return;

            int cellWidth = splitContainer1.Panel1.Width / map.XCount;
            int cellHeight = splitContainer1.Panel1.Height / map.YCount;
            if (cellWidth < cellHeight)
                cellHeight = cellWidth;
            if (cellWidth > cellHeight)
                cellWidth = cellHeight;

            Font ft = new Font("宋体", 11, FontStyle.Bold);
            for (int i = 0; i < map.XCount; i++)
            {
                for (int j = 0; j < map.YCount; j++)
                {
                    var cell = map.Cells[i, j];
                    var brush = Brushes.Green;
                    if (Array.IndexOf(map.ColumnCompete, i) >= 0)
                        brush = Brushes.DarkOrange;
                    if (Array.IndexOf(map.ColumnMiddle, i) >= 0)
                        brush = Brushes.IndianRed;
                    if (cell != 9)
                        brush = Brushes.Black;
                    e.Graphics.FillRectangle(brush, i * cellWidth, j * cellHeight, cellWidth, cellHeight);
                    e.Graphics.DrawRectangle(Pens.DarkRed, i * cellWidth, j * cellHeight, cellWidth, cellHeight);
                    e.Graphics.DrawString(string.Format("{0}:{1}", i, j), ft, Brushes.Black, i * cellWidth+1, j * cellHeight+1);
                    e.Graphics.DrawString(string.Format("{0}:{1}", i, j), ft, Brushes.DarkGray, i * cellWidth, j * cellHeight);
                }
            }
            ft.Dispose();

            string pathParent = "../../PicResource/";
            for (int i = 0; i < map.LeftMon.Length; i += 3)
            {
                var unitId = map.LeftMon[i];
                var xPos = map.LeftMon[i + 1];
                var yPos = map.LeftMon[i + 2];

                var img = Image.FromFile(string.Format("{0}Monsters/{1}.JPG", pathParent, unitId % 1000000));
                e.Graphics.DrawImage(img, xPos * cellWidth, yPos * cellHeight, cellWidth, cellHeight);
                img.Dispose();

                var p = new Pen(Color.Red, 3);
                e.Graphics.DrawRectangle(p, xPos * cellWidth, yPos * cellHeight, cellWidth, cellHeight);
                p.Dispose();
            }
            for (int i = 0; i < map.RightMon.Length; i += 3)
            {
                var unitId = map.RightMon[i];
                var xPos = map.RightMon[i + 1];
                var yPos = map.RightMon[i + 2];

                var img = Image.FromFile(string.Format("{0}Monsters/{1}.JPG", pathParent, unitId % 1000000));
                e.Graphics.DrawImage(img, xPos * cellWidth, yPos * cellHeight, cellWidth, cellHeight);
                img.Dispose();
                var p = new Pen(Color.Blue, 3);
                e.Graphics.DrawRectangle(p, xPos * cellWidth, yPos * cellHeight, cellWidth, cellHeight);
                p.Dispose();
            }
            foreach (var unit in addonUnits)
            {
                var img = Image.FromFile(string.Format("{0}Monsters/{1}.JPG", pathParent, unit.UnitId % 1000000));
                e.Graphics.DrawImage(img, unit.X * cellWidth, unit.Y * cellHeight, cellWidth, cellHeight);
                img.Dispose();

                var p = new Pen(unit.Color, 3);
                e.Graphics.DrawRectangle(p, unit.X * cellWidth, unit.Y * cellHeight, cellWidth, cellHeight);
                p.Dispose();
            }
        }
    }
}
