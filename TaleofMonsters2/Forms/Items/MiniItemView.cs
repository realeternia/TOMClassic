using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Items;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain;

namespace TaleofMonsters.Forms.Items
{
    internal partial class MiniItemView : UserControl
    {
        private const int CellCount = 6;

        private bool show;
        private int tar; //背包里的格子偏移
        private int page;
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private HSCursor myCursor;
        private List<int> ids;
        private MiniItemViewItem[] items;

        public int ItemSubType { get; set; } //只有某一子类别的道具可以进入这个背包
        public HItemUseTypes UseType { get; set; }

        public MiniItemView()
        {
            InitializeComponent();
            tar = -1;
            myCursor = new HSCursor(this);
        }
        
        public void Init()
        {
            show = true;
            myCursor.ChangeCursor("default");
            this.bitmapButtonLeft.ImageNormal = PicLoader.Read("Button.Panel", "PreButton.JPG");
            bitmapButtonLeft.NoUseDrawNine = true;
            this.bitmapButtonRight.ImageNormal = PicLoader.Read("Button.Panel", "NextButton.JPG");
            bitmapButtonRight.NoUseDrawNine = true;
            items = new MiniItemViewItem[CellCount];
            for (int i = 0; i < CellCount; i++)
            {
                items[i] = new MiniItemViewItem(i+1, 30 * (i % 2), i / 2 * 35);
            }

            RefreshList();
        }

        private void RefreshList()
        {
            ids = new List<int>();
            for (int i = 0; i < UserProfile.InfoBag.BagCount; i++)
            {
                if (UserProfile.InfoBag.Items[i].Type != 0)
                {
                    HItemConfig itemConfig = ConfigData.GetHItemConfig(UserProfile.InfoBag.Items[i].Type);
                    if (itemConfig.SubType == ItemSubType && itemConfig.IsUsable)
                    {
                        ids.Add(i);
                    }
                }
            }
            page = 0;
            RefreshItems();
            CheckButton();
        }

        private void RefreshItems()
        {
            for (int i = page * CellCount; i < page * CellCount + CellCount; i++)
            {
                if (i < ids.Count)
                {
                    items[i % CellCount].ItemPos = ids[i];
                }
                else
                {
                    items[i % CellCount].ItemPos = -1;
                }
            }
        }

        public void OnFrame()
        {
            foreach (var item in items)
            {
                if (item.ItemPos < 0)
                    continue;

                int itemId = UserProfile.InfoBag.Items[item.ItemPos].Type;
                var rate = (int)(UserProfile.InfoBag.GetCdTimeRate(itemId)*100);
                if (rate != item.Percent)
                {
                    item.Percent = rate;
                    Invalidate(item.Rectangle);
                }
            }
        }

        private void CheckButton()
        {
            bitmapButtonLeft.Enabled = page > 0;
            bitmapButtonRight.Enabled = (page + 1) * 6 < ids.Count;
            Invalidate();
        }

        private void ItemView_Paint(object sender, PaintEventArgs e)
        {
            if (!show)
                return;

            foreach (MiniItemViewItem item in items)
            {
                item.Draw(e.Graphics, Enabled);
            }
        }

        private void ItemView_MouseMove(object sender, MouseEventArgs e)
        {
            int temp = -1;
            int index = -1;
            foreach (MiniItemViewItem item in items)
            {
                if (item.IsInArea(e.X, e.Y))
                {
                    temp = item.ItemPos;
                    index = item.Id-1;
                    break;
                }
            }

            if (temp != tar)
            {
                tar = temp;
                if (tar != -1)
                {
                    Image image = HItemBook.GetPreview(UserProfile.InfoBag.Items[tar].Type);
                    tooltip.Show(image, this, (index % 2) * 30 - image.Width + 2, (index / 2) * 35 + 3);

                }
                else
                {
                    tooltip.Hide(this);
                }
            }
        }

        private void ItemView_DoubleClick(object sender, EventArgs e)
        {
            if (!Enabled || tar == -1)
                return;

            var itemId = UserProfile.InfoBag.Items[tar].Type;
            HItemConfig itemConfig = ConfigData.GetHItemConfig(itemId);
            if (itemConfig.IsUsable)
            {
                int count = UserProfile.InfoBag.Items[tar].Value;
                UserProfile.InfoBag.UseItemByPos(tar, UseType);
                if (count == 1)
                {
                    RefreshList();
                }
                Invalidate();
            }
        }

        private void ItemView_MouseLeave(object sender, EventArgs e)
        {
            tar = -1;
            tooltip.Hide(this);
            Invalidate();
        }

        private void bitmapButtonRight_Click(object sender, EventArgs e)
        {
            if ((page+1) * 6 < ids.Count)
            {
                tar = -1;
                page++;
                RefreshItems();
                CheckButton();
            }
        }

        private void bitmapButtonLeft_Click(object sender, EventArgs e)
        {
            if (page > 0)
            {
                tar = -1;
                page--;
                RefreshItems();
                CheckButton();
            }
        }
    }
}
