using ConfigDatas;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Decks;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.Controler.Battle.Data.Players
{
    internal class RandomPlayer : Player
    {
        public RandomPlayer(int id, bool isLeft, bool isplayerControl)
            : base(isplayerControl, isLeft)
        {
            PeopleId = id;

            PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(id);
            Level = peopleConfig.Level;
            Job = UserProfile.InfoBasic.Job;

            EnergyGenerator.SetRateNpc(new[] { 0, 0, 0 }, peopleConfig);

            DeckCard[] cd = new DeckCard[GameConstants.DeckCardCount];
            for (int i = 0; i < GameConstants.DeckCardCount; i++)
            {
                cd[i] = new DeckCard(CardConfigManager.GetRandomCard(0, -1), 1, 0);
            }
            Cards = new ActiveCards(cd);
            InitBase();
        }
    }
}
