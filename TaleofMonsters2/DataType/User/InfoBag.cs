using System;
using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Core;
using NarlonLib.Tools;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Achieves;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.MainItem;

namespace TaleofMonsters.DataType.User
{
    public class InfoBag
    {
        [FieldIndex(Index = 1)] public GameResource Resource;
        [FieldIndex(Index = 2)] public int Diamond;
        [FieldIndex(Index = 3)] public IntPair[] Items;
        [FieldIndex(Index = 4)] public int BagCount;
        [FieldIndex(Index = 5)] public int[] CdGroupStartTime; //开始时间
        [FieldIndex(Index = 6)] public int[] CdGroupTime; //到期时间

        public InfoBag()
        {
            Resource = new GameResource();
            Items = new IntPair[100];
            for (int i = 0; i < 100; i++)
            {
                Items[i] = new IntPair();
            }
            CdGroupStartTime = new int[GameConstants.ItemCdGroupCount];
            CdGroupTime = new int[GameConstants.ItemCdGroupCount];
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

        internal bool HasResource(GameResourceType type, uint value)
        {
            return Resource.Has(type, (int)value);
        }

        public void AddDiamond(int value)
        {
            Diamond += value;
            MainTipManager.AddTip(string.Format("|获得|Cyan|{0}||钻石", value), "White");
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
                    MainTipManager.AddTip(string.Format("|获得|{0}|{1}||x{2}", HSTypes.I2ResourceColor(i), HSTypes.I2Resource(i), res[i]), "White");
            }

            AchieveBook.CheckByCheckType("resource");
        }

        internal void AddResource(GameResourceType type, uint value)
        {
            Resource.Add(type, (int)value);
            if (type > 0)
            {
                MainTipManager.AddTip(string.Format("|获得|{0}|{1}||x{2}", HSTypes.I2ResourceColor((int)type), HSTypes.I2Resource((int)type), value), "White");
                AchieveBook.CheckByCheckType("resource"); 
            }
        }

        public bool PayDiamond(int value)
        {
            if (Diamond < value)
            {
                MainTipManager.AddTip(HSErrorTypes.GetDescript(HSErrorTypes.BagNotEnoughDimond), "Red");
                return false;
            }
            Diamond -= value;
            MainTipManager.AddTip(string.Format("|失去了|Cyan|{0}||钻石,账户剩余|Cyan|{1}||钻石", value, Diamond), "White");
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

        internal void SubResource(GameResourceType type, uint value)
        {
            Resource.Add(type, (int)-value);
        }


        public void AddItem(int id, int num)
        {
            HItemConfig itemConfig = ConfigData.GetHItemConfig(id);
            if (itemConfig.Id == 0)
                return;
            MainTipManager.AddTip(string.Format("|获得物品-|{0}|{1}||x{2}", HSTypes.I2RareColor(itemConfig.Rare), itemConfig.Name, num), "White");

            int max = itemConfig.MaxPile;
            if (max <= 0)
            {
                return;
            }

            int count = num;
            for (int i = 0; i < BagCount; i++)
            {
                var pickItem = Items[i];
                if (pickItem.Type == id && pickItem.Value < max)
                {
                    if (pickItem.Value + count <= max)
                    {
                        pickItem.Value += count;
                        return;
                    }
                    count -= max - pickItem.Value;
                    pickItem.Value = max;
                }
            }
            for (int i = 0; i < BagCount; i++)
            {
                var pickItem = Items[i];
                if (pickItem.Type == 0)
                {
                    if (count <= max)
                    {
                        pickItem.Type = id;
                        pickItem.Value = count;
                        return;
                    }
                    pickItem.Type = id;
                    count -= max;
                    pickItem.Value = max;
                }
            }
        }

        public void UseItemByPos(int pos, int type)
        {
            var pickItem = Items[pos];
            if (pickItem.Value <= 0)
                return;

            if (Consumer.UseItemsById(pickItem.Type, type))
            {
                var consumerConfig = ConfigData.GetItemConsumerConfig(pickItem.Type);
                CdGroupStartTime[consumerConfig.CdGroup] = TimeTool.GetNowUnixTime();
                CdGroupTime[consumerConfig.CdGroup] = TimeTool.GetNowUnixTime() + consumerConfig.CdTime;
                pickItem.Value--;
                if (pickItem.Value <= 0)
                    pickItem.Type = 0;
                MainForm.Instance.RefreshView();
            }
        }

        public void ClearItemAllByPos(int pos)
        {
            Items[pos].Value = 0;
            Items[pos].Type = 0;
        }

        public void SellItemAllByPos(int pos)
        {
            var pickItem = Items[pos];
            if (pickItem.Type > 0 && pickItem.Value > 0)
            {
                var config = ConfigData.GetHItemConfig(pickItem.Type);
                uint sellPrice = GameResourceBook.InGoldSellItem(config.Rare, config.ValueFactor);
                uint money = (uint)(sellPrice * pickItem.Value);
                AddResource(GameResourceType.Gold, money);
            }
            ClearItemAllByPos(pos);
        }


        public int GetItemCount(int id)
        {
            int count = 0;
            for (int i = 0; i < BagCount; i++)
            {
                var pickItem = Items[i];
                if (pickItem.Type == id)
                {
                    count += pickItem.Value;
                }
            }
            return count;
        }


        public void DeleteItem(int id, int num)
        {
            int count = num;
            for (int i = 0; i < BagCount; i++)
            {
                var pickItem = Items[i];
                if (pickItem.Type == id)
                {
                    if (pickItem.Value > count)
                    {
                        pickItem.Value -= count;
                        return;
                    }
                    count -= pickItem.Value;
                    pickItem.Type = 0;
                    pickItem.Value = 0;
                }
            }
        }

        public void SortItem()
        {
            Array.Sort(Items, new CompareByMid());
            for (int i = 0; i < 999; i++)
            {
                var pickItem = Items[i];
                if (pickItem.Type == 0)
                    break;
                int max = ConfigData.GetHItemConfig(pickItem.Type).MaxPile;
                if (pickItem.Value < max && pickItem.Type == Items[i + 1].Type)
                {
                    if (pickItem.Value + Items[i + 1].Value <= max)
                    {
                        pickItem.Value = pickItem.Value + Items[i + 1].Value;
                        Items[i + 1].Type = 0;
                        Items[i + 1].Value = 0;
                    }
                    else
                    {
                        Items[i + 1].Value = pickItem.Value + Items[i + 1].Value - max;
                        pickItem.Value = max;
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

        public void ResizeBag(int newSize)
        {
            IntPair[] item = new IntPair[BagCount];
            Array.Copy(Items, item, BagCount);
            Items = new IntPair[newSize];
            Array.Copy(item, Items, BagCount);
            for (int i = BagCount; i < newSize; i++)
                Items[i] = new IntPair();
            BagCount = newSize;
        }

        public float GetCdTimeRate(int itemId)
        {
            var canUse = ConfigData.GetHItemConfig(itemId).IsUsable;
            if (canUse)
            {
                var consumerConfig = ConfigData.GetItemConsumerConfig(itemId);
                var group = consumerConfig.CdGroup;
                if (CdGroupStartTime[group-1] > 0)
                {
                    var nowTime = TimeTool.GetNowUnixTime();
                    if (nowTime >= CdGroupTime[group - 1])
                    {
                        return 0;
                    }
                    else
                    {
                        return (nowTime - CdGroupStartTime[group - 1]) /
                          (CdGroupTime[group - 1] - CdGroupStartTime[group - 1]);
                    }
                }
            }
            return 0;
        }
    }
}
