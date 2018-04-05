using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Math;
using NarlonLib.Tools;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Datas.User.Db;
using TaleofMonsters.Forms.Items.Core;

namespace TaleofMonsters.Forms.VBuilds
{
    internal sealed partial class FarmForm : BasePanel
    {
        private bool showImage;
        private int select;

        public FarmForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            miniItemView1.Init();
            select = -1;

            showImage = true;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private int GetSelectedCell(int x, int y)
        {
            int newsel = -1;
            for (int i = 0; i < 9; i++)
            {
                int baseX = 15 + 218;
                int baseY = 40 + 56;
                if ((i / 3) != 0)
                {
                    baseX += 70 * (i / 3);
                    baseY += 40 * (i / 3);
                }
                if ((i % 3) != 0)
                {
                    baseX -= 80 * (i % 3);
                    baseY += 40 * (i % 3);
                }
                baseX = x - baseX;
                baseY = y - baseY;
                if (MathTool.ValueBetween(baseY, baseX / 2 - 41, baseX / 2 + 44) && MathTool.ValueBetween(baseY, -baseX / 2 + 44, -baseX / 2 + 127))
                {
                    newsel = i;
                }
            }
            return newsel;
        }

        private void FarmForm_MouseMove(object sender, MouseEventArgs e)
        {
            int newsel = GetSelectedCell(e.X, e.Y);
            if (newsel != select)
            {
                select = newsel;
                Invalidate();
            }
        }

        private void FarmForm_MouseClick(object sender, MouseEventArgs e)
        {
            int newsel = GetSelectedCell(e.X, e.Y);
            if (newsel != -1)
            {
                DbFarmState timeState = UserProfile.Profile.InfoCastle.GetFarmState(newsel);
                if (timeState.Type == -1)
                {
                    var pricecount = GameResourceBook.OutWoodBuildFarm((uint)UserProfile.Profile.InfoCastle.GetFarmAvailCount() * 20);
                    if (MessageBoxEx2.Show(string.Format("是否花{0}木材开启额外农田?", pricecount)) == DialogResult.OK)
                    {
                        if (UserProfile.InfoBag.HasResource(GameResourceType.Lumber, pricecount))
                        {
                            UserProfile.InfoBag.SubResource(GameResourceType.Lumber, pricecount);
                            UserProfile.Profile.InfoCastle.SetFarmState(newsel, new DbFarmState(0, 0));
                        }
                        else
                        {
                            AddFlowCenter(HSErrors.GetDescript(ErrorConfig.Indexer.BagNotEnoughResource), "Red");
                        }
                    }
                }
                else if (timeState.Type > 0)
                {
                    if (timeState.Time < TimeTool.DateTimeToUnixTime(DateTime.Now))
                    {
                        UserProfile.InfoBag.AddItem(timeState.Type, 1);
                        UserProfile.Profile.InfoCastle.SetFarmState(newsel, new DbFarmState(0, 0));
                    }
                    else
                    {
                        int pricecount = (timeState.Time - TimeTool.DateTimeToUnixTime(DateTime.Now)) / 600 + 1;
                        if (MessageBoxEx2.Show(string.Format("是否花{0}钻石催熟作物?", pricecount)) == DialogResult.OK)
                        {
                            if (UserProfile.InfoBag.PayDiamond(pricecount))
                            {
                                UserProfile.Profile.InfoCastle.SetFarmState(newsel, new DbFarmState(timeState.Type, 0));
                            }
                        }
                    }
                }
            }

        }

        public override void OnFrame(int tick, float timePass)
        {
            base.OnFrame(tick, timePass);
            if (tick % 6 == 0)
                Invalidate();
        }

        private void FarmForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("农场", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            if (!showImage)
                return;

            Image back = PicLoader.Read("Build", "farm.JPG");
            e.Graphics.DrawImage(back, 15, 40,572,352);
            back.Dispose();

            for (int i = 0; i < 9; i++)
            {
                int baseX = 15 + 218;
                int baseY = 40 + 56;
                if ((i/3)!=0)
                {
                    baseX += 70*(i/3);
                    baseY += 40 * (i / 3);
                }
                if ((i % 3) != 0)
                {
                    baseX -= 80 * (i % 3);
                    baseY += 40 * (i % 3);
                }
                DbFarmState timeState = UserProfile.Profile.InfoCastle.GetFarmState(i);
                string baseName = timeState.Type == -1 ? "Farm.tile2" : "Farm.tile1";
                Image tile = PicLoader.Read("Build", i == select ? baseName + "On.PNG" : baseName + ".PNG");
                e.Graphics.DrawImage(tile, baseX, baseY + 86 - tile.Height, tile.Width, tile.Height);
                tile.Dispose();

                if (timeState.Type <= 0)
                    continue;

                TimeSpan span = TimeTool.UnixTimeToDateTime(timeState.Time) - DateTime.Now;
                var itemConfig = ConfigData.GetHItemConfig(timeState.Type);
                Image veg = PicLoader.Read("Build", string.Format("Farm.{0}.PNG", itemConfig.Url));
                if (veg != null)
                {
                    if (span.TotalSeconds > 0)
                        e.Graphics.DrawImage(veg, new Rectangle(baseX + 59, baseY + 6, veg.Width, veg.Height), 0, 0, veg.Width, veg.Height, GraphicsUnit.Pixel, Core.HSImageAttributes.ToGray);
                    else
                        e.Graphics.DrawImage(veg, baseX + 59, baseY + 6, veg.Width, veg.Height);
                    veg.Dispose();
                }

                if (span.TotalSeconds > 0)
                {
                    string timeText = string.Format("{0}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds);
                    font = new Font("宋体", 9 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                    e.Graphics.DrawString(timeText, font, Brushes.White, baseX + 55, baseY + 30);
                    font.Dispose();
                }
            }
        }
    }
}