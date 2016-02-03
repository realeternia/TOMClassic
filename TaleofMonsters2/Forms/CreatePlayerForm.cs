using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using NarlonLib.Control;
using NarlonLib.Drawing;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Controler.World;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.Decks;
using NarlonLib.Math;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms
{
    internal sealed partial class CreatePlayerForm : Form
    {
        private int headId = -1;
        private int type;
        private int res;
        private int constellation;
        private HSCursor myCursor;
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private VirtualRegion virtualRegion;
        private DialogResult result;

        public CreatePlayerForm()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            myCursor = new HSCursor(this);
            virtualRegion = new VirtualRegion(this);
            virtualRegion.AddRegion(new SubVirtualRegion(1, 141, 159, 24, 24, 1));
            virtualRegion.AddRegion(new SubVirtualRegion(2, 141, 192, 24, 24, 2));
            virtualRegion.AddRegion(new SubVirtualRegion(3, 141, 225, 24, 24, 3));
            virtualRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            virtualRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        public DialogResult Result
        {
            get { return result; }
        }

        private void CreatePlayerForm_Load(object sender, EventArgs e)
        {
            headId = 1;
            pictureBoxHead.Image = PicLoader.Read("Player", "1.PNG");
            constellation = MathTool.GetRandom(12);
            type = MathTool.GetRandom(7);
            res = MathTool.GetRandom(6);
            myCursor.ChangeCursor("default");
        }

        private void buttonLeft_Click(object sender, EventArgs e)
        {
            headId += 2;
            if (headId > 32)
                headId -= 32;
            pictureBoxHead.Image = PicLoader.Read("Player", string.Format("{0}.PNG", headId));
            Invalidate();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                headId++;
                pictureBoxHead.Image = PicLoader.Read("Player", string.Format("{0}.PNG", headId));
                Invalidate();
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                headId--;
                pictureBoxHead.Image = PicLoader.Read("Player", string.Format("{0}.PNG", headId));
                Invalidate();
            }
        }

        private Profile CreateProfile()
        {
            Profile profile = new Profile();
            profile.Pid = WorldInfoManager.GetPlayerPid();
            profile.Name = UserProfile.ProfileName;
            profile.InfoBasic.Job = JobConfig.Indexer.NewBie;
            profile.InfoBasic.Constellation = constellation + 1;
            profile.InfoBasic.Face = headId;
            profile.InfoBasic.Level = 1;
            profile.InfoBasic.MapId = SceneConfig.Indexer.BornMapId;
            profile.InfoBag.BagCount = 50;
            return profile;
        }

        private void CreateCards()
        {
            DeckCard[] cds = DeckBook.GetDeckByName("rookie", 1);
            int index = 0;
            foreach (DeckCard cd in cds)
            {
                if (CardConfigManager.GetCardConfig(cd.BaseId).Id == 0)
                {
                    continue;
                }
                var dcard = UserProfile.InfoCard.AddCard(cd.BaseId);
                UserProfile.InfoCard.SelectedDeck.SetCardAt(index++, dcard.BaseId);
            }
        }

        private void buttonType_Click(object sender, EventArgs e)
        {
            type = (type + 1)%7;
            Invalidate();
        }

        private void buttonRes_Click(object sender, EventArgs e)
        {
            res = (res + 1) % 6;
            Invalidate();
        }

        private void buttonJob_Click(object sender, EventArgs e)
        {
            constellation = (constellation + 1) % 12;
            Invalidate();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            UserProfile.Profile = CreateProfile();            
            CreateCards();
            UserProfile.InfoBag.AddItem(HItemConfig.Indexer.NewbieGift, 1);//新手礼包
            UserProfile.InfoBag.AddItem(HItemConfig.Indexer.NewbieGiftCard + type, 1);//主要卡包
            UserProfile.InfoBag.AddItem(HItemConfig.Indexer.NewbieGiftRes + res, 1);//资源追加
            result = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            result = DialogResult.Cancel;
            Close();
        }

        private void virtualRegion_RegionEntered(int info, int x, int y, int key)
        {
            Image image = null;
            if (info == 1)
            {
                image = DrawTool.GetImageByString("", HSTypes.I2ConstellationTip(constellation), 150, Color.LimeGreen);
            }
            else if (info == 2)
            {
                image = DrawTool.GetImageByString("", HSTypes.I2InitialAttrTip(type), 100, Color.Gold);
            }
            else if (info == 3)
            {
                image = DrawTool.GetImageByString(HSTypes.I2Resource(res+1), 100);
            }

            if (image != null)
            {
                tooltip.Show(image, this, x + 2, y + 3);
            }

        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }

        private void CreatePlayerForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("创建角色", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            e.Graphics.DrawImage(HSIcons.GetIconsByEName(string.Format("con{0}", constellation + 1)), 141, 159, 24, 24);
            e.Graphics.DrawImage(HSIcons.GetIconsByEName(string.Format("atr{0}", type)), 141, 192, 24, 24);
            e.Graphics.DrawImage(HSIcons.GetIconsByEName(string.Format("res{0}", res + 2)), 141, 225, 24, 24);
        }
    }
}