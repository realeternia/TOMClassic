using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.Players;

namespace TaleofMonsters.Controler.Battle.Data.MemWeapon
{
    internal class Relic : IRelic
    {
        public Player Owner { get; set; }
        public int Id { get; set; }//weapon表id
        public int Level { get; set; }//触发技能的等级
    }
}
