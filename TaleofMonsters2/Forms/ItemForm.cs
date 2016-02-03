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
        private int pageid;
        private bool isDirty;
        private bool show;
        private int tar;
        private int selectTar;
        private int baseid;
        private Bitmap tempImage;
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private HSCursor myCursor;
        private NLPageSelector nlPageSelector1;

        private PopMenuItem popMenuItem;
        private PoperContainer popContainer;

        public ItemForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");
            this.bitmapButtonSort.ImageNormal = PicLoader.Read("ButtonBitmap", "SortButton.JPG");
            bitmapButtonSort.NoUseDrawNine = true;
            this.nlPageSelector1 = new ControlPlus.NLPageSelector(this, 123, 362, 204);
            nlPageSelector1.PageChange += nlPageSelector1_PageChange;

            tempImage = new Bitmap(324, 324);
            baseid = 0;
            tar = selectTar = -1;
            myCursor = new HSCursor(this);

            popMenuItem = new PopMenuItem();
            popContainer = new PoperContainer(popMenuItem);
            popMenuItem.PoperContainer = popContainer;
            popMenuItem.Form = this;
        }

        internal override void Init(int width, int height)
        {
            base.Init(width, height);
            show = true;
            nlPageSelector1.TotalPage = Math.Min((UserProfile.InfoBag.BagCount - 1) / 100 + 2, 9);
            RefreshFrame();
        }

        private void RefreshFrame()
        {
            tar = -1;
            selectTar = -1;
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
                if (selectTar == -1)
                {
                    if (baseid + tar < UserProfile.InfoBag.BagCount)
                    {
                        if (UserProfile.InfoBag.Items[baseid + tar].Type != 0)
                        {
                            HItemConfig itemConfig = ConfigData.GetHItemConfig(UserProfile.InfoBag.Items[baseid + tar].Type);
                            myCursor.ChangeCursor("Item", String.Format("{0}.JPG", itemConfig.Url), 40, 40);
                            selectTar = tar;
                            tooltip.Hide(this);
                        }
                    }
                    else
                    {
                        int diff = baseid + tar - UserProfile.InfoBag.BagCount + 1;
                        int pricecount = 0;
                        for (int i = 0; i < diff; i++)
                        {
                            pricecount += (UserProfile.InfoBag.BagCount + i) / 50 + 1;
                        }
                        if (MessageBoxEx2.Show(string.Format("是否花{0}钻石开启额外的{1}格物品格?", pricecount, diff)) == DialogResult.OK)
                        {
                            if (UserProfile.InfoBag.PayDiamond(pricecount))
                            {
                                IntPair[] item = new IntPair[UserProfile.InfoBag.BagCount];
                                Array.Copy(UserProfile.InfoBag.Items, item, UserProfile.InfoBag.BagCount);
                                UserProfile.InfoBag.BagCount += diff;
                                UserProfile.InfoBag.Items = new IntPair[UserProfile.InfoBag.BagCount];
                                Array.Copy(item, UserProfile.InfoBag.Items, UserProfile.InfoBag.BagCount - diff);

                                nlPageSelector1.TotalPage = Math.Min((UserProfile.InfoBag.BagCount - 1) / 100 + 2, 9);

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
                            UserProfile.InfoBag.Items[baseid + tar].Type = UserProfile.InfoBag.Items[baseid + selectTar].Type;
                            UserProfile.InfoBag.Items[baseid + selectTar].Type = 0;
                            UserProfile.InfoBag.Items[baseid + tar].Value = UserProfile.InfoBag.Items[baseid + selectTar].Value;
                            UserProfile.InfoBag.Items[baseid + selectTar].Value = 0;
                        }
                        else
                        {
                            int oldid = UserProfile.InfoBag.Items[baseid + tar].Type;
                            UserProfile.InfoBag.Items[baseid + tar].Type = UserProfile.InfoBag.Items[baseid + selectTar].Type;
                            UserProfile.InfoBag.Items[baseid + selectTar].Type = oldid;
                            int oldcount = UserProfile.InfoBag.Items[baseid + tar].Value;
                            UserProfile.InfoBag.Items[baseid + tar].Value = UserProfile.InfoBag.Items[baseid + selectTar].Value;
                            UserProfile.InfoBag.Items[baseid + selectTar].Value = oldcount;
                        }
                    }
                    selectTar = -1;
                }
                isDirty = true;
                Invalidate(new Rectangle(6, 36, 324, 324));
            }
            else if (e.Button == MouseButtons.Right)
            {
                selectTar = tar;
                tooltip.Hide(this);

                if (selectTar == -1)
                    return;

                popMenuItem.Clear();
                HItemConfig itemConfig = ConfigData.GetHItemConfig(UserProfile.InfoBag.Items[baseid + selectTar].Type);

                #region 构建菜单
                if (itemConfig.IsUsable)
                {
                    popMenuItem.AddItem("use", "使用");
                }
                if (itemConfig.Id != 0 && itemConfig.IsThrowable)
                {
                    popMenuItem.AddItem("throw", "丢弃", "Red");
                    popMenuItem.AddItem("sold", "卖出", "Red");
                }
                popMenuItem.AddItem("exit", "退出");
                #endregion

                popMenuItem.AutoResize();
                popMenuItem.ItemIndex = baseid + selectTar;
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
                selectTar = -1;
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
                for (int i = 0; i < 100; i++)
                {
                    int imul = i/10;
                    if (baseid + i < UserProfile.InfoBag.BagCount)
                    {
                        IntPair thing = UserProfile.InfoBag.Items[baseid + i];
                        if (thing.Type != 0)
                        {
                            int max = ConfigData.GetHItemConfig(thing.Type).MaxPile;
                            g.DrawImage(HItemBook.GetHItemImage(thing.Type), (i % 10) * 31.2F + 5, imul * 31.8F + 3, 30.0F, 30.0F);
                            g.DrawString(thing.Value.ToString(), font, Brushes.Black, (i % 10) * 31.2F + 8, imul * 31.8F + 14);
                            g.DrawString(thing.Value.ToString(), font, thing.Value == max ? Brushes.Tomato : Brushes.White, (i % 10) * 31.2F + 7, imul * 31.8F + 13);
                        }
                    }
                    else
                    {
                        g.DrawImage(HSIcons.GetIconsByEName("oth3"), (i % 10) * 31.2F + 8, imul * 31.8F + 4, 24.0F, 24.0F);
                    }
                }
                font.Dispose();
                g.Dispose();
                isDirty = false;
            }
            e.Graphics.DrawImage(tempImage, 6, 36);
            int rect = tar;
            if (tar == -1 && selectTar >= 0)
                rect = selectTar;
            if (rect >= 0)
            {
                if (baseid + rect < UserProfile.InfoBag.BagCount)
                {
                    SolidBrush yellowbrush = new SolidBrush(Color.FromArgb(80, Color.Yellow));
                    e.Graphics.FillRectangle(yellowbrush, (rect%10)*31.2F + 11, (rect/10)*31.8F + 39, 30.0F, 30.0F);
                    yellowbrush.Dispose();

                    Pen yellowpen = new Pen(Brushes.Yellow, 2);
                    e.Graphics.DrawRectangle(yellowpen, (rect%10)*31.2F + 11, (rect/10)*31.8F + 39, 30.0F, 30.0F);
                    yellowpen.Dispose();
                }
                else
                {
                    int tpoff = rect;
                    while (tpoff >= 0 && baseid + tpoff >= UserProfile.InfoBag.BagCount)
                    {
                        e.Graphics.DrawImage(HSIcons.GetIconsByEName("oth4"), (tpoff % 10) * 31.2F + 14, (tpoff / 10) * 31.8F + 40, 24F, 24F);
                        tpoff--;
                    }
                }
            }
        }

        private void nlPageSelector1_PageChange(int pg)
        {
            pageid = pg;
            baseid = pageid * 100;
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