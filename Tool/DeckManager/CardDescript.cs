using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DeckManager
{
    [Serializable]
    public struct CardDescript
    {
        public int Id;
        public string Tip;

        public CardDescript(int id, string tip)
        {
            Id = id;
            Tip = tip;
        }
        
        public static CardDescript[] MakeLoad(string name, int level)
        {
            var deck = new List<CardDescript>();
            StreamReader sr = new StreamReader(name);

            for (int i = 0; i < 30; i++)
            {
                var line = sr.ReadLine();
                if (line == null)
                    break;
                if (line.Contains("//"))
                {
                    int index = line.IndexOf("//"); //È¥³ý×¢ÊÍ
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

            return deck.ToArray();
        }
    }
}
