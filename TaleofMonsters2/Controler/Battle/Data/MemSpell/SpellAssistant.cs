using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemEffect;
using TaleofMonsters.Controler.Battle.Data.MemFlow;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType.Cards.Spells;
using TaleofMonsters.DataType.Effects;

namespace TaleofMonsters.Controler.Battle.Data.MemSpell
{
    internal static class SpellAssistant
    {
        public static void CheckSpellEffect(Spell spell, bool isLeft, LiveMonster target, Point mouse)
        {
            MemBaseSpell spl = new MemBaseSpell(spell);
            spl.CheckSpellEffect(isLeft, target, mouse);
            if (spell.Addon != 0 && (spl.SpellConfig.Cure > 0 || spl.SpellConfig.Damage > 0))
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord(string.Format("{0}倍施法！", 1+spell.Addon), mouse, 0, "Gold", 26, 0, 0, 2, 15));

            SpellConfig spellConfig = spell.SpellConfig;
            if (spl.HintWord!="")
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord(spl.HintWord, mouse, 0, "Cyan", 26, 0, 0, 0, 15));
            if (!string.IsNullOrEmpty(spellConfig.UnitEffect))
            {
                if (BattleTargetManager.PlayEffectOnMonster(spellConfig.Target))
                    BattleManager.Instance.EffectQueue.Add(new ActiveEffect(EffectBook.GetEffect(spellConfig.UnitEffect), target, false));
                if (BattleTargetManager.PlayEffectOnMouse(spellConfig.Target))
                    BattleManager.Instance.EffectQueue.Add(new ActiveEffect(EffectBook.GetEffect(spellConfig.UnitEffect), mouse, false));
            }
        }
    }
}
