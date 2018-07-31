using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Cards.Monsters;
using TaleofMonsters.Datas.Cards.Weapons;
using TaleofMonsters.Datas.Skills;
using TaleofMonsters.Forms.Items.Core;

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

            cardDetail = new CardDetail(this, cardWidth * xCount + 65, 35, cardHeight * yCount + 101 + cardHeight);
            nlClickLabel1.Location = new Point(75, cardHeight * yCount + 100 + cardHeight);
            nlClickLabel1.Size = new Size(cardWidth * xCount - 20, 71);

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
                if (!string.IsNullOrEmpty(skill.Remark))
                {
                    if (skill.Remark.Contains(","))
                    {
                        foreach (var checkRemark in skill.Remark.Split(','))
                            remarkDict[checkRemark] = true;
                    }
                    else
                    {
                        remarkDict[skill.Remark] = true;
                    }
                }

                if (!string.IsNullOrEmpty(skill.Type))
                {
                    if (skill.Type != "特殊" && skill.Type != "道具")
                        remarkDict[skill.Type] = true; //type也算是remark
                    typeDict[skill.Type] = true;
                }
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
            List<IntPair> skillList = new List<IntPair>();
            foreach (var skill in ConfigData.SkillDict.Values)
            {
                if (filterType != "全部" && skill.Type != filterType)
                    continue;
                if (filterRemark != "全部" && skill.Type != filterRemark && !skill.Remark.Contains(filterRemark))
                    continue;

                IntPair checkItem = new IntPair();
                checkItem.Type = skill.Id;
                checkItem.Value = skill.Id;
                skillList.Add(checkItem);
                totalCount++;
            }
            skillList.Sort(new CompareBySid());

            skills = skillList.ConvertAll(sd => sd.Value);
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
                var skillId = skills[tar];
                var skillDes = SkillBook.GetSkillDes(skillId, 1);
                skillDesStr = string.Format("{0}:{1}", ConfigData.GetSkillConfig(skillId).Name, skillDes);
                nlClickLabel1.ClearLabel();
                foreach (int mid in MonsterBook.GetSkillMids(skillId))
                {
                    var cardConfig = ConfigData.GetMonsterConfig(mid);
                    var colorName = HSTypes.I2QualityColor(cardConfig.Quality);
                    if (colorName == "White")
                        colorName = "DarkGray";
                    nlClickLabel1.AddLabel(cardConfig.Name, mid, Color.FromName(colorName));
                }
                foreach (int wid in WeaponBook.GetSkillWids(skillId))
                {
                    var cardConfig = ConfigData.GetWeaponConfig(wid);
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
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(245, 244, 242)), 65, cardHeight * yCount + 35 + cardHeight, cardWidth * xCount, 101);
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(190, 175, 160)), 65, cardHeight * yCount + 35 + cardHeight, cardWidth * xCount, 20);
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(190, 175, 160)), 65, cardHeight * yCount + 75 + cardHeight, cardWidth * xCount, 20);
            e.Graphics.DrawString("技能说明", fontblack, Brushes.White, 65, cardHeight * yCount + 37 + cardHeight);
            if (!string.IsNullOrEmpty(skillDesStr))
                e.Graphics.DrawString(skillDesStr, font, Brushes.SaddleBrown, 65+5, cardHeight * yCount + 37 + cardHeight + 21);
            e.Graphics.DrawString("所有生物", fontblack, Brushes.White, 65, cardHeight * yCount + 77 + cardHeight);
            fontblack.Dispose();
            font.Dispose();
        }
    }
}