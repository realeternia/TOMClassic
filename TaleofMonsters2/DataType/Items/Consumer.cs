using System;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType.Cards.Monsters;
using TaleofMonsters.DataType.Cards.Spells;
using TaleofMonsters.DataType.Cards.Weapons;
using TaleofMonsters.DataType.Decks;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms;

namespace TaleofMonsters.DataType.Items
{
    internal static class Consumer
    {
        public static bool UseItemsById(int id, int useMethod)
        {
            HItemConfig itemConfig = ConfigData.GetHItemConfig(id);
            if (itemConfig.Id == ConfigData.NoneHItem.Id)
                return false;

            ItemConsumerConfig consumerConfig = ConfigData.GetItemConsumerConfig(id);
            if (useMethod == HItemTypes.Common)
            {
                if (itemConfig.SubType == HItemTypes.Gift)
                    return UseGift(id);
                if (itemConfig.SubType == HItemTypes.Item)
                    return UseItem(consumerConfig);
                if (itemConfig.SubType == HItemTypes.RandomCard)
                    return UseRandomCard(consumerConfig);
                if (itemConfig.SubType == HItemTypes.RandomItem)
                    return UseRandomItem(consumerConfig);
            }
            else if (useMethod == HItemTypes.Fight)
            {
                if (itemConfig.SubType == HItemTypes.Fight)
                    return UseFightItem(consumerConfig);
            }
            else if (useMethod == HItemTypes.Seed)
            {
                if (itemConfig.SubType == HItemTypes.Seed)
                    return UseSeedItem(consumerConfig);
            }

            return false;
        }

        private static bool UseItem(ItemConsumerConfig itemConfig)
        {
            if (itemConfig.ResourceId > 0)
            {
                UserProfile.InfoBag.AddResource((GameResourceType)(itemConfig.ResourceId - 1), (uint)itemConfig.ResourceCount);
            }
            if (itemConfig.GainExp > 0)
            {
                UserProfile.InfoBasic.AddExp(itemConfig.GainExp);
            }
            if (itemConfig.GainAp > 0)
            {
                UserProfile.InfoBasic.AddFood((uint)itemConfig.GainAp);
            }

            return true;
        }

        private static bool UseFightItem(ItemConsumerConfig itemConfig)
        {
            var player = BattleManager.Instance.PlayerManager.LeftPlayer;
            if (itemConfig.GainLp > 0)
            {
                player.AddLp(itemConfig.GainLp);
            }
            if (itemConfig.GainPp > 0)
            {
                player.AddPp(itemConfig.GainPp);
            }
            if (itemConfig.GainMp > 0)
            {
                player.AddMp(itemConfig.GainMp);
            }

            if (itemConfig.DirectDamage > 0)
            {
                player.DirectDamage += itemConfig.DirectDamage;
            }

            if (itemConfig.FightRandomCardType > 0)
            {
                int cardId = 0;
                switch (itemConfig.FightRandomCardType)
                {
                    case 1: cardId = MonsterBook.GetRandMonsterId(); break;
                    case 2: cardId = WeaponBook.GetRandWeaponId(); break;
                    case 3: cardId = SpellBook.GetRandSpellId(); break;
                }
                var card = new ActiveCard(new DeckCard(cardId, 1, 0));
                player.CardManager.AddCard(card);
            }
            if (!String.IsNullOrEmpty(itemConfig.HolyWord))
            {
                player.AddHolyWord(itemConfig.HolyWord);
            }
            if (itemConfig.AttrAddAfterSummon != null && itemConfig.AttrAddAfterSummon.Length>0)
            {
                player.AddMonsterAddon(itemConfig.AttrAddAfterSummon);
            }
            if (itemConfig.AddTowerHp > 0)
            {
                player.AddTowerHp(itemConfig.AddTowerHp);
            }
            return true;
        }

        private static bool UseSeedItem(ItemConsumerConfig itemConfig)
        {
            return UserProfile.Profile.InfoFarm.UseSeed(itemConfig.FarmItemId, itemConfig.FarmTime);
        }
        
        private static bool UseRandomCard(ItemConsumerConfig itemConfig)
        {
            var form = MainForm.Instance.FindForm(typeof(CardBagForm));
            if (form != null)//如果打开着开包面板，退出
            {
                return false;
            }
            form = new CardBagForm();
            ((CardBagForm)form).SetEffect(itemConfig.Id);
            MainForm.Instance.DealPanel(form);

            return true;
        }
        public static bool UseRandomItem(ItemConsumerConfig itemConfig)
        {
            for (int i = 0; i < itemConfig.RandomItemCount; i++)
            {
                int sum = 0;
                foreach (var r in itemConfig.RandomItemRate)
                {
                    sum += r;
                }
                int roll = MathTool.GetRandom(sum);
                int rare = 0;
                sum = 0;
                for (int j = 0; j < itemConfig.RandomItemRate.Length; j++)
                {
                    sum += itemConfig.RandomItemRate[j];
                    if (roll < sum)
                    {
                        rare = j+1;
                        break;
                    }
                }

                UserProfile.InfoBag.AddItem(HItemBook.GetRandRareItemId(rare), 1);
            }

            return true;
        }

        private static bool UseGift(int id)
        {
            RLIItemRateCountList items = ConfigData.GetItemGiftConfig(id).Items;

            int roll = MathTool.GetRandom(1, 101);
            for (int i = 0; i < items.Count; i++)
            {
                RLIItemRateCount item = items[i];

                if (roll < item.RollMin || roll > item.RollMax)
                    continue;
                if (item.Type == 1)
                {
                    UserProfile.InfoBag.AddItem(item.Id, item.Count);
                }
                else if (item.Type == 2)
                {
                    UserProfile.InfoEquip.AddEquip(item.Id, 0);
                }
                else if (item.Type == 3)
                {
                    if (CardConfigManager.GetCardConfig(item.Id).Id>0)
                    {
                        UserProfile.InfoCard.AddCard(item.Id);
                    }
                }
            }

            return true;
        }
    }
}
