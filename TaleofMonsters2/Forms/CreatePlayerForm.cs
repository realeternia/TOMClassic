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
using TaleofMonsters.DataType.Others;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;

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

        private const int CellSize = 32; 

        public CreatePlayerForm()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(MainForm.Instance.Location.X + MainForm.Instance.Size.Width / 2 - Size.Width / 2, MainForm.Instance.Location.Y
                + MainForm.Instance.Size.Height / 2 - Size.Height / 2);

            myCursor = new HSCursor(this);
            vRegion = new VirtualRegion(this);
            for (int i = 1; i <= 24; i++)
            {
                var dnaConfig = ConfigData.GetPlayerDnaConfig(i);
                vRegion.AddRegion(new SubVirtualRegion(i, 30 + 36*(dnaConfig.X), 150 + 36*(dnaConfig.Y), CellSize, CellSize));
            }

            vRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            vRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
            vRegion.RegionClicked += VRegionOnRegionClicked;
            vRegion.CellDrawBefore += VRegion_CellDraw;

            DoubleBuffered = true;
        }

        public DialogResult Result
        {
            get { return result; }
        }

        private void CreatePlayerForm_Load(object sender, EventArgs e)
        {
            headId = 1;
            pictureBoxHead.Image = PicLoader.Read("Player", "1.PNG");

            for (int i = 0; i < 6; i++)
                SetDnaState(MathTool.GetRandom(1 + i*2, 3 + i*2), true);
            SetDnaState(MathTool.GetRandom(13, 16), true);
            SetDnaState(MathTool.GetRandom(16, 19), true);
            SetDnaState(MathTool.GetRandom(19, 23), true);

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
            UserProfile.Profile.OnCreate(dna, headId);        
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
            {
                tooltip.Show(DnaBook.GetPreview(id), this, x, y);
            }
        }


        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }

        private void VRegionOnRegionClicked(int id, int x, int y, MouseButtons button)
        {
            var old = dna & (uint)Math.Pow(2, id);
            SetDnaState(id, old == 0);
            var dnaConfig = ConfigData.GetPlayerDnaConfig(id);
            if (dnaConfig.MutexId != null)
            {
                foreach (var mid in dnaConfig.MutexId)
                    SetDnaState(mid, false);
            }
            Invalidate();
        }

        private void SetDnaState(int id, bool check)
        {
            if (check) //选中
            {
                dna |= (uint) Math.Pow(2, id);
                vRegion.SetRegionDecorator(id, 0, new RegionImageDecorator(HSIcons.GetIconsByEName("round"), 32));
            }
            else //取消
            {
                dna &= ~(uint) Math.Pow(2, id);
                vRegion.SetRegionDecorator(id, 0, null);
            }
        }

        private void VRegion_CellDraw(int id, int x, int y, int key, Graphics g)
        {
            var dnaConfig = ConfigData.GetPlayerDnaConfig(id);
            var pen = new Pen(Color.DodgerBlue, 4);
            var halfSize = 36/2;
            if (dnaConfig.LineType == 1)
                g.DrawLine(pen, x + halfSize, y + halfSize, x, y + halfSize);
            else if (dnaConfig.LineType == 2)
                g.DrawLine(pen, x + halfSize, y + halfSize, x + halfSize*2, y + halfSize);
            else if (dnaConfig.LineType == 3)
                g.DrawLine(pen, x, y + halfSize, x + halfSize * 2, y + halfSize);
            pen.Dispose();

            var img = DnaBook.GetDnaImage(id);
            g.DrawImage(img, x+2, y+2, CellSize-4, CellSize-4);
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