using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType.Cards.Monsters;
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

            EnergyGenerator.SetRateNpc(peopleConfig);

            DeckCard[] cds = DeckBook.GetDeckByName(deck, Level);
            Cards = new ActiveCards(cds);
            PlayerAttr attr = new PlayerAttr();
            if (peopleConfig.Job != 0)
            {
             //   EAddonBook.UpdateMasterData(State.Masterskills.Keys(), State.Masterskills.Values());
                HeroData = new Monster(MonsterConfig.Indexer.HeroCardId);
                HeroData.Name = peopleConfig.Name;
                HeroData.UpgradeToLevel(Cards.GetAvgLevel());

                JobConfig jobConfig = ConfigDatas.ConfigData.GetJobConfig(peopleConfig.Job);
                HeroImage = PicLoader.Read("Hero", string.Format("{0}.JPG", jobConfig.ProtoImage));
            }
            else
            {
                HeroData = new Monster(peopleConfig.KingCard);
                HeroData.UpgradeToLevel(Cards.GetAvgLevel());
                HeroImage = PicLoader.Read("Monsters", string.Format("{0}.JPG", HeroData.MonsterConfig.Icon));
            }
            attr.ModifyMonsterData(HeroData);
            InitBase();
        }
        
        public override void InitialCards()
        {
            base.InitialCards();
#if DEBUG
        //    CardManager.AddCard(new ActiveCard(20001, 1, 0));
#endif
        }
    }
}
