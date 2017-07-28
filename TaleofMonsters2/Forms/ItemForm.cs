using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Control;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.MainItem;

namespace TaleofMonsters.Forms
{
    internal sealed partial class ItemForm : BasePanel
    {
        private const int CellCountPerPage = 100;

        private int pageid;
        private bool isDirty;
        private bool show;
        private int tar;
        private int rightSelectTar; //右键目标
        private int leftSelectTar;//左键目标
        private int baseid;
        private Bitmap tempImage;
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private HSCursor myCursor;
        private NLPageSelector nlPageSelector1;

        private PopMenuItem popMenuItem;
        private PoperContainer popContainer;

        private int[] itemCdRate = new int[CellCountPerPage]; //cd显示

        public ItemForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            this.bitmapButtonSort.ImageNormal = PicLoader.Read("Button.Panel", "SortButton.JPG");
            bitmapButtonSort.NoUseDrawNine = true;
            this.nlPageSelector1 = new ControlPlus.NLPageSelector(this, 123, 362, 204);
            nlPageSelector1.PageChange += nlPageSelector1_PageChange;

            tempImage = new Bitmap(324, 324);
            baseid = 0;
            tar = leftSelectTar = rightSelectTar = -1;
            myCursor = new HSCursor(this);

            popMenuItem = new PopMenuItem();
            popContainer = new PoperContainer(popMenuItem);
            popMenuItem.PoperContainer = popContainer;
            popMenuItem.Form = this;
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            show = true;
            nlPageSelector1.TotalPage = Math.Min((UserProfile.InfoBag.BagCount - 1) / CellCountPerPage + 2, 9);
            nlPageSelector1.NoFresh = true;
            RefreshFrame();
        }

        public override void OnFrame(int tick, float timePass)
        {
            base.OnFrame(tick, timePass);

            for(int i=0;i< CellCountPerPage; i++)
            {
                if (baseid + i >= UserProfile.InfoBag.BagCount)
                    continue;

                IntPair thing = UserProfile.InfoBag.Items[baseid + i];
                var rate = (int)(UserProfile.InfoBag.GetCdTimeRate(thing.Type) * 100); //type允许为0
                if (rate != itemCdRate[i])
                {
                    itemCdRate[i] = rate;
                    Invalidate(new Rectangle((int)((i % 10) * 31.2f + 5 + 6), (int)((i / 10) * 31.8f + 3 + 36), 30, 30));
                }
            }
        }

