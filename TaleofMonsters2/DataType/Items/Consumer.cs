using ConfigDatas;
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
        public static bool UseOre(ItemConsumerConfig itemConfig)
        {
            UserProfile.InfoBag.AddResource((GameResourceType)(itemConfig.ResourceId - 1), (uint)itemConfig.ResourceCount);

            return true;
        }
        
        public static bool UseFightItem(ItemConsumerConfig itemConfig)
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
            if (!string.IsNullOrEmpty(itemConfig.HolyWord))
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

        public static bool UseSeedItem(ItemConsumerConfig itemConfig)
        {
            return UserProfile.Profile.InfoFarm.UseSeed(itemConfig.FarmItemId, itemConfig.FarmTime);
        }

        public static bool UseItem(ItemConsumerConfig itemConfig)
        {
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

        public static bool UsePcard(ItemConsumerConfig itemConfig)
        {
            UserProfile.InfoRival.SetRivalAvail(itemConfig.PeopleId);

            return true;
        }

        public static bool UseScard(ItemConsumerConfig itemConfig)
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
    }
}
