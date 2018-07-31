using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Log;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Datas.Others;

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
            mon.OwnerPlayer.Modifier.CheckMonsterEvent(true, mon);
            monsters.Add(mon);
            BattleManager.Instance.MemMap.UpdateCellOwner(mon.Position, mon.Id);
            mon.OnInit();
            if (mon.IsLeft)
                LeftCount++;
            else
                RightCount++;
        }

        public void AddDelay(LiveMonster mon)
        {
            toAdd.Add(mon);
        }

        private void Remove(LiveMonster mon)
        {
            mon.OwnerPlayer.Modifier.CheckMonsterEvent(false, mon);
            if (BattleManager.Instance.MemMap.GetMouseCell(mon.Position.X, mon.Position.Y).Owner == -mon.Id)
                BattleManager.Instance.MemMap.UpdateCellOwner(mon.Position, 0);
            monsters.Remove(mon);                                
        }

        private void Remove(int monid)
        {
            foreach (var pickMon in monsters)
            {
                if (pickMon.Id == monid)
                {
                    Remove(pickMon);
                    return;
                }
            }
        }

        public void RemoveDirect(int monid)
        {
            Remove(monid);
        }

        public LiveMonster GetMonsterByUniqueId(int id)
        {
            for (int i = 0; i < monsters.Count; i++)
            {
                if (monsters[i].Id == System.Math.Abs(id))
                    return monsters[i];
            }
            return null;
        }

        public LiveMonster GetKingTower(bool isLeft)
        {
            for (int i = 0; i < monsters.Count; i++)
            {
                var towerUnit = monsters[i] as TowerMonster;
                if (towerUnit != null && towerUnit.IsLeft == isLeft && towerUnit.IsKing)
                    return towerUnit;
            }
            return null;
        }

        public void NextAction(float pastRound)
        {
            LeftCount = 0;
            RightCount = 0;
            List<int> removeMids = new List<int>();            
            foreach (var pickMon in monsters)
            {
                var rival = pickMon.Rival as Player;
                var directDam = rival.SpecialAttr.DirectDamage;
                if (directDam > 0)
                {
                    HitDamage damage = new HitDamage(directDam, directDam, 0, DamageTypes.Magic);
                    pickMon.HpBar.OnDamage(damage);
                }
                if (!pickMon.IsAlive)
                {
                    if (!pickMon.IsGhost)
                    {
                        pickMon.OnDie();
                    }
                    else
                    {
                        if (pickMon.OwnerPlayer.SpikeManager.HasSpike("grave") || (pickMon.Rival as Player).SpikeManager.HasSpike("grave"))
                            pickMon.GhostTime += 0.005f;
                        else
                            pickMon.GhostTime += 0.01f;
                        if (pickMon.GhostTime >= 1)
                            removeMids.Add(pickMon.Id);
                    }
                }
                else
                {
                    if (!pickMon.IsLeft) RightCount = RightCount + 1;
                    else LeftCount = LeftCount + 1;
                }
            }
            BattleManager.Instance.PlayerManager.LeftPlayer.SpecialAttr.DirectDamage = 0;//伤害清除
            BattleManager.Instance.PlayerManager.RightPlayer.SpecialAttr.DirectDamage = 0;

            foreach (var mid in removeMids)
                Remove(mid);
            
            foreach (var roundMonster in monsters)
            {
                if (roundMonster.IsGhost) 
                    continue;

                int tile = BattleManager.Instance.MemMap.GetMouseCell(roundMonster.Position.X, roundMonster.Position.Y).Tile;
                var match = TileBook.IsTileMatch(tile,roundMonster.Avatar.MonsterConfig.Attr);
                roundMonster.Next(pastRound, match);
            }

            foreach (var delayMid in toAdd) //添加延时怪
            {
                Add(delayMid);
                NLog.Debug("NextAction AddMon pid={0} cid={1}", delayMid.OwnerPlayer.PeopleId, delayMid.CardId);
            }
            toAdd.Clear();
        }

        public void Clear()
        {
            monsters.Clear();
        }
    }
}
