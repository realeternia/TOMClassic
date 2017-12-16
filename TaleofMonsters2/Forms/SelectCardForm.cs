using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Forms.Items.Core;
using ConfigDatas;
using TaleofMonsters.Config;

namespace TaleofMonsters.Forms
{
    internal sealed partial class SelectCardForm : BasePanel
    {
        private NLSelectPanel selectPanel;
        private int baseid;
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

            selectPanel = new NLSelectPanel(8, 38, 486, 300, this);
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
            if (isTarget)
            {
                g.FillRectangle(Brushes.DarkGreen, xOff, yOff, 162, 300);
            }
            else if (inMouseOn)
            {
                g.FillRectangle(Brushes.DarkCyan, xOff, yOff, 162, 300);
            }
            g.DrawRectangle(Pens.Thistle, 1 + xOff, yOff, 162 - 2, 300 - 4);

            if (!onlyBorder)
            {
                var cardStart = (info - 1)*3;
                Font font = new Font("微软雅黑", 11.25F * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                for (int i = 0; i < 3; i++)
                {
                    var cardId = cardIdList[i+cardStart];
                    var img = DataType.Cards.CardAssistant.GetCardImage(cardId, 32, 32);
                    g.DrawImage(img, xOff, yOff + i * 50, 32, 32);

                    var cardData = CardConfigManager.GetCardConfig(cardId);
                    string cardBorder = DataType.Cards.CardAssistant.GetCardBorder(cardData);
                    var borderImg = PicLoader.Read("Border", cardBorder);
                    g.DrawImage(borderImg, xOff, yOff+i*50, 32, 32);
                    borderImg.Dispose();

                    g.DrawString(cardData.Name, font, Brushes.White, xOff+20, yOff + i * 50);
                }
                font.Dispose();
            }
       
        }

        private void SelectJobForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("选择职业", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();
            
            e.Graphics.DrawRectangle(Pens.DodgerBlue, 8, 39, 153, 279);
            e.Graphics.DrawRectangle(Pens.DodgerBlue, 170, 38, 330, 125);
            e.Graphics.DrawRectangle(Pens.DodgerBlue, 170, 168, 330, 86);
            e.Graphics.DrawRectangle(Pens.DodgerBlue, 170, 259, 330, 59);
        }

        private void bitmapButtonSelect_Click(object sender, EventArgs e)
        {
            //var jobConfig = ConfigData.GetJobConfig(selectJobId);
            //if (jobConfig.IsSpecial || jobConfig.InitialLocked && !UserProfile.Profile.InfoBasic.AvailJobList.Contains(selectJobId))
            //    return;

            //if (UserProfile.InfoBasic.Job != JobConfig.Indexer.NewBie)
            //{//转职
            //    UserProfile.InfoBasic.Job = selectJobId;
            //}
            //else
            //{//第一次选职业
            //    UserProfile.InfoBasic.Job = selectJobId;
            //    SendJobReward();
            //}

            Close();
        }
    }
}