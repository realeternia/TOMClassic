using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using ConfigDatas;

namespace DeckManager
{
    public class CardDeck
    {
        private static int myJobId; //ws下

        public struct CardDescript
        {
            public string Type;
            public int Id;

            public CardDescript(int id, string tp)
            {
                Id = id;
                Type = tp;
            }

            public void Draw(Graphics g, Font ft, int x, int y, int wid, int het)
            {
                if (Id > 0)
                {
                    var img = ImageCache.GetImage(Id);
                    if (img == null) //可能是老卡
                        return;
                    g.DrawImage(img, x, y, wid, het);
                    var cardConfig = CardConfigManager.GetCardConfig(Id);
                    var brush = new SolidBrush(Color.FromName(HSTypes.I2QualityColor((int)cardConfig.Quality)));
                    g.DrawString(cardConfig.Name, ft, Brushes.LightBlue, x + 4, y + het - 16);
                    g.DrawString(cardConfig.Name, ft, brush, x + 3, y + het - 17);
                    brush.Dispose();

                    Font ft2 = new Font("宋体", 6);
                    g.DrawString(("★★★★★★★★★★").Substring(10 - cardConfig.Star), ft2, Brushes.Yellow, x + 1, y + 3);
                    ft2.Dispose();

                    var jobId = cardConfig.JobId;
                    if (jobId > 0)
                    {
                        var jobConfig = ConfigData.GetJobConfig(jobId);
                        g.DrawString(jobConfig.Name, ft, Brushes.Red, x + wid - 24, y + 4);
                        if (myJobId > 0 && myJobId != jobId)
                        {
                            Brush cover = new SolidBrush(Color.FromArgb(180, Color.Black));
                            g.FillRectangle(cover, x, y, wid, het);
                            cover.Dispose();
                        }
                    }
                }
                else
                {

                    var dts = Type.Split('|');
                    var des = "";
                    if (dts[0] == "race")
                    {
                        des = "随机" + HSTypes.I2CardTypeSub(int.Parse(dts[1]));
                        g.DrawRectangle(Pens.DarkCyan, x, y, wid, het);
                    }
                    else if (dts[0] == "attr")
                    {
                        des = "随机" + HSTypes.I2Attr(int.Parse(dts[1]));
                        g.DrawRectangle(Pens.DarkRed, x, y, wid, het);
                    }
                    else if (dts[0] == "job")
                    {
                        des = "随机" + ConfigData.GetJobConfig(int.Parse(dts[1])).Name;
                        g.DrawRectangle(Pens.Yellow, x, y, wid, het);
                    }
                    des += dts[2];
                    g.DrawString(des, ft, Brushes.LightBlue, x + 4, y + het - 16);
                }
            }
        }

        private string fileName;
        private CardDescript[] cards;

        public void Load(string name)
        {
            fileName = name;
            var deck = new List<CardDescript>();
            StreamReader sr = new StreamReader(name);

            myJobId = 0;
            for (int i = 0; i < 30; i++)
            {
                var line = sr.ReadLine();
                if (line == null)
                    break;
                if (line.Contains("//"))
                {
                    int index = line.IndexOf("//"); //去除注释
                    line = line.Substring(0, index);
                }

                string[] datas = line.Split('=');
                int cardId = 0;
                string tip = "";

                string tpStr = datas[0].Trim();
                string valStr = datas[1].Trim();
                if (tpStr == "Id")
                {
                    cardId = int.Parse(valStr);
                }
                else if (tpStr == "Rand")
                {
                    tip = valStr;
                }
                else
                {
                    throw new ApplicationException("card type error " + tpStr + "@" + name);
                }

                var card = new CardDescript(cardId, tip);
                deck.Add(card);
            }
            sr.Close();

            cards = deck.ToArray();
            UpdateMyCardJob();
        }

        public void Save()
        {
            Array.Sort(cards, new CompareByStarCard());

            StreamWriter sw = new StreamWriter(fileName);
            for (int i = 0; i < 30; i++)
            {
                if (cards[i].Id > 0)
                {
                    sw.WriteLine("Id={0}//{1}", cards[i].Id, CardConfigManager.GetCardConfig(cards[i].Id).Name);
                }
                else
                {
                    sw.WriteLine("Rand={0}", cards[i].Type);
                }
            }
            sw.Close();
        }

        public CardDescript GetCardId(int id)
        {
            return cards[id];
        }

        public void Replace(int index, CardDescript cd)
        {
            cards[index].Id= cd.Id;
            cards[index].Type = cd.Type;

            UpdateMyCardJob();
        }

        private void UpdateMyCardJob()
        {
            myJobId = 0;
            foreach (var cardDescript in cards)
            {
                if (cardDescript.Id > 0)
                {
                    var cardConfig = CardConfigManager.GetCardConfig(cardDescript.Id);
                    if (cardConfig.JobId > 0)
                    {
                        myJobId = cardConfig.JobId;
                        return;
                    }
                }
                else
                {
                    if (cardDescript.Type.StartsWith("job"))
                    {
                        myJobId = int.Parse(cardDescript.Type.Split('|')[1]);
                        return;
                    }
                }
            }
        }
    }

    class CompareByStarCard : IComparer<CardDeck.CardDescript>
    {
        #region IComparer<int> 成员

        public int Compare(CardDeck.CardDescript cx, CardDeck.CardDescript cy)
        {
            if (cx.Id == 0 && cy.Id == 0)
                return cx.Type.CompareTo(cy.Type);
            if (cx.Id == 0)
                return 1;
            if (cy.Id == 0)
                return -1;

            var x = CardConfigManager.GetCardConfig(cx.Id);
            var y = CardConfigManager.GetCardConfig(cy.Id);
            if (x.Type != y.Type)
                return x.Type.CompareTo(y.Type);
            if (x.Star != y.Star)
                return x.Star.CompareTo(y.Star);
            return x.Id.CompareTo(y.Id);
        }

        #endregion
    }
}
