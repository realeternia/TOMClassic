using System.Collections.Generic;
using TaleofMonsters.Config;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Achieves;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.Decks;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.User.Mem;

namespace TaleofMonsters.DataType.User
{
    public class InfoCard
    {
        [FieldIndex(Index = 1)]
        public Dictionary<int, DeckCard> Cards = new Dictionary<int, DeckCard>();//模板id为key
        [FieldIndex(Index = 2)]
        public List<int> Newcards = new List<int>();
        [FieldIndex(Index = 3)] 
        public MemDeckData[] Decks;
        [FieldIndex(Index = 4)]
        public int DeckId;

        public InfoCard()
        {
            Decks = new MemDeckData[GameConstants.PlayDeckCount];
            for (int i = 0; i < Decks.Length; i++)
            {
                Decks[i] = new MemDeckData(i + 1);
            }
        }

        internal int GetCardCountByType(CardTypes type)
        {
            if (type == CardTypes.Null)
            {
                return Cards.Count;
            }
            int count = 0;
            foreach (DeckCard cd in Cards.Values)
            {
                if (CardAssistant.GetCardType(cd.BaseId) == type)
                {
                    count++;
                }
            }
            return count;
        }

        public MemDeckData SelectedDeck
        {
            get
            {
                return Decks[DeckId];
            }
        }

        /// <summary>
        /// 添加卡牌，外层都会检查cid的合法性，所以里面不用在判断了
        /// </summary>
        /// <param name="cid">卡牌id</param>
        public DeckCard AddCard(int cid)
        {
            DeckCard card = new DeckCard(cid, 1, 0);
            var cardData = CardConfigManager.GetCardConfig(cid);
            if (GetCardCount(cid) >= GameConstants.CardLimit)
            {
                var myCard = GetDeckCardById(cid);
                myCard.AddExp(1);//多余的卡转化为经验值
                return card;//每种卡牌只能拥有1张
            }

            Cards.Add(card.BaseId, card);
            Newcards.Add(card.BaseId);
            if (Newcards.Count > 10)
                Newcards.RemoveAt(0);

            AchieveBook.CheckByCheckType("card");
            MainForm.Instance.AddTip(string.Format("|获得卡片-|{0}|{1}", HSTypes.I2QualityColor(cardData.Quality), cardData.Name), "White");

            return card;
        }

        public void RemoveCardPiece(int id, bool returnResource)
        {
            DeckCard dc;
            if (Cards.TryGetValue(id, out dc))
            {
                if (dc.Exp == 0)
                {
                    return;
                }

                if (returnResource)
                {
                    var cardData = CardConfigManager.GetCardConfig(dc.BaseId);
                    MainForm.Instance.AddTip(string.Format("|分解卡片-|{0}|{1}", HSTypes.I2QualityColor(cardData.Quality), cardData.Name), "White");
                    int qual = CardConfigManager.GetCardConfig(dc.BaseId).Quality + 1;
                    UserProfile.Profile.InfoBag.AddResource(GameResourceType.Gem, GameResourceBook.GetGemCardDecompose(qual));
                }

                dc.Exp--;
            }
        }

        public int GetCardCount(int id)//只有0或1
        {
            if (Cards.ContainsKey(id))
            {
                return 1;
            }
            return 0;
        }

        public string[] GetDeckNames()
        {
            string[] names = new string[Decks.Length];
            for (int i = 0; i < Decks.Length; i++)
            {
                names[i] = Decks[i].Name;
            }
            return names;
        }

        public DeckCard GetDeckCardById(int id)
        {
            if (Cards.ContainsKey(id))
            {
                return Cards[id];
            }
            return new DeckCard(0, 0, 0);
        }

        public int GetCardExp(int cardId)
        {
            var card = GetDeckCardById(cardId);
            if (card.BaseId > 0)
            {
                return card.Exp;
            }
            return 0;
        }

        public void AddCardExp(int cardId, int expadd)
        {
            var card = GetDeckCardById(cardId);
            if (card.BaseId > 0)
            {
                card.AddExp(expadd);
            }
        }

        public bool CanLevelUp(int cardId)
        {
            var card = GetDeckCardById(cardId);
            if (card.BaseId > 0)
            {
                int expNeed = ExpTree.GetNextRequiredCard(card.Level);
                if (card.Exp < expNeed)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public void CardLevelUp(int cardId)
        {       
            var card = GetDeckCardById(cardId);
            if (card.BaseId > 0)
            {
                int expNeed = ExpTree.GetNextRequiredCard(card.Level);
                if (card.Exp < expNeed)
                {
                    return;
                }
                card.Exp = (ushort)(card.Exp - expNeed);
                card.Level++;
            }
        }
    }
}
