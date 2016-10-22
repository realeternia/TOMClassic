using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.DataType.Decks;

namespace TaleofMonsters.Controler.Battle.Data.Players
{
    internal class AIPlayer : Player
    {
        public AIPlayer(int id, string deck, bool isLeft, int rlevel, bool isPlayerControl)
            : base(isPlayerControl, isLeft)
        {
            PeopleId = id;

            Level = rlevel;
            PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(id);
            Level = peopleConfig.Level;
            Job = peopleConfig.Job;

            EnergyGenerator.SetRateNpc(peopleConfig);

            DeckCard[] cds = DeckBook.GetDeckByName(deck, Level);
            Cards = new ActiveCards(cds);
          //  PlayerAttr attr = new PlayerAttr();
          //  attr.ModifyMonsterData(HeroData);
            InitBase();
        }
        
        public override void InitialCards()
        {
            base.InitialCards();
        }
    }
}
