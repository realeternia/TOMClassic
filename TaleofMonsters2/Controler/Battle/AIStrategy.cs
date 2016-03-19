using System.Drawing;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Cards.Monsters;
using TaleofMonsters.DataType.Cards.Spells;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards.Weapons;

namespace TaleofMonsters.Controler.Battle
{
    internal static class AIStrategy
    {
        internal static void OnInit(Player player)
        {
            var cds = player.CardsDesk.GetAllCard();
            for (int i = 0; i < cds.Length; i++)
            {
                var card = cds[i];
                if (card.Card.Star>3)//把3费以上卡都换掉
                {
                    player.CardManager.GetNextCardAt(i + 1);
                }
            }
        }

        internal static void AIProc(Player player)
        {            
            if (player.GetCardNumber() <= 0)
                return;

            if (MathTool.GetRandom(4) != 0)
                return;

            int row = BattleManager.Instance.MemMap.RowCount;
            int size = BattleManager.Instance.MemMap.CardSize;
            bool isLeft = player.IsLeft;
            var rival = (player == BattleManager.Instance.PlayerManager.LeftPlayer) ? BattleManager.Instance.PlayerManager.RightPlayer : BattleManager.Instance.PlayerManager.LeftPlayer;
            
            player.CardsDesk.SetSelectId(MathTool.GetRandom(player.GetCardNumber()) + 1);
            if (player.SelectCardId != 0)
            {
                ActiveCard card = player.CardsDesk.GetSelectCard();
                if (player.CheckUseCard(card, player, rival) != HSErrorTypes.OK)
                {
                    return;
                }

                int tar = -1;
                if (card.CardType == CardTypes.Weapon)
                {
                    for (int i = 0; i <BattleManager.Instance.MonsterQueue.Count; i++)
                    {
                        LiveMonster monster =BattleManager.Instance.MonsterQueue[i];
                        if (!monster.IsGhost && monster.IsLeft == isLeft && monster.TWeapon.CardId == 0 && monster.Life > monster.MaxHp / 2)
                        {
                            if (!monster.CanAddWeapon())//建筑无法使用武器
                                continue;

                            if (tar == -1 || monster.Avatar.MonsterConfig.Star >BattleManager.Instance.MonsterQueue[tar].Avatar.MonsterConfig.Star)
                                tar = i;
                        }
                    }
                    if (tar == -1)
                        return;
                }
                else if (card.CardType == CardTypes.Spell)
                {
                    SpellConfig spellConfig = ConfigData.GetSpellConfig(card.CardId);
                    if (BattleTargetManager.IsSpellUnitTarget(spellConfig.Target))
                    {
                        for (int i = 0; i <BattleManager.Instance.MonsterQueue.Count; i++)
                        {
                            LiveMonster monster =BattleManager.Instance.MonsterQueue[i];
                            if (!monster.IsGhost && ((monster.IsLeft != isLeft && spellConfig.Target[1] == 'F') || (monster.IsLeft == isLeft && spellConfig.Target[1] != 'F')))
                            {
                                if (tar == -1 || monster.Avatar.MonsterConfig.Star >BattleManager.Instance.MonsterQueue[tar].Avatar.MonsterConfig.Star)
                                    tar = i;
                            }
                        }
                        if (tar == -1)
                            return;
                    }
                }

                //可能有触发/状态等
                player.OnUseCard(player.CardsDesk.GetSelectCard());

                if (rival.CheckTrapOnUseCard(card, rival, player))
                {
                    return;
                }

                if (card.CardType == CardTypes.Monster)
                {
                    Point monPos = GetMonsterPoint(false);
                    var mon = new Monster(card.CardId);
                    mon.UpgradeToLevel(card.Level);
                    player.OnSummon(mon);
                    
                    LiveMonster newMon = new LiveMonster(card.Id, card.Level, mon, monPos, false);
                   BattleManager.Instance.MonsterQueue.Add(newMon);
                    
                    rival.CheckTrapOnSummon(newMon, rival, player);
                }
                else if (card.CardType == CardTypes.Weapon)
                {
                    Weapon wpn = new Weapon(card.CardId);
                    wpn.UpgradeToLevel(card.Level);
                    player.OnUseWeapon(wpn);
                   
                    var lm =BattleManager.Instance.MonsterQueue[tar];
                    var tWeapon = new TrueWeapon(lm, card.Level, wpn);
                    lm.AddWeapon(tWeapon);
                }
                else if (card.CardType == CardTypes.Spell)
                {
                    SpellConfig spellConfig = ConfigData.GetSpellConfig(card.CardId);
                    Spell spell = new Spell(card.CardId);
                    spell.Addon = player.SpellEffectAddon;
                    spell.UpgradeToLevel(card.Level);
                    player.OnDoSpell(spell);

                    if (BattleTargetManager.IsSpellNullTarget(spellConfig.Target))
                    {
                        Point targetPoint = new Point(isLeft ? MathTool.GetRandom(200, 300) : MathTool.GetRandom(600, 700), MathTool.GetRandom(size * 3 / 10, row * size - size * 3 / 10));
                        SpellAssistant.CheckSpellEffect(spell, isLeft, null, targetPoint);

                    }
                    else if (BattleTargetManager.IsSpellUnitTarget(spellConfig.Target))
                    {
                        Point targetPoint =BattleManager.Instance.MonsterQueue[tar].CenterPosition;
                        SpellAssistant.CheckSpellEffect(spell, isLeft,BattleManager.Instance.MonsterQueue[tar], targetPoint);
                    }
                }

                player.CardManager.DeleteCardAt(player.SelectId);
            }
        }

        private static Point GetMonsterPoint(bool isLeft)
        {
            int size = BattleManager.Instance.MemMap.CardSize;
            var sideCell = BattleManager.Instance.MemMap.ColumnCount / 2;
            while (true)
            {
                int x = isLeft ? MathTool.GetRandom(0, sideCell) : MathTool.GetRandom(sideCell + 1, BattleManager.Instance.MemMap.ColumnCount - 1);
                int y = MathTool.GetRandom(0, BattleManager.Instance.MemMap.RowCount);
                if (BattleManager.Instance.MemMap.Cells[x, y].Owner == 0)
                {
                    return new Point(x * size, y * size);
                }
            }
        }
    }
}