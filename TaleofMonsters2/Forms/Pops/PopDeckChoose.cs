using System;
using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.Maps;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.Forms.Pops
{
    internal partial class PopDeckChoose : Form
    {
        private NLPageSelector pageSelector;
        private int selectPage = 0;

        private int tile;
        private bool comfirm;
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

        private void MessageBoxEx_Paint(object sender, PaintEventArgs e)
        {
            if (img != null)
            {
                e.Graphics.DrawImage(img, 38, 32, 100, 100);

                var deck = UserProfile.InfoCard.Decks[selectPage];
                for (int i = 0; i < deck.CardIds.Length; i++)
                {
                    int x = i%6;
                    int y = i/6;
                    e.Graphics.DrawImage(CardAssistant.GetCardImage(deck.CardIds[i],100,100),x*20+150,y*20+35,20,20);
                }
            }

            Font font = new Font("宋体", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            e.Graphics.DrawString(string.Format("地形:{0}", tile == -1 ? "默认" : ConfigDatas.ConfigData.GetTileConfig(tile).Cname), font, Brushes.White, 63, 117);
            font.Dispose();
        }

        public static bool Show(string map, int tile, string[] datas)
        {
            PopDeckChoose mb = new PopDeckChoose();
            mb.tile = tile;
            mb.img = BattleMapBook.GetMapImage(map, tile);
            mb.pageSelector.SetTarget(UserProfile.InfoCard.DeckId);
            mb.ShowDialog();
            return mb.comfirm;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            comfirm = true;
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