using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Cards.Monsters;
using TaleofMonsters.DataType.Skills;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.DataType.Cards.Weapons;

namespace TaleofMonsters.Forms.MagicBook
{
    internal sealed partial class MonsterSkillViewForm : BasePanel
    {
        private const int cardWidth = 50;
        private const int cardHeight = 50;
        private int xCount = 10;
        private int yCount = 9;
        private int cardCount;
        private int totalCount;
        private int page;
        private int tar = -1;
        private List<int> skills;
        private CardDetail cardDetail;
        private string skillDesStr = ""; //显示在下方的技能说明

        private string filterType = "全部";
        private string filterRemark = "全部";

        private string[] strTypeList = null;
        private string[] strRemarkList = null;

        private NLSelectPanel selectPanel;

        public MonsterSkillViewForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            bitmapButtonClose.NoUseDrawNine = true;
            this.bitmapButtonNext.ImageNormal = PicLoader.Read("Button.Panel", "NextButton.JPG");
            bitmapButtonNext.NoUseDrawNine = true;
            this.bitmapButtonPre.ImageNormal = PicLoader.Read("Button.Panel", "PreButton.JPG");
            bitmapButtonPre.NoUseDrawNine = true;
            comboBoxType.SelectedIndex = 0;
        }

        public override void Init(int width, int height)
        {
            #region 智能修正面板元素数量
            int borderX = 783 - cardWidth * xCount;
            int borderY = 585 - cardHeight * yCount;

            xCount = (width - borderX) * 7 / 10 / cardWidth; //0.8作为一个边缘因子
            yCount = (height - borderY) * 8 / 10 / cardHeight;
            cardCount = xCount * yCount;

            Width = xCount * cardWidth + borderX;
            Height = yCount * cardHeight + borderY + 63;//63是下方说明栏跨度

            selectPanel = new NLSelectPanel(65, 35+ cardHeight, xCount * cardWidth, yCount * cardHeight, this);
            selectPanel.ItemsPerRow = xCount;
            selectPanel.ItemHeight = cardHeight;
            selectPanel.DrawCell += SelectPanel_DrawCell;
            selectPanel.SelectIndexChanged += SelectPanel_SelectIndexChanged;
            #endregion

            base.Init(width, height);

            cardDetail = new CardDetail(this, cardWidth * xCount + 65, 35, cardHeight * yCount + 93 + cardHeight);
            nlClickLabel1.Location = new Point(75, cardHeight * yCount + 100 + cardHeight);
            nlClickLabel1.Size = new Size(cardWidth * xCount - 20, 63);

            SetupType();
            ChangeCards();
        }

        public override void OnFrame(int tick, float timePass)
        {
            cardDetail.OnFrame();
        }

        private void UpdateButtonState()
        {
            bitmapButtonPre.Enabled = page > 0;
            bitmapButtonNext.Enabled = (page + 1) * cardCount < totalCount;
        }

        private void SetupType()
        {
            Dictionary<string, bool> typeDict = new Dictionary<string, bool>();
            Dictionary<string, bool> remarkDict = new Dictionary<string, bool>();
            typeDict["全部"] = true;
            remarkDict["全部"] = true;
            foreach (var skill in ConfigData.SkillDict.Values)
            {
                if (skill.Remark != "")
                    remarkDict[skill.Remark] = true;
                if (skill.Type != "")
                    typeDict[skill.Type] = true;
            }
            strTypeList = new string[typeDict.Count];
            typeDict.Keys.CopyTo(strTypeList, 0);
            strRemarkList = new string[remarkDict.Count];
            remarkDict.Keys.CopyTo(strRemarkList, 0);
        }

