using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using NarlonLib.Control;
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
        private uint dna;
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
            for (int i = 1; i <= 24; i++)
                vRegion.AddRegion(new PictureRegion(1, 25 + 30 * (i%5), 150 + 30 * (i/5), 24, 24, PictureRegionCellType.Dna, i));

            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
            vRegion.RegionClicked += VRegionOnRegionClicked;
        }


        public DialogResult Result
        {
            get { return result; }
        }

        private void CreatePlayerForm_Load(object sender, EventArgs e)
        {
            headId = 1;
            pictureBoxHead.Image = PicLoader.Read("Player", "1.PNG");
            //todo 随机下
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
            profile.OnCreate(dna, headId);
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

        private void buttonOk_Click(object sender, EventArgs e)
        {
            UserProfile.Profile = CreateProfile();            
            CreateCards();
            UserProfile.InfoBag.AddItem(HItemBook.GetItemId("xinshoulibao"), 1);//新手礼包
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
            var region = vRegion.GetRegion(id);
            if (region != null)
                region.ShowTip(tooltip, this, x, y);
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }

        private void VRegionOnRegionClicked(int id, int i, int i1, MouseButtons button)
        {
        }

        private void CreatePlayerForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("创建角色", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            if(vRegion != null)
                vRegion.Draw(e.Graphics);
        }
    }
}