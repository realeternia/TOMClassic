using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType.Cards.Spells;

namespace TaleofMonsters.Controler.Battle.Data.MemSpell
{
    internal class MemBaseSpell
    {
        private int spellId;
        private Spell spellInfo;
        public string HintWord { get; private set; }
        public int Level { get; set; }

        public MemBaseSpell(Spell spl)
        {
            spellId = spl.Id;
            Level = spl.Level;
            spellInfo = spl;
            HintWord = "";
        }
        
        public SpellConfig SpellConfig
        {
            get { return spellInfo.SpellConfig; }
        }

        public void CheckSpellEffect(bool isLeft, LiveMonster target, Point mouse)
        {
            if (spellInfo.SpellConfig.Effect != null)
            {
                Player p1 = isLeft ? BattleManager.Instance.PlayerManager.LeftPlayer : BattleManager.Instance.PlayerManager.RightPlayer;
                Player p2 = !isLeft ? BattleManager.Instance.PlayerManager.LeftPlayer : BattleManager.Instance.PlayerManager.RightPlayer;
                spellInfo.SpellConfig.Effect(spellInfo, BattleManager.Instance.MemMap, p1, p2, target, mouse, Level);
            }
        }
    }
}
