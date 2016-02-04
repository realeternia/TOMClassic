using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType.Cards.Monsters;

namespace TaleofMonsters.Controler.Battle.Data.Players
{
    internal class MirrorPlayer : Player
    {
        public MirrorPlayer(int id, ActiveCards cpcards, bool isLeft)
            : base(false, isLeft)
        {
            PeopleId = id;

            PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(id);
            Level = peopleConfig.Level;

            EnergyGenerator.SetRateNpc(peopleConfig);

            PlayerAttr attr = new PlayerAttr();
            Cards = cpcards.GetCopy();
            HeroData = new Monster(peopleConfig.KingCard);//todo 用attr构造
            HeroData.UpgradeToLevel(Cards.GetAvgLevel());
            HeroImage = PicLoader.Read("Monsters", string.Format("{0}.JPG", HeroData.MonsterConfig.Icon));
            InitBase();
        }
    }
}
