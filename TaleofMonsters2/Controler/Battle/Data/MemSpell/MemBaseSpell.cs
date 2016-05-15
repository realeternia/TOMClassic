using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemEffect;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards.Spells;
using TaleofMonsters.DataType.Effects;

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

                if (!string.IsNullOrEmpty(spellInfo.SpellConfig.AreaEffect))
                {
                    //播放特效
                    RegionTypes rt = BattleTargetManager.GetRegionType(spellInfo.SpellConfig.Target[2]);
                    var cardSize = BattleManager.Instance.MemMap.CardSize;
                    foreach (var memMapPoint in BattleManager.Instance.MemMap.Cells)
                    {
                        var pointData = memMapPoint.ToPoint();
                        if (BattleLocationManager.IsPointInRegionType(rt, mouse.X, mouse.Y, pointData, spellInfo.SpellConfig.Range, isLeft))
                        {
                            var effectData = new ActiveEffect(EffectBook.GetEffect(spellInfo.SpellConfig.AreaEffect), pointData + new Size(cardSize / 2, cardSize / 2), false);
                            BattleManager.Instance.EffectQueue.Add(effectData);
                        }
                    } 
                }
            }
        }
    }
}
