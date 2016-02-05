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
        private int leftCount;
        private int rightCount;

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

        public int LeftCount
        {
            get { return leftCount; }
        }

        public int RightCount
        {
            get { return rightCount; }
        }

        public void Add(LiveMonster mon)
        {
            mon.OwnerPlayer.State.CheckMonsterEvent(true, mon.Avatar);
            monsters.Add(mon);
            BattleLocationManager.UpdateCellOwner(mon.Position.X, mon.Position.Y, mon.Id);
            mon.SkillManager.CheckInitialEffect();
            if (mon.IsLeft)
            {
                leftCount++;
            }
            else
            {
                rightCount++;
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

        public void AddInitialAction()
        {
            List<MonsterIdSpeed> lives = new List<MonsterIdSpeed>();
            foreach (LiveMonster mon in monsters)
            {
                MonsterIdSpeed spd = new MonsterIdSpeed();
                spd.id = mon.Id;
                spd.speed = mon.Avatar.Spd;
                spd.mark = mon.Level;
                lives.Add(spd);
            }
            lives.Sort(new CompareBySpeed());
            for (int i = 0; i < lives.Count; i++)
            {
                LiveMonster tar = GetMonsterByUniqueId(lives[i].id);
                tar.Action += (11 - i)*75;
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

        public void NextAction()
        {
            leftCount = 0;
            rightCount = 0;
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
                    if (!mon.IsLeft) rightCount = RightCount + 1;
                    else leftCount = LeftCount + 1;
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
                TileMatchResult match = TileBook.MatchTile(tile,roundMonster.Avatar.MonsterConfig.Type);
                roundMonster.Next(match);
               
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
