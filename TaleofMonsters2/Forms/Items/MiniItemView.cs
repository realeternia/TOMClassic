using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using NarlonLib.Control;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.Forms.Items
{
    internal partial class MiniItemView : UserControl
    {
        private bool show;
        private int tar; //背包里的格子偏移
        private int page;
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private HSCursor myCursor;
        private List<int> ids;
        private MiniItemViewItem[] items;
        private const int cellCount = 6;
        private const int cdCount = 11;
        private int[] cds;

        public MiniItemView()
        {
            InitializeComponent();
            tar = -1;
            myCursor = new HSCursor(this);
        }

        public int ItemSubType { get; set; } //只有某一子类别的道具可以进入这个背包

        public void Init()
        {
            show = true;
            myCursor.ChangeCursor("default");
            this.bitmapButtonLeft.ImageNormal = PicLoader.Read("ButtonBitmap", "PreButton.JPG");
            bitmapButtonLeft.NoUseDrawNine = true;
            this.bitmapButtonRight.ImageNormal = PicLoader.Read("ButtonBitmap", "NextButton.JPG");
            bitmapButtonRight.NoUseDrawNine = true;
            items = new MiniItemViewItem[cellCount];
            for (int i = 0; i < cellCount; i++)
            {
                items[i] = new MiniItemViewItem(i+1, 30 * (i % 2), i / 2 * 35);
            }
            cds = new int[cdCount];
            for (int i = 0; i < cellCount; i++)
            {
                cds[i] = 0;
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
            for (int i = page * cellCount; i < page * cellCount + cellCount; i++)
            {
                if (i < ids.Count)
                {
                    items[i % cellCount].itemPos = ids[i];
                }
                else
                {
                    items[i % cellCount].itemPos = -1;
                }
            }
        }

        public void NewTick()
        {
            foreach (var item in items)
            {
                if (item.itemPos < 0)
                    continue;

                int itid = UserProfile.InfoBag.Items[item.itemPos].Type;
                HItemConfig itemConfig = ConfigData.GetHItemConfig(itid);
                if (itemConfig.CdGroup > 0 && cds[itemConfig.CdGroup] > 0)
                {
                    item.Percent = cds[itemConfig.CdGroup] * 100 / CdGroup.GetCDTime(itemConfig.CdGroup);
                    Invalidate(item.Rectangle);
                }
            }

            for (int i = 0; i < cdCount; i++)
            {
                if (cds[i] > 0)
                {
                    cds[i]--;
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
                    temp = item.itemPos;
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

            HItemConfig itemConfig = ConfigData.GetHItemConfig(UserProfile.InfoBag.Items[tar].Type);
            if (itemConfig.IsUsable)
            {
                if (itemConfig.CdGroup > 0)
                {
                    if (cds[itemConfig.CdGroup] > 0)
                    {
                        return;
                    }
                    cds[itemConfig.CdGroup] = CdGroup.GetCDTime(itemConfig.CdGroup);
                }
                int count = UserProfile.InfoBag.Items[tar].Value;
                UserProfile.InfoBag.UseItemByPos(tar, ItemSubType);
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
