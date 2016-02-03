using System;
using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Core;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Achieves;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.DataType.Others;

namespace TaleofMonsters.DataType.User
{
    public class InfoBag
    {
        [FieldIndex(Index = 1)]
        public GameResource Resource;
        [FieldIndex(Index = 2)]
        public int Diamond;
        [FieldIndex(Index = 3)]
        public IntPair[] Items ;
        [FieldIndex(Index = 4)]
        public int BagCount ;

        [Obsolete("此数据不用存回，但目前罗技有问题")]
        public AutoDictionary<int, int> tpBonusItem = new AutoDictionary<int, int>();

        public InfoBag()
        {
            Resource = new GameResource();
            Items = new IntPair[50];
        }

        public bool CheckResource(int[] resourceInfo)
        {
            if (Resource.Gold >= resourceInfo[0] &&
                Resource.Lumber >= resourceInfo[1] &&
                Resource.Stone >= resourceInfo[2] &&
                Resource.Mercury >= resourceInfo[3] &&
                Resource.Carbuncle >= resourceInfo[4] &&
                Resource.Sulfur >= resourceInfo[5] &&
                Resource.Gem >= resourceInfo[6])
                return true;
            return false;
        }

        internal bool HasResource(GameResourceType type, int value)
        {
            return Resource.Has(type, value);
        }

        public void AddDiamond(int value)
        {
            Diamond += value;
            MainForm.Instance.AddTip(string.Format("|获得|Cyan|{0}||钻石", value), "White");
        }

        public void AddResource(int[] res)
        {
            Resource.Gold += res[0];
            Resource.Lumber += res[1];
            Resource.Stone += res[2];
            Resource.Mercury += res[3];
            Resource.Carbuncle += res[4];
            Resource.Sulfur += res[5];
            Resource.Gem += res[6];

            for (int i = 0; i < 7; i++)
            {
                if (res[i] > 0)
                    MainForm.Instance.AddTip(string.Format("|获得|{0}|{1}||x{2}", HSTypes.I2ResourceColor(i), HSTypes.I2Resource(i), res[i]), "White");
            }

            AchieveBook.CheckByCheckType("resource");
        }

        internal void AddResource(GameResourceType type, int value)
        {
            Resource.Add(type, value);
            if (type > 0)
            {
                MainForm.Instance.AddTip(string.Format("|获得|{0}|{1}||x{2}", HSTypes.I2ResourceColor((int)type), HSTypes.I2Resource((int)type), value), "White");
                AchieveBook.CheckByCheckType("resource"); 
            }
        }

        public bool PayDiamond(int value)
        {
            if (Diamond < value)
            {
                MainForm.Instance.AddTip("钻石不足", "Red");
                return false;
            }
            Diamond -= value;
            MainForm.Instance.AddTip(string.Format("|失去了|Cyan|{0}||钻石,账户剩余|Cyan|{1}||钻石", value, Diamond), "White");
            return true;
        }

        public void SubResource(int[] res)
        {
            Resource.Gold -= res[0];
            Resource.Lumber -= res[1];
            Resource.Stone -= res[2];
            Resource.Mercury -= res[3];
            Resource.Carbuncle -= res[4];
            Resource.Sulfur -= res[5];
            Resource.Gem -= res[6];
        }


        public void AddItem(int id, int num)
        {
            HItemConfig itemConfig = ConfigData.GetHItemConfig(id);
            MainForm.Instance.AddTip(string.Format("|获得物品-|{0}|{1}||x{2}", HSTypes.I2RareColor(itemConfig.Rare), itemConfig.Name, num), "White");

            int max = itemConfig.MaxPile;
            if (max <= 0)
            {
                return;
            }

            if (tpBonusItem[id] > 0)
            {
                tpBonusItem[id] = Math.Max(tpBonusItem[id] - num, 0);
            }

            int count = num;
            for (int i = 0; i < BagCount; i++)
            {
                if (Items[i].Type == id && Items[i].Value < max)
                {
                    if (Items[i].Value + count <= max)
                    {
                        Items[i].Value += count;
                        return;
                    }
                    count -= max - Items[i].Value;
                    Items[i].Value = max;
                }
            }
            for (int i = 0; i < BagCount; i++)
            {
                if (Items[i].Type == 0)
                {
                    if (count <= max)
                    {
                        Items[i].Type = id;
                        Items[i].Value = count;
                        return;
                    }
                    Items[i].Type = id;
                    count -= max;
                    Items[i].Value = max;
                }
            }
        }

        public void UseItemByPos(int pos, int type)
        {
            if (Items[pos].Value <= 0)
                return;

            if (HItemAssistant.UseItemsById(Items[pos].Type, type))
            {
                Items[pos].Value--;
                if (Items[pos].Value <= 0)
                    Items[pos].Type = 0;
            }
        }

        public void ClearItemAllByPos(int pos)
        {
            Items[pos].Value = 0;
            Items[pos].Type = 0;
        }

        public void SellItemAllByPos(int pos)
        {
            if (Items[pos].Type > 0 && Items[pos].Value > 0)
            {
                int money = ConfigData.GetHItemConfig(Items[pos].Type).Value * Items[pos].Value;
                AddResource(GameResourceType.Gold, money);
            }
            ClearItemAllByPos(pos);
        }


        public int GetItemCount(int id)
        {
            int count = 0;
            for (int i = 0; i < BagCount; i++)
            {
                if (Items[i].Type == id)
                {
                    count += Items[i].Value;
                }
            }
            return count;
        }


        public void DeleteItem(int id, int num)
        {
            int count = num;
            for (int i = 0; i < BagCount; i++)
            {
                if (Items[i].Type == id)
                {
                    if (Items[i].Value > count)
                    {
                        Items[i].Value -= count;
                        return;
                    }
                    count -= Items[i].Value;
                    Items[i].Type = 0;
                    Items[i].Value = 0;
                }
            }
        }

        public void SortItem()
        {
            Array.Sort(Items, new CompareByMid());
            for (int i = 0; i < 999; i++)
            {
                if (Items[i].Type == 0)
                    break;
                int max = ConfigData.GetHItemConfig(Items[i].Type).MaxPile;
                if (Items[i].Value < max && Items[i].Type == Items[i + 1].Type)
                {
                    if (Items[i].Value + Items[i + 1].Value <= max)
                    {
                        Items[i].Value = Items[i].Value + Items[i + 1].Value;
                        Items[i + 1].Type = 0;
                        Items[i + 1].Value = 0;
                    }
                    else
                    {
                        Items[i + 1].Value = Items[i].Value + Items[i + 1].Value - max;
                        Items[i].Value = max;
                    }
                }
            }
        }

        public List<IntPair> GetItemCountBySubtype(int type)
        {
            AutoDictionary<int, int> counter = new AutoDictionary<int, int>();
            for (int i = 0; i < BagCount; i++)
            {
                HItemConfig itemConfig = ConfigData.GetHItemConfig(Items[i].Type);
                if (itemConfig != null && itemConfig.SubType == type)
                {
                    counter[itemConfig.Id] += Items[i].Value;
                }
            }
            List<IntPair> datas = new List<IntPair>();
            foreach (int itemId in counter.Keys())
            {
                IntPair pairData = new IntPair();
                pairData.Type = itemId;
                pairData.Value = counter[itemId];
                datas.Add(pairData);
            }
            return datas;
        }

        [Obsolete("暂时没用的")]
        public bool IsItemTaskNeed(int itemid)
        {
            return tpBonusItem[itemid] > 0;
        }
    }
}
