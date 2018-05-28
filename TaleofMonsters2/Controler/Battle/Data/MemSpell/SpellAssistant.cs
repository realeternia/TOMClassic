using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemFlow;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Cards.Spells;
using TaleofMonsters.Datas.Effects;
using TaleofMonsters.Datas.Effects.Facts;

namespace TaleofMonsters.Controler.Battle.Data.MemSpell
{
    internal static class SpellAssistant
    {
        public static bool CanCast(Spell spell, LiveMonster target)
        {
            if (spell.SpellConfig.CanCast == null)
                return true;
            return spell.SpellConfig.CanCast(spell, target);
        }

        public static void Cast(Spell spell, bool isLeft, LiveMonster target, Point mouse)
        {
            PlaySpell(spell, isLeft, target, mouse);
            if (spell.Addon != 0 && (spell.SpellConfig.Cure > 0 || spell.SpellConfig.Damage > 0))
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord(string.Format("{0}倍施法！", 1+spell.Addon), mouse, 0, "Gold", 26, 0, 0, 2, 15));

            SpellConfig spellConfig = spell.SpellConfig;
            if (!string.IsNullOrEmpty(spellConfig.UnitEffect))
            {
                if (BattleTargetManager.PlayEffectOnMonster(spellConfig.Target))
                    BattleManager.Instance.EffectQueue.Add(new MonsterBindEffect(EffectBook.GetEffect(spellConfig.UnitEffect), target, false));
                if (BattleTargetManager.PlayEffectOnMouse(spellConfig.Target))
                    BattleManager.Instance.EffectQueue.Add(new MonsterBindEffect(EffectBook.GetEffect(spellConfig.UnitEffect), mouse, false));
            }
        }

        private static void PlaySpell(Spell spell, bool isLeft, LiveMonster target, Point mouse)
        {
            if (spell.SpellConfig.Effect != null)
            {
                Player p1 = isLeft ? BattleManager.Instance.PlayerManager.LeftPlayer : BattleManager.Instance.PlayerManager.RightPlayer;
                Player p2 = !isLeft ? BattleManager.Instance.PlayerManager.LeftPlayer : BattleManager.Instance.PlayerManager.RightPlayer;

                spell.SpellConfig.Effect(spell, BattleManager.Instance.MemMap, p1, p2, target, mouse);

                if (!string.IsNullOrEmpty(spell.SpellConfig.AreaEffect))
                {
                    //播放特效
                    RegionTypes rt = BattleTargetManager.GetRegionType(spell.SpellConfig.Target[2]);
                    var cardSize = BattleManager.Instance.MemMap.CardSize;
                    foreach (var pickCell in BattleManager.Instance.MemMap.Cells)
                    {
                        var pointData = pickCell.ToPoint();
                        if (BattleLocationManager.IsPointInRegionType(rt, mouse.X, mouse.Y, pointData, spell.SpellConfig.Range, isLeft))
                        {
                            var effectData = new MonsterBindEffect(EffectBook.GetEffect(spell.SpellConfig.AreaEffect), pointData + new Size(cardSize / 2, cardSize / 2), false);
                            BattleManager.Instance.EffectQueue.Add(effectData);
                        }
                    }
                }
            }
        }
    }
}
