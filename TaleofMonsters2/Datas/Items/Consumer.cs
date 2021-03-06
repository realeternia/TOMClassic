﻿using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Log;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Datas.Drops;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.CMain.Blesses;
using TaleofMonsters.Forms.CMain.Scenes;

namespace TaleofMonsters.Datas.Items
{
    internal static class Consumer
    {
        public static bool UseItemsById(int id, HItemUseTypes useMethod)
        {
            HItemConfig itemConfig = ConfigData.GetHItemConfig(id);
            ItemConsumerConfig consumerConfig = ConfigData.GetItemConsumerConfig(id);
            if (useMethod == HItemUseTypes.Common)
            {
                if (itemConfig.SubType == (int)HItemTypes.Gift)
                    return UseGift(consumerConfig);
                if (itemConfig.SubType == (int)HItemTypes.Item)
                    return UseItem(consumerConfig);
                if (itemConfig.SubType == (int)HItemTypes.RandomCard)
                    return UseRandomCard(consumerConfig);
                if (itemConfig.SubType == (int)HItemTypes.DropItem)
                    return UseDropItem(consumerConfig);
            }
            else if (useMethod == HItemUseTypes.Fight)
            {
                if (itemConfig.SubType == (int)HItemTypes.Fight)
                    return UseFightItem(consumerConfig);
            }
            else if (useMethod == HItemUseTypes.Seed)
            {
                if (itemConfig.SubType == (int)HItemTypes.Seed)
                    return UseSeedItem(consumerConfig);
            }

            return false;
        }

        private static bool UseItem(ItemConsumerConfig itemConfig)
        {
            if (itemConfig.ResourceId > 0)
                UserProfile.InfoBag.AddResource((GameResourceType)(itemConfig.ResourceId - 1), (uint)itemConfig.ResourceCount);
            if (itemConfig.GainExp > 0)
                UserProfile.InfoBasic.AddExp(itemConfig.GainExp);
            if (itemConfig.GainFood > 0)
                UserProfile.InfoBasic.AddFood((uint)itemConfig.GainFood);
            if (itemConfig.GainHealth > 0)
                UserProfile.InfoBasic.AddHealth((uint)itemConfig.GainHealth);
            if (itemConfig.GainMental > 0)
                UserProfile.InfoBasic.AddMental((uint)itemConfig.GainMental);
            if (itemConfig.BlessId > 0)
                BlessManager.AddBless(itemConfig.BlessId);
            if (itemConfig.BuildPoint > 0)
                UserProfile.InfoCastle.AddEp(itemConfig.BuildPoint);
            if (!string.IsNullOrEmpty(itemConfig.Instruction))
                CheckInstruction(itemConfig.Instruction);
            if (itemConfig.DungeonAttr != null && itemConfig.DungeonAttr.Length > 0)
            {
                if (UserProfile.InfoDungeon.DungeonId < 0)
                    return false;
                UserProfile.InfoDungeon.ChangeAttr(itemConfig.DungeonAttr[0], itemConfig.DungeonAttr[1]
                    , itemConfig.DungeonAttr[2], itemConfig.DungeonAttr[3], itemConfig.DungeonAttr[4]);
            }
            return true;
        }

        private static bool UseFightItem(ItemConsumerConfig itemConfig)
        {
            var player = BattleManager.Instance.PlayerManager.LeftPlayer;
            if (itemConfig.GainLp > 0)
                player.AddLp(itemConfig.GainLp);
            if (itemConfig.GainPp > 0)
                player.AddPp(itemConfig.GainPp);
            if (itemConfig.GainMp > 0)
                player.AddMp(itemConfig.GainMp);
            if (itemConfig.FightRandomCardType > 0)
            {
                int cardId = CardConfigManager.GetRandomTypeCard(itemConfig.FightRandomCardType);
                player.HandCards.AddCard(new ActiveCard(cardId, 1));
            }
            if (!string.IsNullOrEmpty(itemConfig.HolyWord))
            {
                switch (itemConfig.HolyWord)
                {//瞬发
                    case "dambig": player.SpecialAttr.DirectDamage += 9999; break;
                    case "damsmall": player.SpecialAttr.DirectDamage += 100; break;
                    case "refreshep": player.EnergyGenerator.Refresh(); break;
                    default: player.HolyBook.AddWord(itemConfig.HolyWord); break; //持续
                }
            }
            if (itemConfig.AddTowerHp > 0)
                player.AddTowerHp(itemConfig.AddTowerHp);
            return true;
        }

        private static bool UseSeedItem(ItemConsumerConfig itemConfig)
        {
            return UserProfile.Profile.InfoCastle.UseSeed(itemConfig.FarmItemId, itemConfig.FarmEnergy);
        }
        
        private static bool UseRandomCard(ItemConsumerConfig itemConfig)
        {
            var form = PanelManager.FindPanel(typeof(CardBagForm));
            if (form != null)//如果打开着开包面板，退出
                return false;

            form = new CardBagForm();
            ((CardBagForm)form).SetData(itemConfig.Id);
            PanelManager.DealPanel(form);

            return true;
        }
        public static bool UseDropItem(ItemConsumerConfig itemConfig)
        {
            if (!string.IsNullOrEmpty(itemConfig.DropItem))
            {
                var itemList = DropBook.GetDropItemList(itemConfig.DropItem);
                if (UserProfile.InfoBag.GetBlankCount() < itemList.Count)
                    return false;
                var countList = new List<int>();
                foreach (var itemId in itemList)
                {
                    if (ConfigIdManager.IsEquip(itemId))
                    {
                        NLog.Warn("UseDropItem id={0} contains equip", itemConfig.Id);
                        continue;
                    }

                    UserProfile.InfoBag.AddItem(itemId, 1);
                    countList.Add(1);
                }
                var form = new ItemPackageForm();
                form.SetItem(itemList.ToArray(), countList.ToArray());
                PanelManager.DealPanel(form);
            }

            return true;
        }

        private static bool UseGift(ItemConsumerConfig itemConfig)
        {
            if (UserProfile.InfoBag.GetBlankCount() < itemConfig.Items.Length)
                return false;

            List<int> itemList = new List<int>();
            List<int> countList = new List<int>();
            for (int i = 0; i < itemConfig.Items.Length; i++)
            {
                var itemId = HItemBook.GetItemId(itemConfig.Items[i]);
                UserProfile.InfoBag.AddItem(itemId, itemConfig.ItemCount[i]);
                itemList.Add(itemId);
                countList.Add(itemConfig.ItemCount[i]);
            }
            var form = new ItemPackageForm();
            form.SetItem(itemList.ToArray(), countList.ToArray());
            PanelManager.DealPanel(form);
            return true;
        }

        private static void CheckInstruction(string ins)
        {
            switch (ins)
            {
                case "detectall": Scene.Instance.DetectAll(); break;
            }
        }
    }
}
