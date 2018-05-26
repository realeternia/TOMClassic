using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Datas.User;

namespace TaleofMonsters.Controler.Battle.Data.Players
{
    internal class MirrorPlayer : Player
    {
        public MirrorPlayer(int id, CardOffBundle cpcards, bool isLeft)
            : base(false, isLeft)
        {
            PeopleId = id;

            PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(id);
            Level = peopleConfig.Level;
            Job = UserProfile.InfoBasic.Job;

            AIModule = new AIStrategy(this);
            EnergyGenerator.SetRateNpc(new [] { 0, 0, 0 },peopleConfig);

            OffCards = cpcards.GetCopy();
            EnergyGenerator.Next(0);
        }
    }
}
