﻿using System;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.Decks;
using System.Drawing;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Equips;
using TaleofMonsters.DataType.Others;

namespace TaleofMonsters.Controler.Battle.Data.Players
{
    internal class HumanPlayer : Player
    {
        public HumanPlayer(bool isLeft)
            : base(true, isLeft)
        {
            PeopleId = 0;
            Level = UserProfile.InfoBasic.Level;
            
            DeckCard[] cd = new DeckCard[GameConstants.DeckCardCount];
            for (int i = 0; i < GameConstants.DeckCardCount; i++)
            {
                int id = UserProfile.InfoCard.SelectedDeck.GetCardAt(i);
                cd[i] = UserProfile.InfoCard.GetDeckCardById(id);
            }
            Cards = new ActiveCards(cd);

            PlayerAttr attr = new PlayerAttr();
            int[] energyRate = {0, 0, 0};
            CalculateEquipAndSkill(UserProfile.InfoEquip.Equipon, attr, energyRate);
            EnergyGenerator.SetRate(energyRate);

            InitBase();
        }

        public override void AddResource(GameResourceType type, int number)
        {
            UserProfile.InfoBag.AddResource(type, number);
        }
        
        public override void OnKillMonster(int killerId, int dieLevel, int dieStar, Point position)
        {
            base.OnKillMonster(killerId, dieLevel, dieStar, position);

            int expGet = (int)Math.Sqrt(dieLevel * dieStar) / 2 + 1; //杀死怪物经验
            int rexp = UserProfile.InfoRecord.GetRecordById((int)MemPlayerRecordTypes.HeroExpPoint) + expGet;
            UserProfile.InfoRecord.SetRecordById((int)MemPlayerRecordTypes.HeroExpPoint, Math.Min(ExpTree.GetNextRequiredCard(Level), rexp));
         //   BattleManager.Instance.FlowWordQueue.Add(new FlowExpInfo(expGet, position, 20, 50), true);
        }

        public override void InitialCards()
        {
            base.InitialCards();

#if DEBUG
            CardManager.AddCard(new ActiveCard(53000078, 1, 0));
            CardManager.AddCard(new ActiveCard(52000003, 1, 0));
#endif
        }

        private void CalculateEquipAndSkill(int[] equipids, PlayerAttr attr, int[] energyData)
        {
            var equipList = EquipBook.GetEquipsList(equipids);
            var addon = EquipBook.GetVirtualEquips(equipList);
            attr.AddAttrs(PlayerAttrs.Atk, addon.Atk);
            attr.AddAttrs(PlayerAttrs.Hp, addon.Hp);
            foreach (var equip in equipList)
            {
                EquipConfig equipConfig = ConfigData.GetEquipConfig(equip.TemplateId);
                for (int i = 0; i < 3; i++)
                {
                    energyData[i] += equipConfig.EnergyRate[i];
                }
                if (equipConfig.SpecialSkill > 0)
                {
                    HeroSkillList.Add(equipConfig.SpecialSkill);
                }
            }
            //  State.UpdateSkills(addons.Keys(), addons.Values());
        }
    }
}
