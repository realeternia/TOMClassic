using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Control;

namespace TaleofMonsters.MainItem
{
    internal partial class PopMenuBase : PopedCotainer
    {
        public PoperContainer PoperContainer;
        private const int yOff = 18;

        public PopMenuBase()
        {
            InitializeComponent();
            datas = new List<MenuItemData>();
        }

        private List<MenuItemData> datas;

        public void AddItem(string type, string text)
        {
            datas.Add(new MenuItemData(type, text, "White"));
        }

        public void AddItem(string type, string text, string color)
        {
            datas.Add(new MenuItemData(type, text, color));
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
            foreach (MenuItemData menuItemData in datas)
            {
                SizeF size= g.MeasureString(menuItemData.Text, fontsong);
                width = Math.Max(width, size.Width);
                height += yOff;
            }
            Width = (int)width+6;
            Height = (int)height;
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
            {
                return;
            }
            OnClick(datas[selectIndex]);
        }

        private void PopMenuDeck_Paint(object sender, PaintEventArgs e)
        {
            Font fontsong = new Font("ËÎÌו", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            int index = 0;
            if (selectIndex>-1)
            {
                e.Graphics.FillRectangle(Brushes.DarkBlue, 0, selectIndex * yOff, Width, yOff);
            }
            foreach (MenuItemData menuItemData in datas)
            {
                Brush b = new SolidBrush(Color.FromName(menuItemData.Color));
                e.Graphics.DrawString(menuItemData.Text, fontsong, b, 3, yOff*index+3);
                b.Dispose();
                index++;
            }
            fontsong.Dispose();
        }

        protected int selectIndex =-1;
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

    internal struct MenuItemData
    {
        public string Type;
        public string Text;
        public string Color;

        public MenuItemData(string t,string tx, string cr)
        {
            Type = t;
            Text = tx;
            Color = cr;
        }
    }
}

