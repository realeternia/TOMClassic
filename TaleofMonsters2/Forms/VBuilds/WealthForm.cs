using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Drops;
using TaleofMonsters.Datas.Effects;
using TaleofMonsters.Datas.Effects.Facts;
using TaleofMonsters.Datas.Items;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Rpc;

namespace TaleofMonsters.Forms.VBuilds
{
    internal sealed partial class WealthForm : BasePanel
    {
        private bool showImage;
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private VirtualRegion vRegion;
        private VirtualRegionMoveMediator moveMediator;
        private const int WealthMaxHp = 20;

        public WealthForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            this.bitmapButtonHelp.ImageNormal = PicLoader.Read("Button.Panel", "LearnButton.JPG");

            this.bitmapButtonC1.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonC1.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("tsk2");
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            showImage = true;
            vRegion = new VirtualRegion(this);
            
            vRegion.AddRegion(new ImageRegion(100, 210, 100, 160, 160, ImageRegionCellType.None, PicLoader.Read("Build.Wealth", "box.PNG")));
            for (int i = 0; i < 15; i++)
                vRegion.AddRegion(new PictureRegion(i+1, 36 + (i%3)*48, 90 + (i/ 3) * 48, 40, 40, PictureRegionCellType.Item, 0));

            var dropConfig = ConfigData.GetDropConfig(DropBook.GetDropId("dlhaidaowan"));
            for (int i = 0; i < dropConfig.Items.Length; i++)
                vRegion.SetRegionKey(i + 1, HItemBook.GetItemId(dropConfig.Items[i]));

            if (UserProfile.InfoCastle.WealthHpLeft <= 0)
                UserProfile.InfoCastle.WealthHpLeft = WealthMaxHp;

            moveMediator = new VirtualRegionMoveMediator(vRegion);

            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            if (id < 100 && key > 0)
            {
                Image image = HItemBook.GetPreview(key);
                tooltip.Show(image, this, x, y);
            }

        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void WealthForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("海盗湾", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            if (!showImage)
                return;

            Image back = PicLoader.Read("Build", "wealth.JPG");
            e.Graphics.DrawImage(back, 15, 40, 572, 352);
            back.Dispose();

            vRegion.Draw(e.Graphics);

            font = new Font("宋体", 9 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
         
            var hpTotal = WealthMaxHp;
            var hpLeft = UserProfile.InfoCastle.WealthHpLeft;
            e.Graphics.FillRectangle(Brushes.Red, 210, 88, 160, 12);
            e.Graphics.FillRectangle(Brushes.Lime, 210, 88, 160*hpLeft/hpTotal, 12);
            e.Graphics.DrawString(string.Format("血量 {0}/{1}", hpLeft, hpTotal), font, Brushes.Brown, 210 + 50, 88);

            Brush b = new SolidBrush(Color.FromArgb(200, Color.Black));
            e.Graphics.FillRectangle(b, 30, 57, 70, 20);
            e.Graphics.DrawString("掉落列表", font, Brushes.White, 33, 60);
            e.Graphics.FillRectangle(b, 30, 350, 100, 20);
            e.Graphics.DrawString(string.Format("挖宝次数 {0}", UserProfile.InfoCastle.WealthPoint), font, Brushes.White, 33, 353);
            font.Dispose();
            b.Dispose();
        }

        private void bitmapButtonC1_Click(object sender, EventArgs e)
        {
            if (UserProfile.InfoCastle.WealthHpLeft <= 0)
                return;

            if (UserProfile.InfoCastle.WealthPoint <= 0)
            {
                AddFlowCenter("挖宝次数不足", "Red");
                return;
            }

            var effect = new StaticUIEffect(EffectBook.GetEffect("yellowsplash"), new Point(210+30, 100+30), new Size(100, 100));
            effect.Repeat = false;
            AddEffect(effect);
            moveMediator.FireShake(100);

            UserProfile.InfoCastle.WealthHpLeft--;
            AddFlowCenter("-1", "Red"); //生命-1

            if (UserProfile.InfoCastle.WealthHpLeft == 0)
            {
                var itemList = DropBook.GetDropItemList("dlhaidaowan");
                foreach (var itemId in itemList)
                {
                    AddFlowCenter("+1", "Lime", HItemBook.GetHItemImage(itemId));
                    UserProfile.InfoBag.AddItem(itemId, 1);
                }
                UserProfile.InfoCastle.WealthHpLeft = WealthMaxHp;
                moveMediator.FireFadeOut(100);
                TalePlayer.Start(DelayRevive());
            }

            UserProfile.InfoCastle.WealthPoint--;
            Invalidate();
        }

        private IEnumerator DelayRevive()
        {
            yield return new NLWaitForSeconds(1f);
            vRegion.SetRegionDecorator(100, 0, null);
            Invalidate();
        }

        private void bitmapButtonHelp_Click(object sender, EventArgs e)
        {
            MessageBoxEx.Show("通过完成事件和战斗可以累积能量");
        }
    }
}