        private void comboBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxValue.Items.Clear();
            var type = comboBoxType.SelectedItem.ToString();
            switch (type)
            {
                case "类别":
                    foreach (var typeStr in strTypeList)
                        comboBoxValue.Items.Add(typeStr);
                    break;
                case "特性":
                    foreach (var remarkStr in strRemarkList)
                        comboBoxValue.Items.Add(remarkStr); break;
                default:
                    comboBoxValue.Items.Add("全部"); break;
            }
            comboBoxValue.SelectedIndex = 0;
        }

        private void ChangeCards()
        {
            page = 0;
            totalCount = 0;
            tar = -1;
            cardDetail.SetInfo(-1);
            #region 数据装载
            List<IntPair> things = new List<IntPair>();
            foreach (var skill in ConfigData.SkillDict.Values)
            {
                if (filterType != "全部" && !skill.Type.Contains(filterType))
                    continue;
                if (filterRemark != "全部" && !skill.Remark.Contains(filterRemark))
                    continue;

                IntPair mt = new IntPair();
                mt.Type = skill.Id;
                mt.Value = skill.Id;
                things.Add(mt);
                totalCount++;
            }
            things.Sort(new CompareBySid());

            skills = new List<int>();
            foreach (var mt in things)
            {
                skills.Add(mt.Value);
            }
            #endregion
            UpdateButtonState();
            InitItems();
            Invalidate(new Rectangle(65, 35, cardWidth * xCount + 200, 630));
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            filterType = "全部";
            filterRemark = "全部";

            var type = comboBoxType.SelectedItem.ToString();
            switch (type)
            {
                case "类别":
                    filterType = comboBoxValue.SelectedItem.ToString(); break;
                case "特性":
                    filterRemark = comboBoxValue.SelectedItem.ToString(); break;
            }
            ChangeCards();
        }

        private void buttonPre_Click(object sender, EventArgs e)
        {
            page--;
            if (page < 0)
            {
                page++;
                return;
            }
            tar = -1;
            cardDetail.SetInfo(-1);
            UpdateButtonState();
            InitItems();
            Invalidate(new Rectangle(65, 35, cardWidth * xCount+200, 630));
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            page++;
            if (page > (totalCount - 1) / cardCount)
            {
                page--;
                return;
            }
            tar = -1;
            cardDetail.SetInfo(-1);
            UpdateButtonState();
            InitItems();
            Invalidate(new Rectangle(65, 35, cardWidth * xCount+200, 630));
        }


        private void InitItems()
        {
            int pages = totalCount / cardCount + 1;
            int cardLimit = (page < pages - 1) ? cardCount : (totalCount % cardCount);
            int former = cardCount * page + 1;
            var datas = new List<int>();
            for (int i = former - 1; i < former + cardLimit - 1; i++)
                datas.Add(skills[i]);
            selectPanel.AddContent(datas);
        }

        private void nlClickLabel1_SelectionChange(Object value)
        {
            cardDetail.SetInfo((int)value);
            Invalidate(new Rectangle(65, 35, cardWidth * xCount + 200, 630));
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SelectPanel_SelectIndexChanged()
        {
            tar = selectPanel.SelectIndex + page * cardCount;

            if (tar != -1)
            {
                Skill skill = new Skill(skills[tar]);
                skillDesStr = string.Format("{0}:{1}", skill.SkillConfig.Name, skill.Descript);
                nlClickLabel1.ClearLabel();
                foreach (int mid in MonsterBook.GetSkillMids(skill.Id))
                {
                    var cardConfig = ConfigData.GetMonsterConfig(mid);
                    var colorName = HSTypes.I2QualityColor(cardConfig.Quality);
                    if (colorName == "White")
                        colorName = "DarkGray";
                    nlClickLabel1.AddLabel(cardConfig.Name, mid, Color.FromName(colorName));
                }
                foreach (int wid in WeaponBook.GetSkillWids(skill.Id))
                {
                    var cardConfig = ConfigData.GetMonsterConfig(wid);
                    var colorName = HSTypes.I2QualityColor(cardConfig.Quality);
                    if (colorName == "White")
                        colorName = "DarkGray";
                    nlClickLabel1.AddLabel(cardConfig.Name, wid, Color.FromName(colorName));
                }
                Invalidate(new Rectangle(65 + 5, cardHeight * yCount + 37 + cardHeight + 21, cardWidth*xCount,25));
                nlClickLabel1.Invalidate();
            }
        }

        private void SelectPanel_DrawCell(Graphics g, int info, int xOff, int yOff, bool inMouseOn, bool isTarget, bool onlyBorder)
        {
            if (!onlyBorder)
            {
                var cardImg = SkillBook.GetSkillImage(info);
                if (cardImg != null)
                    g.DrawImage(cardImg, xOff, yOff, cardWidth, cardHeight);
            }
        
            if (inMouseOn || isTarget)
            {
                Color borderColor = isTarget ? Color.Lime : Color.Yellow;
                SolidBrush yellowbrush = new SolidBrush(Color.FromArgb(80, borderColor));
                g.FillRectangle(yellowbrush, xOff, yOff, cardWidth, cardHeight);
                yellowbrush.Dispose();

                Pen yellowpen = new Pen(borderColor, 3);
                g.DrawRectangle(yellowpen, xOff, yOff, cardWidth - 3, cardHeight - 3);
                yellowpen.Dispose();
            }
        }

        private void MonsterSkillViewForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(" 全技能 ", font, Brushes.White, 320, 8);
            font.Dispose();

            cardDetail.Draw(e.Graphics);
            e.Graphics.FillRectangle(Brushes.DarkGray, 67, 37, cardWidth * xCount - 4, 44);
            Image img = PicLoader.Read("System", "SearchBack.JPG");
            e.Graphics.DrawImage(img, 70, 40, cardWidth * xCount - 10, 38);
            img.Dispose();

            font = new Font("宋体", 9 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            e.Graphics.DrawString("分类", font, Brushes.White, 86, 51);
            font.Dispose();

            font = new Font("宋体", 10 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            Font fontblack = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(245, 244, 242)), 65, cardHeight * yCount + 35 + cardHeight, cardWidth * xCount, 93);
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(190, 175, 160)), 65, cardHeight * yCount + 35 + cardHeight, cardWidth * xCount, 20);
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(190, 175, 160)), 65, cardHeight * yCount + 75 + cardHeight, cardWidth * xCount, 20);
            e.Graphics.DrawString("技能说明", fontblack, Brushes.White, 65, cardHeight * yCount + 37 + cardHeight);
            if (!string.IsNullOrEmpty(skillDesStr))
            {
                e.Graphics.DrawString(skillDesStr, font, Brushes.SaddleBrown, 65+5, cardHeight * yCount + 37 + cardHeight + 21);
            }
            e.Graphics.DrawString("所有生物", fontblack, Brushes.White, 65, cardHeight * yCount + 77 + cardHeight);
            fontblack.Dispose();
            font.Dispose();
        }
    }
}