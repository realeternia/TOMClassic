using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemCard;

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

            Cards = cpcards.GetCopy();
            InitBase();
        }
    }
}
