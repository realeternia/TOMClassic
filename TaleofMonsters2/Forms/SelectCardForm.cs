using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Forms.Items.Core;
using ConfigDatas;
using TaleofMonsters.Config;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.Forms
{
    internal sealed partial class SelectCardForm : BasePanel
    {
        private NLSelectPanel selectPanel;
        private List<int> cardIdList;
        private int selectDeckIndex; //1,2,3
        public int SceneQuestId { get; set; }

        public SelectCardForm()
        {
            InitializeComponent();

            bitmapButtonSelect.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonSelect.Font = new Font("宋体", 8*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonSelect.ForeColor = Color.White;
            bitmapButtonSelect.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("oth2");
            bitmapButtonSelect.IconSize = new Size(16, 16);
            bitmapButtonSelect.IconXY = new Point(8, 5);
            bitmapButtonSelect.TextOffX = 8;

            bitmapButtonSelect2.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonSelect2.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonSelect2.ForeColor = Color.White;
            bitmapButtonSelect2.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("oth2");
            bitmapButtonSelect2.IconSize = new Size(16, 16);
            bitmapButtonSelect2.IconXY = new Point(8, 5);
            bitmapButtonSelect2.TextOffX = 8;


            bitmapButtonSelect3.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonSelect3.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonSelect3.ForeColor = Color.White;
            bitmapButtonSelect3.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("oth2");
            bitmapButtonSelect3.IconSize = new Size(16, 16);
            bitmapButtonSelect3.IconXY = new Point(8, 5);
            bitmapButtonSelect3.TextOffX = 8;

            selectPanel = new NLSelectPanel(3, 38, 600, 300-30, this);
            selectPanel.ItemHeight = 300;
            selectPanel.ItemsPerRow = 3;
            selectPanel.SelectIndexChanged += selectPanel_SelectedIndexChanged;
            selectPanel.DrawCell += selectPanel_DrawCell;
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            cardIdList = new List<int>();
            var sceneQuestConfig = ConfigData.GetSceneQuestConfig(SceneQuestId);
            for (int i = 0; i < 3; i++) //3副牌
            {
                cardIdList.Add(CardConfigManager.GetRateCardStr(sceneQuestConfig.DeckCardAttr1, null));
                cardIdList.Add(CardConfigManager.GetRateCardStr(sceneQuestConfig.DeckCardAttr2, null));
                cardIdList.Add(CardConfigManager.GetRateCardStr(sceneQuestConfig.DeckCardAttr3, null));
            }
            RefreshInfo();
        }

        private void RefreshInfo()
        {
            var datas = new List<int>();
            for (int i = 1; i <= 3; i++)
                datas.Add(i);
            selectPanel.AddContent(datas);
            selectPanel.SelectIndex = 0;
        }

        private void selectPanel_SelectedIndexChanged()
        {
            selectDeckIndex = selectPanel.SelectIndex + 1;
            Invalidate();
        }

        private void selectPanel_DrawCell(Graphics g, int info, int xOff, int yOff, bool inMouseOn, bool isTarget, bool onlyBorder)
        {
            if (inMouseOn)
            {
                g.FillRectangle(Brushes.DarkGreen, xOff, yOff, 200, 300 - 30);
            }
            g.DrawRectangle(Pens.Thistle, 1 + xOff, yOff, 200 - 2, 300 - 30);

            if (!onlyBorder)
            {
                var cardStart = (info - 1)*3;
                Font font = new Font("微软雅黑", 11 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                Font fontStar = new Font("微软雅黑", 15 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                for (int i = 0; i < 3; i++)
                {
                    var border = PicLoader.Read("Border", "cardborder.PNG");
                    g.DrawImage(border, xOff, yOff + 10 + i * 40, 196, 32);
                    border.Dispose();

                    var cardId = cardIdList[i+cardStart];
                    var img = DataType.Cards.CardAssistant.GetCardImage(cardId, 80, 80);
                    g.DrawImage(img, new Rectangle(xOff+110, yOff+15 + i * 40, 80, 22), 0, 6, 80, 24, GraphicsUnit.Pixel);

                    var mask = PicLoader.Read("Border", "cardmask.PNG");
                    g.DrawImage(mask, xOff + 110, yOff + 15 + i * 40, 80, 22);
                    mask.Dispose();

                    var cardData = CardConfigManager.GetCardConfig(cardId);
                    Color color = Color.FromName(HSTypes.I2QualityColor((int)cardData.Quality));
                    g.DrawString(cardData.Star.ToString(), fontStar, Brushes.Gold, xOff + 5, yOff + 12 + i * 40);
                    var brush = new SolidBrush(color);
                    g.DrawString(cardData.Name, font, brush, xOff+30, yOff + 15 + i * 40);
                    brush.Dispose();
                }
                fontStar.Dispose();
                font.Dispose();
            }
       
        }

        private void SelectCardForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("选择卡牌", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();
        }

        private void bitmapButtonSelect_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 3; i++)
                UserProfile.InfoCard.AddDungeonCard(cardIdList[selectDeckIndex*3-3+i]);
            Close();
        }
    }
}