using System;
using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using ControlPlus;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Buffs;

namespace TaleofMonsters.Controler.Battle.Data.MemMonster.Component
{
    internal class BuffManager
    {
        private LiveMonster self;
        private Dictionary<int, MemBaseBuff> buffDict;

        public BuffManager(LiveMonster liveMonster)
        {
            self = liveMonster;
        }

        public void Reload()
        {
            buffDict = new Dictionary<int, MemBaseBuff>();
        }

        public bool IsTileMatching
        {
            get { return HasBuff(BuffEffectTypes.Tile); }
        }

        /// <summary>
        /// 免疫目前有2中形式，1，加buff时免疫，按免疫时间缩减buff持续时间。2，对于转换型buff（变羊，变石头），降低几率
        /// </summary>
        public double GetBuffImmuneRate(int group)
        {
            if (group > 0 && group < self.Avatar.MonsterConfig.BuffImmune.Length)
                return self.Avatar.MonsterConfig.BuffImmune[group];
            return 0;
        }

        public void AddBuff(int buffId, int blevel, double dura)
        {
            BuffConfig buffConfig = ConfigData.GetBuffConfig(buffId);
            var immuneRate = GetBuffImmuneRate(buffConfig.Group);
            if (immuneRate >= 1)//免疫了
                return;
            if (immuneRate > 0)
                dura *= (1 - immuneRate);

            MemBaseBuff buffdata;
            if (buffDict.TryGetValue(buffId, out buffdata))
            {
                buffdata.TimeLeft = Math.Max(buffdata.TimeLeft, dura);
            }
            else
            {
                Buff buff = new Buff(buffId);
                buff.UpgradeToLevel(blevel);
                buffdata = new MemBaseBuff(buff, dura);
                buffdata.OnAddBuff(self);
                buffDict.Add(buffId, buffdata);
            }
        }

        public void ClearBuff(bool debuffOnly)
        {
            foreach (var buff in buffDict.Values)
            {
                if (debuffOnly && buff.Type == "ns")
                    buff.TimeLeft = 0;
                if (!debuffOnly && buff.Type.EndsWith("s"))
                    buff.TimeLeft = 0;
            }
        }

        public void ExtendDebuff(double count)
        {
            foreach (var buff in buffDict.Values)
            {
                if (buff.Type == "ns")
                    buff.TimeLeft += count;
            }
        }

        public bool HasBuff(int buffid)
        {
            foreach (var buff in buffDict.Values)
            {
                if (buff.Id == buffid)
                    return true;
            }
            return false;
        }

        public void BuffCount()
        {
            List<int> toDelete = new List<int>();
            foreach (var buff in buffDict.Values)
            {
                buff.OnRoundEffect(self);
                if (buff.TimeLeft <= 0)
                    toDelete.Add(buff.Id);
            }

            if (toDelete.Count > 0)
            {
                foreach (int buffId in toDelete)
                {
                    self.RemoveAttrModify((int)LiveMonster.AttrModifyInfo.AttrModifyTypes.Buff, buffId);
                    buffDict[buffId].OnRemoveBuff(self);
                    buffDict.Remove(buffId);
                }
                toDelete.Clear();
            }
        }

        public bool HasBuff(BuffEffectTypes type)
        {
            foreach (var buff in buffDict.Values)
            {
                if (BuffBook.HasEffect(buff.Id,type))
                    return true;
            }
            return false;
        }

        public void DelBuff(BuffEffectTypes type)
        {
            foreach (var buff in buffDict.Values)
            {
                if (BuffBook.HasEffect(buff.Id, type))
                {
                    buffDict.Remove(buff.Id);
                    return;
                }
            }            
        }

        public void DrawBuffToolTip(TipImage tipData)
        {
            MemBaseBuff[] memBasebuffInfos = new MemBaseBuff[buffDict.Count];
            buffDict.Values.CopyTo(memBasebuffInfos, 0);
            foreach (MemBaseBuff buffdata in memBasebuffInfos)
            {
                Buff buff = buffdata.BuffInfo;
                string tp = "";
                if (buff.BuffConfig.Type[1] == 's')
                    tp = string.Format("{0}(剩余{1:0.0}回合)", buff.BuffConfig.Name, buffdata.TimeLeft);
                else if (buff.BuffConfig.Type[1] == 'a')
                    tp = string.Format("{0}({1})", buff.BuffConfig.Name, buff.Descript);

                tipData.AddImageNewLine(BuffBook.GetBuffImage(buffdata.Id, 0));

                tipData.AddText(tp, BuffBook.GetBuffColor(buffdata.Id));
            }
        }

        public void DrawBuff(Graphics g, int roundKey)
        {
            MemBaseBuff[] copybuffs = new MemBaseBuff[buffDict.Count];
            buffDict.Values.CopyTo(copybuffs, 0);
            if (copybuffs.Length >= 1 && roundKey % 2 == 0)
            {
                int index = 0;
                int wid = 100/copybuffs.Length;
                foreach (var buff in copybuffs)
                {
                    var color = BuffBook.GetBuffColor(buff.Id);
                    var brush = new SolidBrush(color);
                    if (index == copybuffs.Length-1)//最后一个
                        g.FillRectangle(brush, wid * index, 0,100- wid*(index-1), 100);
                    else
                        g.FillRectangle(brush, wid * index, 0, wid, 100);
                    brush.Dispose();

                    g.DrawImage(BuffBook.GetBuffImage(buff.Id,(buff.RoundMark / 3) % 2), new Rectangle(wid * index + (wid-20)/2, 40, 20, 20), new Rectangle(0, 0, 20, 20), GraphicsUnit.Pixel);

                    index++;
                } 
            }
        }

        public void CheckRecoverOnHit()
        {
            foreach (var buff in buffDict.Values)
            {
                if (buff.BuffConfig.EndOnHit)
                    buff.TimeLeft = 0;
            }
        }
    }
}