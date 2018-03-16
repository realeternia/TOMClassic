using System;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using TaleofMonsters.Core;
using NarlonLib.Math;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Decks;
using TaleofMonsters.Datas.Items;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;
using TaleofMonsters.Tools;

namespace TaleofMonsters.Forms
{
    internal sealed partial class CreatePlayerForm : Form
    {
        private int headId = -1;
        private uint dna;
        private HSCursor myCursor;
        private ImageToolTip tooltip = SystemToolTip.Instance;
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

            string[] nameHead = {"伟大的", "神秘的", "威猛的", "快乐的", "神奇的", "灵活的"};
            string[] nameMiddle = { "火", "水", "蓝", "绿", "雷", "黑" };
            string[] nameEnd = { "德", "斯", "卡", "隆", "里", "拉" };
            textBoxName.Text = nameHead[MathTool.GetRandom(nameHead.Length)] +
                               nameMiddle[MathTool.GetRandom(nameMiddle.Length)] +
                               nameEnd[MathTool.GetRandom(nameEnd.Length)];
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
            var resultD = NameChecker.CheckName(textBoxName.Text, GameConstants.RoleNameLengthMin, GameConstants.RoleNameLengthMax);
            if (resultD != NameChecker.NameCheckResult.Ok)
            {
                if (resultD == NameChecker.NameCheckResult.NameLengthError)
                    MessageBoxEx.Show("角色名需要在2-6个字之内");
                else if (resultD == NameChecker.NameCheckResult.PunctuationOnly)
                    MessageBoxEx.Show("不能仅包含标点符号");
                return;
            }

            UserProfile.Profile.OnCreate(textBoxName.Text, dna, headId);
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
                tooltip.Show(DnaBook.GetPreview(id), this, x, y);
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