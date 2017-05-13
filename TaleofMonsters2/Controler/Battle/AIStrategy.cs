using System.Drawing;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.DataType;

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
                    player.CardManager.RedrawCardAt(i + 1);
                }
            }

#if DEBUG
            //int[] cardToGive = new[] { 53000019 };
            //foreach (var cardId in cardToGive)
            //{
            //    player.CardManager.AddCard(new ActiveCard(cardId, 1, 0));
            //}
#endif
        }

        internal static void AIProc(Player player)
        {            
            if (player.CardNumber <= 0)
                return;

            if (MathTool.GetRandom(4) != 0)
                return;

            int row = BattleManager.Instance.MemMap.RowCount;
            int size = BattleManager.Instance.MemMap.CardSize;
            bool isLeft = player.IsLeft;
            var rival = player == BattleManager.Instance.PlayerManager.LeftPlayer ? 
                BattleManager.Instance.PlayerManager.RightPlayer : BattleManager.Instance.PlayerManager.LeftPlayer;
            
            player.CardsDesk.SetSelectId(MathTool.GetRandom(player.CardNumber) + 1);
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
                        if (!monster.IsGhost && monster.IsLeft == isLeft && monster.Weapon == null && monster.Life > monster.RealMaxHp / 2)
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
                        var targetStar = -1;
                        if (tar >= 0)
                            targetStar = BattleManager.Instance.MonsterQueue[tar].Avatar.MonsterConfig.Star;
                        for (int i = 0; i <BattleManager.Instance.MonsterQueue.Count; i++)
                        {
                            LiveMonster monster =BattleManager.Instance.MonsterQueue[i];
                            if(monster.IsGhost)
                                continue;
                            if ((monster.IsLeft != isLeft && spellConfig.Target[1] != 'F') || (monster.IsLeft == isLeft && spellConfig.Target[1] != 'E'))
                            {
                                if (tar == -1 || monster.Avatar.MonsterConfig.Star > targetStar)
                                {
                                    tar = i;
                                    targetStar = monster.Avatar.MonsterConfig.Star;
                                }
                            }
                        }
                        if (tar == -1)
                            return;
                    }
                }

                if (card.CardType == CardTypes.Monster)
                {
                    Point monPos = GetMonsterPoint(card.CardId, false);
                    player.UseMonster(card, monPos);
                }
                else if (card.CardType == CardTypes.Weapon)
                {
                    var lm =BattleManager.Instance.MonsterQueue[tar];
                    player.UseWeapon(lm, card);
                }
                else if (card.CardType == CardTypes.Spell)
                {
                    SpellConfig spellConfig = ConfigData.GetSpellConfig(card.CardId);
                    Point targetPos = Point.Empty;
                    LiveMonster targetMonster = null;
                    if (BattleTargetManager.IsSpellNullTarget(spellConfig.Target))
                    {
                        targetPos = new Point(isLeft ? MathTool.GetRandom(200, 300) : MathTool.GetRandom(600, 700), MathTool.GetRandom(size * 3 / 10, row * size - size * 3 / 10));
                    }
                    else if (BattleTargetManager.IsSpellUnitTarget(spellConfig.Target))
                    {
                        targetMonster = BattleManager.Instance.MonsterQueue[tar];
                        targetPos = targetMonster.CenterPosition;
                    }

                    player.DoSpell(targetMonster, card, targetPos);
                }
            }
        }

        private static Point GetMonsterPoint(int mid, bool isLeft)
        {
            int size = BattleManager.Instance.MemMap.CardSize;
            var sideCell = BattleManager.Instance.MemMap.ColumnCount / 2;
            while (true)
            {
                int x = isLeft ? MathTool.GetRandom(0, sideCell) : MathTool.GetRandom(sideCell + 1, BattleManager.Instance.MemMap.ColumnCount - 1);
                int y = MathTool.GetRandom(0, BattleManager.Instance.MemMap.RowCount);
                x *= size;
                y *= size;
                if (BattleLocationManager.IsPlaceCanSummon(mid,x, y,false))
                {
                    return new Point(x, y);
                }
            }
        }

        public static void Discover(Player p, IMonster m, int[] cardId, int lv)
        {
            var targetCardId = cardId[MathTool.GetRandom(cardId.Length)]; //随机拿一张
            p.AddDiscoverCard(m, targetCardId, lv);
        }
    }
}