using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Datas.Decks;
using TaleofMonsters.Datas.User;

namespace TaleofMonsters.Controler.Battle.Data.Players
{
    internal class RandomPlayer : Player
    {
        public RandomPlayer(int id, bool isLeft)
            : base(isLeft)
        {
            PeopleId = id;

            PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(id);
            Level = peopleConfig.Level;
            Job = UserProfile.InfoBasic.Job;

            EnergyGenerator.SetRateNpc(new[] { 0, 0, 0 }, peopleConfig);

            DeckCard[] cd = new DeckCard[GameConstants.DeckCardCount];
            for (int i = 0; i < GameConstants.DeckCardCount; i++)
                cd[i] = new DeckCard(CardConfigManager.GetRandomCard(0, -1), 1, 0);
            OffCards = new CardOffBundle(cd);
            EnergyGenerator.Next(0);
        }
    }
}
