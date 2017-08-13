using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.Core;
using TaleofMonsters.Forms.Items.Core;
using ConfigDatas;
using ControlPlus;

namespace TaleofMonsters.Forms.MagicBook
{
    internal sealed partial class CardViewForm : BasePanel
    {
        private const int cardWidth = 67;
        private const int cardHeight = 90;
        private int xCount = 8;
        private int yCount = 5;
        private int cardCount;
        private int page;
        private int tar = -1;
        private List<int> cards;
        private CardDetail cardDetail;

        private int filterLevel=0;
        private int filterQual=-1;
        private int filterType = -1;
        private string filterTypeSub = "全部";
        private int filterJob = -1;
        private int filterEle = -1;
        private string filterRemark = "全部";

        private NLSelectPanel selectPanel;

        public CardViewForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            bitmapButtonClose.NoUseDrawNine = true;
            this.bitmapButtonNext.ImageNormal = PicLoader.Read("Button.Panel", "NextButton.JPG");
            bitmapButtonNext.NoUseDrawNine = true;
            this.bitmapButtonPre.ImageNormal = PicLoader.Read("Button.Panel", "PreButton.JPG");
            bitmapButtonPre.NoUseDrawNine = true;
        }

        public override void Init(int width, int height)
        {
            #region 智能修正面板元素数量
            //823,575 x=8,y=5
            int borderX = 823 - cardWidth * xCount;
            int borderY = 575 - cardHeight * yCount;

            xCount = (width - borderX) * 8 / 10 / cardWidth; //0.8作为一个边缘因子
            yCount= (height - borderY) * 9 / 10 / cardHeight;
            cardCount = xCount*yCount;

            Width = xCount * cardWidth + borderX;
            Height = yCount * cardHeight + borderY;

            selectPanel = new NLSelectPanel(65, 110, xCount * cardWidth, yCount * cardHeight, this);
            selectPanel.ItemsPerRow = xCount;
            selectPanel.ItemHeight = cardHeight;
            selectPanel.DrawCell += SelectPanel_DrawCell;
            selectPanel.SelectIndexChanged += SelectPanel_SelectIndexChanged;

            selectPanel.UseCache = true;
            #endregion

            base.Init(width, height);
            cards = new List<int>();
            comboBoxCatalog.SelectedIndex = 0;
            cardDetail = new CardDetail(this, cardWidth * xCount + 65, 35, cardHeight * yCount + 70);
            ChangeCards();
        }

        public override void OnFrame(int tick, float timePass)
        {
            cardDetail.OnFrame();
        }

        private void comboBoxCatalog_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxValue.Items.Clear();
            var type = comboBoxCatalog.SelectedItem.ToString();
            switch (type)
            {
                case "分类":
                    comboBoxValue.Items.AddRange(new object[] { "全部", "Yellow|生物", "Red|武器", "DodgerBlue|法术" }); break;
                case "-细分":
                    comboBoxValue.Items.AddRange(new object[] { "全部", "恶魔","机械","精灵","昆虫","龙","鸟",
                        "爬行","人类","兽人","亡灵","野兽","鱼","元素","植物","地精","石像",
                        "Red|武器","Red|卷轴","Red|防具", "Red|饰品",
                        "DodgerBlue|单体法术","DodgerBlue|群体法术","DodgerBlue|基本法术","DodgerBlue|地形变化" }); break;
                case "品质":
                    comboBoxValue.Items.AddRange(new object[] { "全部", "普通", "Green|良好", "DodgerBlue|优秀", "Violet|史诗", "Orange|传说" }); break;
                case "星级":
                    comboBoxValue.Items.AddRange(new object[] { "全部", "★", "★★", "★★★", "★★★★", "★★★★★", "★★★★★★", "Gold|★x7" }); break;
                case "职业":
                    comboBoxValue.Items.Add("全部");
                    foreach (var configData in ConfigData.JobDict.Values)
                    {
                        if (!configData.IsSpecial)
                            comboBoxValue.Items.Add(configData.Color +"|"+ configData.Name);
                    } break;
                case "元素":
                      comboBoxValue.Items.AddRange(new object[] { "全部", "无", "Aqua|水", "Green|风", "Red|火", "Peru|地", "Gold|光", "DimGray|暗" }); break;
                case "标签":
                    comboBoxValue.Items.AddRange(new object[] { "全部", "基本","Red|直伤","范围","状态","Gold|治疗","手牌","Aqua|魔法","属性","召唤","陷阱" }); break;
            }
            comboBoxValue.SelectedIndex = 0;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            filterLevel = 0;
            filterQual = -1;
            filterType = -1;
            filterTypeSub = "全部";
            filterJob = -1;
            filterEle = -1;
            filterRemark = "全部";

