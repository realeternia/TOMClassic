using System;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.Decks;
using System.Drawing;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Others;

namespace TaleofMonsters.Controler.Battle.Data.Players
{
    internal class HumanPlayer : Player
    {
        public HumanPlayer(bool isLeft)
            : base(true, isLeft)
        {
            PeopleId = 0;
            Level = UserProfile.InfoBasic.Level;
            Job = UserProfile.InfoBasic.Job;
            
            DeckCard[] cd = new DeckCard[GameConstants.DeckCardCount];
            for (int i = 0; i < GameConstants.DeckCardCount; i++)
            {
                int id = UserProfile.InfoCard.SelectedDeck.GetCardAt(i);
                cd[i] = new DeckCard(UserProfile.InfoCard.GetDeckCardById(id));
            }

            Cards = new ActiveCards(cd);

            int[] energyRate = {0, 0, 0};
            CalculateEquipAndSkill(UserProfile.InfoEquip.GetValidEquipsList(), energyRate);
            EnergyGenerator.SetRate(energyRate, UserProfile.InfoBasic.Job);
            EnergyGenerator.Next(0);


            BattleManager.Instance.RuleData.CheckPlayerData(this);
        }

        public override void AddResource(GameResourceType type, int number)
        {
            if (number > 0)
            {
                UserProfile.InfoBag.AddResource(type, (uint)number);
            }
            else if (number < 0)
            {
                UserProfile.InfoBag.SubResource(type, (uint)(-number));
            }
        }

        public override void InitialCards()
        {
            base.InitialCards();

#if DEBUG
            int[] cardToGive = new[] { 52000045, 52000127 };
            foreach (var cardId in cardToGive)
            {
                CardManager.AddCard(new ActiveCard(cardId, 1, 0));
            }
#endif
        }

    }
}
