using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using NarlonLib.Control;
using NarlonLib.Drawing;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.Decks;
using NarlonLib.Math;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms
{
    internal sealed partial class CreatePlayerForm : Form
    {
        private int headId = -1;
        private int type;
        private int bldType; //血型
        private int constellation; //星座
        private HSCursor myCursor;
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private VirtualRegion vRegion;
        private DialogResult result;

        public CreatePlayerForm()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            myCursor = new HSCursor(this);
            vRegion = new VirtualRegion(this);
            vRegion.AddRegion(new SubVirtualRegion(1, 141, 159, 24, 24));
            vRegion.AddRegion(new SubVirtualRegion(2, 141, 192, 24, 24));
            vRegion.AddRegion(new SubVirtualRegion(3, 141, 225, 24, 24));
            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
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
            bldType = MathTool.GetRandom(4);
            myCursor.ChangeCursor("default");
        }

        private void buttonLeft_Click(object sender, EventArgs e)
        {
            headId ++;
            if (headId > 10)
                headId -= 10;
            pictureBoxHead.Image = PicLoader.Read("Player", string.Format("{0}.PNG", headId));
            Invalidate();
        }

        private Profile CreateProfile()
        {
            Profile profile = new Profile();
            profile.OnCreate(constellation, bldType, headId);
            return profile;
        }

        private void CreateCards()
        {
            DeckCard[] rookieDeck = DeckBook.GetDeckByName("rookie", 1);
            int index = 0;
            foreach (var checkCard in rookieDeck)
            {
                if (CardConfigManager.GetCardConfig(checkCard.BaseId).Id == 0)
                    continue;
                var dcard = UserProfile.InfoCard.AddCard(checkCard.BaseId);
                UserProfile.InfoCard.SelectedDeck.SetCardAt(index++, dcard.BaseId);
            }
            #region 把所有基础卡牌都给玩家
            
            foreach (var config in ConfigData.MonsterDict.Values)
            {
                if (config.Remark.Contains("基本") && UserProfile.InfoCard.GetCardCount(config.Id) == 0)
                    UserProfile.InfoCard.AddCard(config.Id);
            }
            foreach (var config in ConfigData.WeaponDict.Values)
            {
                if (config.Remark.Contains("基本") && UserProfile.InfoCard.GetCardCount(config.Id) == 0)
                    UserProfile.InfoCard.AddCard(config.Id);
            }
            foreach (var config in ConfigData.SpellDict.Values)
            {
                if (config.Remark.Contains("基本") && UserProfile.InfoCard.GetCardCount(config.Id) == 0)
                    UserProfile.InfoCard.AddCard(config.Id);
            }

            #endregion
        }

        private void buttonType_Click(object sender, EventArgs e)
        {
            type = (type + 1)%7;
            Invalidate();
        }

        private void buttonRes_Click(object sender, EventArgs e)
        {
            bldType = (bldType + 1) % 4;
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
            UserProfile.InfoBag.AddItem(HItemBook.GetItemId("xinshoulibao"), 1);//新手礼包
            UserProfile.InfoBag.AddItem(HItemBook.GetItemId("scwu1") + type*10, 10);//属性石
            result = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            result = DialogResult.Cancel;
            Close();
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            Image image = null;
            if (id == 1)
                image = DrawTool.GetImageByString("", HSTypes.I2ConstellationTip(constellation), 150, Color.LimeGreen);
            else if (id == 2)
                image = DrawTool.GetImageByString("", HSTypes.I2InitialAttrTip(type), 140, Color.Gold);
            else if (id == 3)
                image = DrawTool.GetImageByString("", HSTypes.I2BloodTypeTip(bldType), 150, Color.LimeGreen);

            if (image != null)
                tooltip.Show(image, this, x + 2, y + 3);

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
            e.Graphics.DrawImage(HSIcons.GetIconsByEName(string.Format("bld{0}", bldType+1)), 141, 225, 24, 24);
        }
    }
}