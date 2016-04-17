using System.Collections.Generic;
using TaleofMonsters.Controler.Battle.Data.MemFlow;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Others;

namespace TaleofMonsters.Controler.Battle.DataTent
{
    internal class MonsterQueue
    {
        private List<LiveMonster> monsters = new List<LiveMonster>();
        private List<LiveMonster> toAdd = new List<LiveMonster>();
        public int LeftCount { get; private set; }
        public int RightCount { get; private set; }

        public List<LiveMonster> Enumerator
        {
            get { return monsters; }
        }

        public LiveMonster this[int index]
        {
            get { return monsters[index]; }
            set { monsters[index] = value; }
        }

        public int Count
        {
            get { return monsters.Count; }
        }


        public void Add(LiveMonster mon)
        {
            mon.OwnerPlayer.State.CheckMonsterEvent(true, mon.Avatar);
            monsters.Add(mon);
            BattleLocationManager.UpdateCellOwner(mon.Position.X, mon.Position.Y, mon.Id);
            mon.SkillManager.CheckInitialEffect();
            if (mon.IsLeft)
            {
                LeftCount++;
            }
            else
            {
                RightCount++;
            }
        }

        public void AddDelay(LiveMonster mon)
        {
            toAdd.Add(mon);
        }

        private void Remove(LiveMonster mon)
        {
            mon.OwnerPlayer.State.CheckMonsterEvent(false, mon.Avatar);
            if (BattleManager.Instance.MemMap.GetMouseCell(mon.Position.X, mon.Position.Y).Owner == -mon.Id)
                BattleLocationManager.UpdateCellOwner(mon.Position.X, mon.Position.Y, 0);
            monsters.Remove(mon);                                
        }

        private void Remove(int monid)
        {
            foreach (LiveMonster monster in monsters)
            {
                if (monster.Id == monid)
                {
                    Remove(monster);
                    return;
                }
            }
        }

        public LiveMonster GetMonsterByUniqueId(int id)
        {
            for (int i = 0; i < monsters.Count; i++)
            {
                if (monsters[i].Id == System.Math.Abs(id))
                {
                    return monsters[i];
                }
            }
            return null;
        }

        public LiveMonster GetKingTower(bool isLeft)
        {
            for (int i = 0; i < monsters.Count; i++)
            {
                var lm = monsters[i];
                if (lm.IsLeft == isLeft && lm.Type == (int)CardTypeSub.Hero)
                {
                    return lm;
                }
            }
            return null;
        }

        public void NextAction(float pastRound)
        {
            LeftCount = 0;
            RightCount = 0;
            List<int> removeMids = new List<int>();            
            foreach (LiveMonster mon in monsters)
            {
                var rival = mon.Rival as Player;
                if (rival.DirectDamage > 0)
                    mon.Life -= rival.DirectDamage;
                int loss = mon.LossLife;
                if (loss > 0)
                {
                    BattleManager.Instance.FlowWordQueue.Add(new FlowWord(loss.ToString(), mon.CenterPosition, 5, "red", 0, -10), false);//掉血显示
                }
                if (!mon.IsAlive)
                {
                    if (!mon.IsGhost)
                    {
                        mon.OnDie();
                    }
                    else
                    {
                        mon.GhostTime++;
                        if (mon.GhostTime>=100) //100 is ghost time 1round
                        {
                            removeMids.Add(mon.Id);
                        }
                    }
                }
                else
                {
                    if (!mon.IsLeft) RightCount = RightCount + 1;
                    else LeftCount = LeftCount + 1;
                }
            }
            BattleManager.Instance.PlayerManager.LeftPlayer.DirectDamage = 0;//伤害清除
            BattleManager.Instance.PlayerManager.RightPlayer.DirectDamage = 0;

            foreach (int mid in removeMids)
            {
                Remove(mid);
            }


            foreach (LiveMonster roundMonster in monsters)
            {
                if (roundMonster.IsGhost) 
                    continue;

                int tile = BattleManager.Instance.MemMap.GetMouseCell(roundMonster.Position.X, roundMonster.Position.Y).Tile;
                TileMatchResult match = TileBook.MatchTile(tile,roundMonster.Avatar.MonsterConfig.Attr);
                roundMonster.Next(pastRound, match);
            }

            foreach (var lm in toAdd)
            {
                Add(lm);
            }
            toAdd.Clear();
        }

        public void Clear()
        {
            monsters.Clear();
        }
    }
}
