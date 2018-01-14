using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.Decks;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType;

namespace TaleofMonsters.Controler.Battle.Data.Players
{
    internal class HumanPlayer : Player
    {
        public HumanPlayer(bool isLeft, DeckCard[] cardInitial)
            : base(true, isLeft)
        {
            PeopleId = 0;
            Level = UserProfile.InfoBasic.Level;
            Job = UserProfile.InfoBasic.Job;

            DeckCards = new ActiveCards(cardInitial);

            int[] energyRate = {0, 0, 0};
            CalculateEquipAndSkill(UserProfile.InfoEquip.GetValidEquipsList(), energyRate);
            EnergyGenerator.SetRate(energyRate, UserProfile.InfoBasic.Job);
            EnergyGenerator.Next(0);
            
            BattleManager.Instance.RuleData.CheckPlayerData(this);
        }

        public override void AddResource(GameResourceType type, int number)
        {
            if (number > 0)
                UserProfile.InfoBag.AddResource(type, (uint)number);
            else if (number < 0)
                UserProfile.InfoBag.SubResource(type, (uint)(-number));
        }

        public override void InitialCards()
        {
            base.InitialCards();

#if DEBUG
            int[] cardToGive = new[] { 51000317 };
            foreach (var cardId in cardToGive)
            {
                CardManager.AddCard(new ActiveCard(cardId, 1, 0));
            }
#endif
        }

    }
}
