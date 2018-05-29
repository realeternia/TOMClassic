using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.Players;

namespace TaleofMonsters.Controler.Battle.Data.MemSpell
{
    internal class Trap : ITrap
    {
        public Player Owner { get; set; }
        public int Id { get; set; }//配置表id
        public int SpellId { get; set; } //触发的技能id
        public double Rate { get; set; }//触发几率，大于100表示100%触发
        public int Level { get; set; }//触发技能的等级
        public int Damage { get; set; }//陷阱伤害
        public double Help { get; set; }//辅助数据
    }
}
