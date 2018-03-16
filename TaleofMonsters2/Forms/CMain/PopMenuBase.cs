using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Control;
using TaleofMonsters.Core;

namespace TaleofMonsters.Forms.CMain
{
    internal partial class PopMenuBase : PopedCotainer
    {
        protected struct MenuItemData
        {
            public string Type;
            public string Text;
            public string Color;
            public string Icon;

            public MenuItemData(string t, string tx, string cr, string icon)
            {
                Type = t;
                Text = tx;
                Color = cr;
                Icon = icon;
            }
        }

        public PoperContainer PoperContainer;
        private const int yOff = 18;
        protected int selectIndex = -1;
        private List<MenuItemData> datas;

        public PopMenuBase()
        {
            InitializeComponent();
            datas = new List<MenuItemData>();
        }
        
        public void AddItem(string type, string text)
        {
            datas.Add(new MenuItemData(type, text, "white", ""));
        }

        public void AddItem(string type, string text, string color)
        {
            datas.Add(new MenuItemData(type, text, color, ""));
        }

        public void AddItem(string type, string text, string color, string icon)
        {
            datas.Add(new MenuItemData(type, text, color, icon));
        }

        public void Clear()
        {
            datas.Clear();
        }

        public void AutoResize()
        {
            float width = 0;
            float height = 0;
            Font fontsong = new Font("ËÎÌו", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            Bitmap tempImg = new Bitmap(3, 3);
            Graphics g = Graphics.FromImage(tempImg);
            foreach (var menuItemData in datas)
            {
                var imgOff = 0;
                if (menuItemData.Icon != "")
                    imgOff += 16;
                var size = TextRenderer.MeasureText(g, menuItemData.Text, fontsong, new Size(0, 0), TextFormatFlags.NoPadding);
                width = Math.Max(width, size.Width + imgOff);
                height += yOff;
            }
            Width = (int)width+6+5;
            Height = (int)height+3;
            fontsong.Dispose();
            g.Dispose();
            tempImg.Dispose();
        }

        protected virtual void OnClick(MenuItemData target)
        {
            
        }

        private void PopMenuDeck_Click(object sender, EventArgs e)
        {
            PoperContainer.Close();
            if (selectIndex == -1)
                return;
            OnClick(datas[selectIndex]);
        }

        private void PopMenuDeck_Paint(object sender, PaintEventArgs e)
        {
            Font fontsong = new Font("ËÎÌו", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            int index = 0;
            if (selectIndex > -1)
                e.Graphics.FillRectangle(Brushes.DeepSkyBlue, 0, selectIndex*yOff, Width, yOff);
            foreach (var menuItemData in datas)
            {
                var imgOff = 0;
                if (menuItemData.Icon != "")
                {
                    var iconImg = HSIcons.GetIconsByEName(menuItemData.Icon);
                    e.Graphics.DrawImage(iconImg, 4, yOff*index+2, 14, 14);
                    imgOff += 16;
                }
                Brush b = new SolidBrush(Color.FromName(menuItemData.Color));
                e.Graphics.DrawString(menuItemData.Text, fontsong, b, imgOff + 3, yOff*index + 3);
                b.Dispose();
                index++;
            }
            fontsong.Dispose();
        }

        private void PopMenuDeck_MouseMove(object sender, MouseEventArgs e)
        {
            int tepIndex = e.Y/yOff;
            if (selectIndex != tepIndex)
            {
                selectIndex = tepIndex;
                Invalidate();
            }
        }

        private void PopMenuDeck_MouseLeave(object sender, EventArgs e)
        {
            if (selectIndex != -1)
            {
                selectIndex = -1;
                Invalidate();
            }
        }
    }

}

