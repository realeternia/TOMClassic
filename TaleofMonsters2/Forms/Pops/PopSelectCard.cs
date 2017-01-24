using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.Decks;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.Others;

namespace TaleofMonsters.Forms.Pops
{
    internal partial class PopSelectCard : Form
    {
        internal class CompareDeckCardByLevel : IComparer<DeckCard>
        {
            #region IComparer<CardDescript> 成员

            public int Compare(DeckCard cx, DeckCard cy)
            {
                if (cx.Level != cy.Level)
                {
                    return cx.Level.CompareTo(cy.Level);
                }

                return (cx.BaseId.CompareTo(cy.BaseId));
            }

            #endregion
        }

        internal class CompareDeckCardByExp : IComparer<DeckCard>
        {
            #region IComparer<CardDescript> 成员

            public int Compare(DeckCard cx, DeckCard cy)
            {
                int expx = (ExpTree.GetNextRequiredCard(cx.Level) - cx.Exp) * 100 / ExpTree.GetNextRequiredCard(cx.Level);
                int expy = (ExpTree.GetNextRequiredCard(cy.Level) - cy.Exp) * 100 / ExpTree.GetNextRequiredCard(cy.Level);

                if (expx != expy)
                {
                    return expx.CompareTo(expy);
                }

                return (cx.BaseId.CompareTo(cy.BaseId));
            }

            #endregion
        }

        private int[] cids;
        private int[] activeCids;
        private int sel = -1;

        private int fil_type;
        private int fil_order;

        public PopSelectCard()
        {
            InitializeComponent();
            BackgroundImage = PicLoader.Read("System", "DeckChoose.PNG");
            FormBorderStyle = FormBorderStyle.None;

            Init();
        }

        private void Init()
        {
            cids = new int[50];
            for (int i = 0; i < 50; i++)
            {
                cids[i] = UserProfile.InfoCard.SelectedDeck.GetCardAt(i);
            }
            sel = 0;
            radioButton1.Checked = true;
            comboBoxType.SelectedIndex = 0;

            RegetCards();
        }

        private void RegetCards()
        {
            List<DeckCard> cards = new List<DeckCard>();
            foreach (int cid in cids)
            {
                if ((int)ConfigIdManager.GetCardType(cid)== fil_type)
                {
                    DeckCard card = new DeckCard(UserProfile.InfoCard.GetDeckCardById(cid));
                    cards.Add(card);
                }
            }

            if (cards.Count <= 0)
            {
                sel = -1;
            }
            else
            {
                switch (fil_order)
                {
                    case 0: cards.Sort(new CompareDeckCardByLevel()); break;
                    case 1: cards.Sort(new CompareDeckCardByLevel()); cards.Reverse(); break;
                    case 2: cards.Sort(new CompareDeckCardByExp()); cards.Reverse(); break;
                    case 3: cards.Sort(new CompareDeckCardByExp()); break;
                }

                activeCids = new int[cards.Count];
                for (int i = 0; i < activeCids.Length; i++)
                {
                    activeCids[i] = cards[i].BaseId;
                }
                sel = 0;
            }

            Invalidate();
        }

        private void MessageBoxEx_Paint(object sender, PaintEventArgs e)
        {
            if (sel >= 0)
            {
                DeckCard card = new DeckCard(UserProfile.InfoCard.GetDeckCardById(activeCids[sel]));
                e.Graphics.DrawImage(CardAssistant.GetCardImage(card.BaseId, 100, 100), 38, 62, 100, 100);

                Font font = new Font("Arial", 7*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                const string stars = "★★★★★★★★★★";
                int star = CardConfigManager.GetCardConfig(card.BaseId).Star;
                e.Graphics.DrawString(stars.Substring(10 - star), font, Brushes.Yellow, 38, 62);

                e.Graphics.DrawString(string.Format("{0:00}", card.Level), font, Brushes.Gold, 38, 150);
                e.Graphics.FillRectangle(Brushes.Wheat, 53, 153, card.Exp * 80 / ExpTree.GetNextRequiredCard(card.Level), 5);
                e.Graphics.DrawRectangle(Pens.WhiteSmoke, 53, 153, 80, 5);
                font.Dispose();
            }
        }

        public new static int Show()
        {
            PopSelectCard mb = new PopSelectCard();
            mb.ShowDialog();
            return mb.sel == -1 ? -1 : mb.activeCids[mb.sel];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            sel = -1;
            Close();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            fil_type = int.Parse((sender as Control).Tag.ToString());
            RegetCards();
        }

        private void comboBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            fil_order = comboBoxType.SelectedIndex;
            RegetCards();
        }

        private void buttonMinus_Click(object sender, EventArgs e)
        {
            if (sel>=0)
            {
                sel--;
                sel = (sel + activeCids.Length) % activeCids.Length;
                Invalidate();
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (sel >= 0)
            {
                sel++;
                sel = (sel + activeCids.Length)%activeCids.Length;
                Invalidate();
            }
        }

        private void buttonRand_Click(object sender, EventArgs e)
        {
            if (sel >= 0)
            {
                sel = NarlonLib.Math.MathTool.GetRandom(activeCids.Length);
                Invalidate();
            }
        }
    }
}