        private void RefreshFrame()
        {
            tar = -1;
            rightSelectTar = -1;
            leftSelectTar = -1;
            tooltip.Hide(this);
            myCursor.ChangeCursor("default");
            isDirty = true;
            Invalidate(new Rectangle(6, 36, 324, 324));
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ItemView_MouseMove(object sender, MouseEventArgs e)
        {
            int truex = e.X - 6;
            int truey = e.Y - 36;
            if (truex > 5 && truey > 3 && truex < 315 + 5 && truey < 318 + 3)
            {
                int temp = (truex - 8)*10/315 + (truey - 5)*10/318*10;
                if (temp != tar)
                {
                    tar = temp;
                    if (baseid + tar < UserProfile.InfoBag.BagCount && UserProfile.InfoBag.Items[baseid + tar].Type != 0)
                    {
                        Image image = HItemBook.GetPreview(UserProfile.InfoBag.Items[baseid + tar].Type);
                        tooltip.Show(image, this, (tar%10)*315/10 + 42, (tar/10)*318/10 + 42);
                    }
                    else
                    {
                        tooltip.Hide(this);
                    }
                    Invalidate(new Rectangle(6, 36, 324, 324));
                }
            }
            else
            {
                tar = -1;
                tooltip.Hide(this);
                Invalidate(new Rectangle(6, 36, 324, 324));
            }
        }

        private void ItemForm_MouseLeave(object sender, EventArgs e)
        {
            tar = -1;
            tooltip.Hide(this);
            Invalidate(new Rectangle(6, 36, 324, 324));
        }

        public void MenuRefresh()
        {
            isDirty = true;
            Invalidate(new Rectangle(6, 36, 324, 324));
        }

        private void ItemView_MouseClick(object sender, MouseEventArgs e)
        {
            if (tar == -1)
                return;

            if (e.Button == MouseButtons.Left)
            {
                if (leftSelectTar == -1)
                {
                    if (baseid + tar < UserProfile.InfoBag.BagCount)
                    {
                        if (UserProfile.InfoBag.Items[baseid + tar].Type != 0)
                        {
                            HItemConfig itemConfig = ConfigData.GetHItemConfig(UserProfile.InfoBag.Items[baseid + tar].Type);
                            myCursor.ChangeCursor("Item", string.Format("{0}.JPG", itemConfig.Url), 40, 40);
                            leftSelectTar = tar;
                            tooltip.Hide(this);
                        }
                    }
                    else
                    {
                        int diff = baseid + tar - UserProfile.InfoBag.BagCount + 1;
                        uint pricecount = 0;
                        for (int i = 0; i < diff; i++)
                        {
                            pricecount += (uint)((UserProfile.InfoBag.BagCount + i) / 5 + 10);
                        }
                        if (MessageBoxEx2.Show(string.Format("是否花{0}金币开启额外的{1}格物品格?", pricecount, diff)) == DialogResult.OK)
                        {
                            if (UserProfile.InfoBag.HasResource(GameResourceType.Gold, pricecount))
                            {
                                UserProfile.InfoBag.SubResource(GameResourceType.Gold, pricecount);
                                UserProfile.InfoBag.ResizeBag(UserProfile.InfoBag.BagCount+diff);

                                nlPageSelector1.TotalPage = Math.Min((UserProfile.InfoBag.BagCount - 1) / CellCountPerPage + 2, 9);
                                AddFlowCenter("背包扩容完成", "LimeGreen");
                            }
                        }
                    }
                }
                else
                {
                    myCursor.ChangeCursor("default");
                    if (baseid + tar < UserProfile.InfoBag.BagCount)
                    {
                        if (UserProfile.InfoBag.Items[baseid + tar].Type == 0)
                        {
                            UserProfile.InfoBag.Items[baseid + tar].Type = UserProfile.InfoBag.Items[baseid + leftSelectTar].Type;
                            UserProfile.InfoBag.Items[baseid + leftSelectTar].Type = 0;
                            UserProfile.InfoBag.Items[baseid + tar].Value = UserProfile.InfoBag.Items[baseid + leftSelectTar].Value;
                            UserProfile.InfoBag.Items[baseid + leftSelectTar].Value = 0;
                        }
                        else
                        {
                            int oldid = UserProfile.InfoBag.Items[baseid + tar].Type;
                            UserProfile.InfoBag.Items[baseid + tar].Type = UserProfile.InfoBag.Items[baseid + leftSelectTar].Type;
                            UserProfile.InfoBag.Items[baseid + leftSelectTar].Type = oldid;
                            int oldcount = UserProfile.InfoBag.Items[baseid + tar].Value;
                            UserProfile.InfoBag.Items[baseid + tar].Value = UserProfile.InfoBag.Items[baseid + leftSelectTar].Value;
                            UserProfile.InfoBag.Items[baseid + leftSelectTar].Value = oldcount;
                        }
                    }
                    leftSelectTar = -1;
                }
                isDirty = true;
                Invalidate(new Rectangle(6, 36, 324, 324));
            }
            else if (e.Button == MouseButtons.Right)
            {
                rightSelectTar = tar;
                tooltip.Hide(this);

                if (rightSelectTar == -1)
                    return;

                popMenuItem.Clear();
                HItemConfig itemConfig = ConfigData.GetHItemConfig(UserProfile.InfoBag.Items[baseid + rightSelectTar].Type);

                #region 构建菜单
                if (itemConfig.IsUsable)
                {
                    popMenuItem.AddItem("use", "使用");
                }
                if (itemConfig.Id == 0 || (itemConfig.Id != 0 && itemConfig.IsThrowable))
                {
                    popMenuItem.AddItem("throw", "丢弃", "Red");
                    popMenuItem.AddItem("sold", "卖出", "Red");
                }
                popMenuItem.AddItem("exit", "退出");
                #endregion

                popMenuItem.AutoResize();
                popMenuItem.ItemIndex = baseid + rightSelectTar;
                popContainer.Show(this, e.Location.X, e.Location.Y);  
            }
        }

        private void ItemView_DoubleClick(object sender, EventArgs e)
        {
            if (tar == -1)
                return;

            HItemConfig itemConfig = ConfigData.GetHItemConfig(UserProfile.InfoBag.Items[baseid + tar].Type);
            if (itemConfig.IsUsable && itemConfig.SubType != HItemTypes.Fight)
            {
                UserProfile.InfoBag.UseItemByPos(baseid + tar, HItemTypes.Common);
                leftSelectTar = -1;
                myCursor.ChangeCursor("default");
                isDirty = true;
                Invalidate(new Rectangle(6, 36, 324, 324));
            }
        }

        private void ItemForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("我的物品", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            if (!show)
                return;

            if (isDirty)
            {
                tempImage.Dispose();
                tempImage = new Bitmap(324, 324);
                Graphics g = Graphics.FromImage(tempImage);
                Image img = PicLoader.Read("System", "ItemBackDown.JPG");
                g.DrawImage(img, 0, 0, 324, 324);
                img.Dispose();
                font = new Font("Aril", 11*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                for (int i = 0; i < CellCountPerPage; i++)
                {
                    if (baseid + i < UserProfile.InfoBag.BagCount)
                    {
                        IntPair thing = UserProfile.InfoBag.Items[baseid + i];
                        if (thing.Type != 0)
                        {
                            var itemConfig = ConfigData.GetHItemConfig(thing.Type);
                            g.DrawImage(HItemBook.GetHItemImage(thing.Type), (i % 10) * 31.2f + 5, (i / 10) * 31.8f + 3, 30.0f, 30.0f);
                            g.DrawString(thing.Value.ToString(), font, Brushes.Black, (i % 10) * 31.2f + 8, (i / 10) * 31.8f + 14);
                            g.DrawString(thing.Value.ToString(), font, thing.Value == itemConfig.MaxPile ? Brushes.Tomato : Brushes.White, (i % 10) * 31.2f + 7, (i / 10) * 31.8f + 13);

                            var pen = new Pen(Color.FromName(HSTypes.I2RareColor(itemConfig.Rare)), 2);
                            g.DrawRectangle(pen, (i % 10) * 31.2f + 5, (i / 10) * 31.8f + 3, 30.0f, 30.0f);
                            pen.Dispose();
                        }
                    }
                    else
                    {
                        g.DrawImage(HSIcons.GetIconsByEName("oth3"), (i % 10) * 31.2f + 8, (i / 10) * 31.8f + 4, 24.0f, 24.0f);
                    }
                }
                font.Dispose();
                g.Dispose();
                isDirty = false;
            }
            e.Graphics.DrawImage(tempImage, 6, 36);

            Brush brush = new SolidBrush(Color.FromArgb(150, Color.Yellow));
            for (int i = 0; i < itemCdRate.Length; i++)
            {
                if (itemCdRate[i] > 0)
                {
                    e.Graphics.FillRectangle(brush, (i % 10) * 31.2f + 5 + 6, (i/10) * 31.8f + 3 + 36, 30, 30 *(100- itemCdRate[i]) / 100);
                }
            }
            brush.Dispose();

            int rect = tar;
            if (rect >= 0)
            {
                if (baseid + rect < UserProfile.InfoBag.BagCount)
                {
                    SolidBrush yellowbrush = new SolidBrush(Color.FromArgb(80, Color.Yellow));
                    e.Graphics.FillRectangle(yellowbrush, (rect%10)*31.2f + 11, (rect/10)*31.8f + 39, 30.0f, 30.0f);
                    yellowbrush.Dispose();

                    Pen yellowpen = new Pen(Brushes.Yellow, 2);
                    e.Graphics.DrawRectangle(yellowpen, (rect%10)*31.2f + 11, (rect/10)*31.8f + 39, 30.0f, 30.0f);
                    yellowpen.Dispose();
                }
                else
                {
                    int tpoff = rect;
                    while (tpoff >= 0 && baseid + tpoff >= UserProfile.InfoBag.BagCount)
                    {
                        e.Graphics.DrawImage(HSIcons.GetIconsByEName("oth4"), (tpoff % 10) * 31.2f + 14, (tpoff / 10) * 31.8f + 40, 24f, 24f);
                        tpoff--;
                    }
                }
            }
        }

        private void nlPageSelector1_PageChange(int pg)
        {
            pageid = pg;
            baseid = pageid * CellCountPerPage;
            RefreshFrame();
        }

        private void bitmapButtonSort_Click(object sender, EventArgs e)
        {
            UserProfile.InfoBag.SortItem();
            RefreshFrame();
            AddFlowCenter("背包整理完成", "LimeGreen");
        }
    }
}