using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Math;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Items;
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
            this.bitmapButtonHelp.ImageNormal = PicLoader.Read("Button.Panel", "LearnButton.JPG");
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
                DbFarmState farmState = UserProfile.Profile.InfoCastle.GetFarmState(newsel);
                if (farmState.Type == -1)//空地
                {
                    var pricecount = GameResourceBook.OutWoodBuildFarm((uint)UserProfile.Profile.InfoCastle.GetFarmAvailCount() * 20);
                    if (MessageBoxEx2.Show(string.Format("是否花{0}木材开启额外农田?", pricecount)) == DialogResult.OK)
                    {
                        if (UserProfile.InfoBag.HasResource(GameResourceType.Lumber, pricecount))
                        {
                            UserProfile.InfoBag.SubResource(GameResourceType.Lumber, pricecount);
                            UserProfile.Profile.InfoCastle.SetFarmState(newsel, new DbFarmState(0));
                        }
                        else
                        {
                            AddFlowCenter(HSErrors.GetDescript(ErrorConfig.Indexer.BagNotEnoughResource), "Red");
                        }
                    }
                }
                else if (farmState.Type > 0) //有种子
                {
                    if (farmState.Ep >= farmState.EpNeed)
                    {
                        UserProfile.InfoBag.AddItem(farmState.Type, 1);
                        UserProfile.Profile.InfoCastle.SetFarmState(newsel, new DbFarmState(0));

                        AddFlowCenter("+1", "Lime", HItemBook.GetHItemImage(farmState.Type));
                    }
                    else
                    {
                        AddFlowCenter("作物还未成熟", "Red");
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
                DbFarmState farmState = UserProfile.Profile.InfoCastle.GetFarmState(i);
                string baseName = farmState.Type == -1 ? "Farm.tile2" : "Farm.tile1";
                Image tile = PicLoader.Read("Build", i == select ? baseName + "On.PNG" : baseName + ".PNG");
                e.Graphics.DrawImage(tile, baseX, baseY + 86 - tile.Height, tile.Width, tile.Height);
                tile.Dispose();

                if (farmState.Type <= 0)
                    continue;

                var itemConfig = ConfigData.GetHItemConfig(farmState.Type);
                Image veg = PicLoader.Read("Build", string.Format("Farm.{0}.PNG", itemConfig.Url));
                if (veg != null)
                {
                    if (farmState.Ep < farmState.EpNeed)
                        e.Graphics.DrawImage(veg, new Rectangle(baseX + 59, baseY + 6, veg.Width, veg.Height), 0, 0, veg.Width, veg.Height, GraphicsUnit.Pixel, Core.HSImageAttributes.ToGray);
                    else
                        e.Graphics.DrawImage(veg, baseX + 59, baseY + 6, veg.Width, veg.Height);
                    veg.Dispose();
                }

                if (farmState.Ep < farmState.EpNeed)
                {
                    string timeText = string.Format("{0}/{1}", farmState.Ep, farmState.EpNeed);
                    font = new Font("宋体", 9 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                    e.Graphics.DrawString(timeText, font, Brushes.White, baseX + 55, baseY + 30);
                    font.Dispose();
                }
            }
        }

        private void bitmapButtonHelp_Click(object sender, EventArgs e)
        {
            MessageBoxEx.Show("通过完成事件和战斗可以使作物成熟");
        }
        public override void OnRemove()
        {
            base.OnRemove();

            miniItemView1.DisposeItem();
        }
    }
}