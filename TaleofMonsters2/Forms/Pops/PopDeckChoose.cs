using System;
using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Cards;
using TaleofMonsters.Datas.Maps;
using TaleofMonsters.Datas.User;

namespace TaleofMonsters.Forms.Pops
{
    internal partial class PopDeckChoose : Form
    {
        private NLPageSelector pageSelector;
        private int selectPage = 0;

        private int tile;
        private bool confirm;
        private Image img; //地形图片

        public PopDeckChoose()
        {
            InitializeComponent();
            BackgroundImage = PicLoader.Read("System", "DeckChoose.PNG");
            FormBorderStyle = FormBorderStyle.None;

            pageSelector = new NLPageSelector(this, 35, 145, 180);
            pageSelector.TotalPage = 9;
            pageSelector.PageChange +=new NLPageSelector.ChangePageEventHandler(pageSelector_PageChange);
        }

        private void PopDeckChoose_Paint(object sender, PaintEventArgs e)
        {
            if (img != null)
            {
                e.Graphics.DrawImage(img, 38, 32, 100, 100);

                var deck = UserProfile.InfoCard.Decks[selectPage];
                for (int i = 0; i < deck.CardIds.Count; i++)
                {
                    int x = i%6;
                    int y = i/6;

                    var cardId = deck.CardIds[i];
                    e.Graphics.DrawImage(CardAssistant.GetCardImage(cardId,100,100),x*20+150,y*20+35,20,20);
                    var cardJob = CardConfigManager.GetCardConfig(cardId).JobId;
                    if (cardJob > 0 && cardJob != UserProfile.InfoBasic.Job)
                    {
                        var brush = new SolidBrush(Color.FromArgb(150, Color.Red));
                        e.Graphics.FillRectangle(brush, x * 20 + 150, y * 20 + 35, 20, 20);
                        brush.Dispose();
                        e.Graphics.DrawRectangle(Pens.Red, x * 20 + 150, y * 20 + 35, 20, 20);
                    }
                }
            }

            Font font = new Font("宋体", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            e.Graphics.DrawString(string.Format("地形:{0}", tile == 0 ? "默认" : ConfigDatas.ConfigData.GetTileConfig(tile).Cname), font, Brushes.White, 63, 117);
            font.Dispose();
        }

        public static bool Show(string map)
        {
            PopDeckChoose mb = new PopDeckChoose();
            mb.tile = 0;
            mb.img = BattleMapBook.GetMapImage(map, 0);
            mb.pageSelector.SetTarget(UserProfile.InfoCard.DeckId);
            mb.ShowDialog();

            return mb.confirm;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            var deck = UserProfile.InfoCard.Decks[selectPage];
            if (deck.Count < GameConstants.DeckCardCount)
            {
                MessageBoxEx2.Show("卡组内卡片数不足");
                return;
            }
            
            foreach (var cardId in deck.CardIds)
            {
                var card = CardAssistant.GetCard(cardId);
                if (card.JobId > 0 && card.JobId != UserProfile.InfoBasic.Job)
                {
                    MessageBoxEx2.Show("部分卡牌职业不匹配");
                    return;
                }
            }

            confirm = true;
            UserProfile.InfoCard.DeckId = selectPage;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        void pageSelector_PageChange(int pg)
        {
            selectPage = pg;
            Invalidate();
        }
    }
}