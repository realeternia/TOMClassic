using TaleofMonsters.Config;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.Decks;

namespace TaleofMonsters.DataType.User.Mem
{
    public class MemDeckData
    {
        [FieldIndex(Index = 1)]
        public int[] CardIds;
        [FieldIndex(Index = 2)]
        public string Name;
        [FieldIndex(Index = 3)]
        public int Mcount;
        [FieldIndex(Index = 4)]
        public int Wcount;

        public MemDeckData()
        {
            
        }

        public MemDeckData(int index)
        {
            CardIds = new int[GameConstants.DeckCardCount];
            for (int i = 0; i < GameConstants.DeckCardCount; i++)
            {
                CardIds[i] = -1;
            }
            Name = string.Format("卡组{0}", index);
        }

        public int Count
        {
            get { return Wcount+Mcount; }
        }

        public int GetCardAt(int index)
        {
            return CardIds[index];
        }

        public void SetCardAt(int index, int card)
        {
            if (index >= 0 && index < GameConstants.DeckCardCount)
            {
                CardIds[index] = card;
                Recalculate();
            }
        }

        public int AddCard(DeckCard card)
        {
            int firstBlank = -1;
            int count = 0;
            int newCardJob = CardConfigManager.GetCardConfig(card.BaseId).JobId;
            for (int i = 0; i < GameConstants.DeckCardCount; i++)
            {
                var dcard = CardIds[i];

                if (dcard == -1 && firstBlank == -1)
                {
                    firstBlank = i;
                }
                else if (dcard != -1)
                {
                    DeckCard tCard = UserProfile.InfoCard.GetDeckCardById(dcard);
                    if (tCard.BaseId == card.BaseId)
                        return HSErrorTypes.DeckCardAlreadyIn;

                    int cardJob = CardConfigManager.GetCardConfig(tCard.BaseId).JobId;
                    if (newCardJob > 0 && cardJob > 0 && newCardJob != cardJob)
                        return HSErrorTypes.CardJobTwice;
                }
            }

            if (count >= GameConstants.CardLimit)
            {
                return HSErrorTypes.DeckCardTypeLimit;
            }
            if (firstBlank==-1)
            {
                return HSErrorTypes.DeckIsFull;
            }
            SetCardAt(firstBlank, card.BaseId);

            return HSErrorTypes.OK;
        }

        public void RemoveCardById(int id)
        {
            for (int i = 0; i < GameConstants.DeckCardCount; i++)
            {
                if (CardIds[i] == id)
                {
                    CardIds[i] = -1;
                    break;
                }
            }
            Recalculate();
        }

        public bool HasCard(int id)
        {
            for (int i = 0; i < GameConstants.DeckCardCount; i++)
            {
                if (CardIds[i] == id)
                {
                    return true;
                }
            }
            return false;
        }

        public void Recalculate()
        {
            Mcount = 0;
            Wcount = 0;
            for (int i = 0; i < GameConstants.DeckCardCount; i++)
            {
                if (CardIds[i] != -1)
                {
                    var cardType = CardAssistant.GetCardType(CardIds[i]);
                    if (cardType == CardTypes.Monster)
                    {
                        Mcount = Mcount + 1;
                    }
                    else
                    {
                        Wcount = Wcount + 1;
                    }
                }
            }
        }
    }
}