            var type = comboBoxCatalog.SelectedItem.ToString();
            switch (type)
            {
                case "分类":
                    filterType = comboBoxValue.SelectedIndex - 1; break;
                case "-细分":
                    filterTypeSub = comboBoxValue.TargetText; break;
                case "星级":
                    filterLevel = comboBoxValue.SelectedIndex; break;
                case "职业":
                    foreach (var configData in ConfigData.JobDict.Values)
                    {
                        if (configData.Name == comboBoxValue.TargetText)
                        {
                            filterJob = configData.Id;
                        }
                    } break;
                case "标签":
                    filterRemark = comboBoxValue.TargetText; break;
                case "品质":
                    filterQual = comboBoxValue.SelectedIndex - 1; break;
                case "元素":
                    filterEle = comboBoxValue.SelectedIndex - 1; break;
            }
            ChangeCards();
        }

        private void ChangeCards()
        {
            page = 0;
            cards.Clear();
            tar = -1;
            cardDetail.SetInfo(-1);

            List<CardConfigData> configData = new List<CardConfigData>();
            #region 数据装载

            if (filterType == -1 || filterType==0)
            {
                foreach (var monsterConfig in ConfigData.MonsterDict.Values)
                {
                    if (monsterConfig.IsSpecial > 0)
                        continue;
                    if (filterJob != -1 && monsterConfig.JobId != filterJob)
                        continue;
                    if (filterLevel != 0 && monsterConfig.Star != filterLevel)
                        continue;
                    if (filterQual != -1 && monsterConfig.Quality != filterQual)
                        continue;
                    if (filterEle != -1 && monsterConfig.Attr != filterEle)
                        continue;
                    if (filterRemark != "全部" && (string.IsNullOrEmpty(monsterConfig.Remark) || !monsterConfig.Remark.Contains(filterRemark)))
                        continue;
                    if (filterTypeSub != "全部" && HSTypes.I2CardTypeSub(monsterConfig.Type) != filterTypeSub)
                        continue;
                    var cardData = CardConfigManager.GetCardConfig(monsterConfig.Id);
                    configData.Add(cardData);
                }
            }
            if (filterType == -1 || filterType == 1)
            {
                foreach (var weaponConfig in ConfigData.WeaponDict.Values)
                {
                    if (weaponConfig.IsSpecial > 0)
                        continue;
                    if (filterJob != -1 && weaponConfig.JobId != filterJob)
                        continue;
                    if (filterLevel != 0 && weaponConfig.Star != filterLevel)
                        continue;
                    if (filterQual != -1 && weaponConfig.Quality != filterQual)
                        continue;
                    if (filterEle != -1 && weaponConfig.Attr != filterEle)
                        continue;
                    if (filterRemark != "全部" && (string.IsNullOrEmpty(weaponConfig.Remark) || !weaponConfig.Remark.Contains(filterRemark)))
                        continue;
                    if (filterTypeSub != "全部" && HSTypes.I2CardTypeSub(weaponConfig.Type) != filterTypeSub)
                        continue;
                    var cardData = CardConfigManager.GetCardConfig(weaponConfig.Id);
                    configData.Add(cardData);
                }
            }
            if (filterType == -1 || filterType == 2)
            {
                foreach (var spellConfig in ConfigData.SpellDict.Values)
                {
                    if (spellConfig.IsSpecial > 0)
                        continue;
                    if (filterJob != -1 && spellConfig.JobId != filterJob)
                        continue;
                    if (filterLevel != 0 && spellConfig.Star != filterLevel)
                        continue;
                    if (filterQual != -1 && spellConfig.Quality != filterQual)
                        continue;
                    if (filterEle != -1 && spellConfig.Attr != filterEle)
                        continue;
                    if (filterRemark != "全部" && (string.IsNullOrEmpty(spellConfig.Remark) || !spellConfig.Remark.Contains(filterRemark)))
                        continue;
                    if (filterTypeSub != "全部" && HSTypes.I2CardTypeSub(spellConfig.Type) != filterTypeSub)
                        continue;
                    var cardData = CardConfigManager.GetCardConfig(spellConfig.Id);
                    configData.Add(cardData);
                }
            }

            #endregion

            configData.Sort(new CompareByCard());
            cards = configData.ConvertAll(card => card.Id);

            UpdateButtonState();
            InitItems();

            if (cards.Count > 0)
                cardDetail.SetInfo(cards[0]);
            Invalidate(new Rectangle(65, 110, cardWidth * xCount, cardHeight * yCount));
        }

        private void UpdateButtonState()
        {
            bitmapButtonPre.Enabled = page > 0;
            bitmapButtonNext.Enabled = (page + 1) * cardCount < cards.Count;
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
            Invalidate(new Rectangle(65, 110, cardWidth*xCount, cardHeight*yCount));
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            page++;
            if (page > (cards.Count - 1) / cardCount)
            {
                page--;
                return;
            }
            tar = -1;
            cardDetail.SetInfo(-1);
            UpdateButtonState();
            InitItems();
            Invalidate(new Rectangle(65, 110, cardWidth * xCount, cardHeight * yCount));
        }

        private void InitItems()
        {
            int pages = cards.Count / cardCount + 1;
            int cardLimit = (page < pages - 1) ? cardCount : (cards.Count % cardCount);
            int former = cardCount * page + 1;
            var datas = new List<int>();
            for (int i = former - 1; i < former + cardLimit - 1; i++)
                datas.Add(cards[i]);
            selectPanel.AddContent(datas);
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
                cardDetail.SetInfo(cards[tar]);
                Invalidate(new Rectangle(65 + cardWidth * xCount, 35, 200, 525));
            }
        }

        private void SelectPanel_DrawCell(Graphics g, int info, int xOff, int yOff, bool inMouseOn, bool isTarget, bool onlyBorder)
        {
            if (!onlyBorder)
                CardAssistant.DrawBase(g, info, xOff, yOff, cardWidth, cardHeight);

            if (inMouseOn)
            {
                var brushes = new SolidBrush(Color.FromArgb(130, Color.Yellow));
                g.FillRectangle(brushes, xOff, yOff, cardWidth, cardHeight);
                brushes.Dispose();
            }

            if (!onlyBorder)
            {
                var cardConfigData = CardConfigManager.GetCardConfig(info);
                Font font = new Font("宋体", 5 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                g.DrawString(("★★★★★★★★★★").Substring(10 - cardConfigData.Star), font, Brushes.Yellow, xOff + 3, yOff + 3);
                font.Dispose();

                var quality = cardConfigData.Quality + 1;
                g.DrawImage(HSIcons.GetIconsByEName("gem" + (int)quality), xOff + cardWidth / 2 - 8, yOff + cardHeight - 20, 16, 16);

                var jobId = cardConfigData.JobId;
                if (jobId > 0)
                {
                    var jobConfig = ConfigData.GetJobConfig(jobId);
                    Brush brush = new SolidBrush(Color.FromName(jobConfig.Color));
                    g.FillRectangle(brush, xOff + cardWidth - 24, yOff + 4, 20, 20);
                    g.DrawImage(HSIcons.GetIconsByEName("job" + jobConfig.JobIndex), xOff + cardWidth - 24, yOff + 4, 20, 20);
                    brush.Dispose();
                }
            }
         
        }
        
        private void CardViewForm_Click(object sender, EventArgs e)
        {

        }

        private void CardViewForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("全卡片", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            cardDetail.Draw(e.Graphics);
            e.Graphics.FillRectangle(Brushes.DarkGray, 67, 37, cardWidth * xCount - 4, 71);
            Image img = PicLoader.Read("System", "SearchBack.JPG");
            e.Graphics.DrawImage(img, 70, 40, cardWidth * xCount - 10, 65);
            img.Dispose();

            font = new Font("宋体", 9 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            e.Graphics.DrawString("分类", font, Brushes.White, 86, 61);
            font.Dispose();
        }

        private class CompareByCard : IComparer<CardConfigData>
        {
            #region IComparer<int> 成员

            public int Compare(CardConfigData x, CardConfigData y)
            {
                if (x.Star != y.Star)
                {
                    return x.Star.CompareTo(y.Star);
                }

                if (x.Quality != y.Quality)
                {
                    return x.Quality.CompareTo(y.Quality);
                }
                if (x.Attr != y.Attr)
                {
                    return x.Attr.CompareTo(y.Attr);
                }

                return x.Id.CompareTo(y.Id);
            }

            #endregion
        }
    }

}