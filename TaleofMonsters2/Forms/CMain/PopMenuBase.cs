using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using NarlonLib.Math;
using TaleofMonsters.Core;

namespace TaleofMonsters.Forms.CMain
{
    internal partial class PopMenuBase : PopedCotainer
    {
        protected class MenuItemData
        {
            public int Id;

            public string Type;
            public string Text;
            public string Color;
            public string Icon;

            public Point Position;
            public Size Size;

            public MenuItemData(int id, string t, string tx, string cr, string icon)
            {
                Id = id;
                Type = t;
                Text = tx;
                Color = cr;
                Icon = icon;
            }
        }

        private const int CellHeight = 18;
        private int CellWidth = 18;
        private int selectIndex = -1;
        private List<MenuItemData> datas;
        protected int columnCount = 1;

        public PoperContainer PoperContainer;

        public PopMenuBase()
        {
            InitializeComponent();
            datas = new List<MenuItemData>();
        }
        
        public void AddItem(string type, string text)
        {
            datas.Add(new MenuItemData(datas.Count+1, type, text, "white", ""));
        }

        public void AddItem(string type, string text, string color)
        {
            datas.Add(new MenuItemData(datas.Count + 1, type, text, color, ""));
        }

        public void AddItem(string type, string text, string color, string icon)
        {
            datas.Add(new MenuItemData(datas.Count + 1, type, text, color, icon));
        }

        public void Clear()
        {
            datas.Clear();
        }

        public void AutoResize()
        {
            float maxCellWidth = 0;
            Font fontsong = new Font("宋体", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            Bitmap tempImg = new Bitmap(3, 3);
            Graphics g = Graphics.FromImage(tempImg);
            foreach (var menuItemData in datas)
            {//计算最大的宽度
                var imgOff = 0;
                if (menuItemData.Icon != "")
                    imgOff += 16;
                var size = TextRenderer.MeasureText(g, menuItemData.Text, fontsong, new Size(0, 0), TextFormatFlags.NoPadding);
                maxCellWidth = Math.Max(maxCellWidth, size.Width + imgOff + 11);
            }
            fontsong.Dispose();
            g.Dispose();
            tempImg.Dispose();

            CellWidth = (int) maxCellWidth;
            int index = 0;
            foreach (var menuItemData in datas)
            {
                menuItemData.Position = new Point((index%columnCount)*CellWidth, (index / columnCount) * CellHeight);
                menuItemData.Size = new Size(CellWidth, CellHeight);
                index++;
            }
            
            Width = columnCount * CellWidth;
            Height = ((index + columnCount - 1)/columnCount)*CellHeight;
        }


        protected virtual void OnClick(MenuItemData target)
        {
            
        }

        private void PopMenuBase_Click(object sender, EventArgs e)
        {
            PoperContainer.Close();
            if (selectIndex == -1)
                return;
            OnClick(datas[selectIndex-1]);
        }

        private void PopMenuBase_Paint(object sender, PaintEventArgs e)
        {
            Font fontsong = new Font("宋体", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            int index = 0;
            foreach (var menuItemData in datas)
            {
                if (selectIndex == menuItemData.Id)
                    e.Graphics.FillRectangle(Brushes.DeepSkyBlue, menuItemData.Position.X, menuItemData.Position.Y, menuItemData.Size.Width, menuItemData.Size.Height);

                var imgOff = 0;
                if (menuItemData.Icon != "")
                {
                    var iconImg = HSIcons.GetIconsByEName(menuItemData.Icon);
                    e.Graphics.DrawImage(iconImg, menuItemData.Position.X + 3, menuItemData.Position.Y+2, 14, 14);
                    imgOff += 16;
                }
                Brush b = new SolidBrush(Color.FromName(menuItemData.Color));
                e.Graphics.DrawString(menuItemData.Text, fontsong, b, imgOff + 3 + menuItemData.Position.X, 2 + menuItemData.Position.Y);
                b.Dispose();
                index++;
            }
            fontsong.Dispose();
        }

        private void PopMenuBase_MouseMove(object sender, MouseEventArgs e)
        {
            int tepIndex = -1;
            foreach (var menuItemData in datas)
            {
                if (MathTool.IsPointInRegion(e.X, e.Y, menuItemData.Position.X, menuItemData.Position.Y,
                    menuItemData.Position.X+menuItemData.Size.Width, menuItemData.Position.Y+menuItemData.Size.Height, true))
                {
                    tepIndex = menuItemData.Id;
                    break;
                }
            }
            if (selectIndex != tepIndex)
            {
                selectIndex = tepIndex;
                Invalidate();
            }
        }

        private void PopMenuBase_MouseLeave(object sender, EventArgs e)
        {
            if (selectIndex != -1)
            {
                selectIndex = -1;
                Invalidate();
            }
        }
    }

}

