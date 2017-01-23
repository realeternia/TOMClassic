using System;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.Decks;
using System.Drawing;
using TaleofMonsters.Controler.Battle.Tool;
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
            Job = UserProfile.InfoBasic.Job;
            
            DeckCard[] cd = new DeckCard[GameConstants.DeckCardCount];
            for (int i = 0; i < GameConstants.DeckCardCount; i++)
            {
                int id = UserProfile.InfoCard.SelectedDeck.GetCardAt(i);
                cd[i] = new DeckCard(UserProfile.InfoCard.GetDeckCardById(id));
            }
            Cards = new ActiveCards(cd);

            int[] energyRate = {0, 0, 0};
            int[] equipOn = Array.ConvertAll(UserProfile.InfoEquip.Equipon, a => a.BaseId);
            CalculateEquipAndSkill(equipOn, energyRate);
            EnergyGenerator.SetRate(energyRate, UserProfile.InfoBasic.Job);

            InitBase();

            BattleManager.Instance.RuleData.CheckPlayerData(this);
        }

        public override void AddResource(GameResourceType type, int number)
        {
            if (number > 0)
            {
                UserProfile.InfoBag.AddResource(type, (uint)number);
            }
            else if (number < 0)
            {
                UserProfile.InfoBag.SubResource(type, (uint)(-number));
            }
        }
        
        public override void OnKillMonster(int dieLevel, int dieStar, Point position)
        {
            base.OnKillMonster(dieLevel, dieStar, position);

            int expGet = (int)Math.Sqrt(dieLevel * dieStar) / 2 + 1; //杀死怪物经验
            int rexp = UserProfile.InfoRecord.GetRecordById((int)MemPlayerRecordTypes.HeroExpPoint) + expGet;
            UserProfile.InfoRecord.SetRecordById((int)MemPlayerRecordTypes.HeroExpPoint, Math.Min(ExpTree.GetNextRequiredCard(Level), rexp));
         //   BattleManager.Instance.FlowWordQueue.Add(new FlowExpInfo(expGet, position, 20, 50), true);
        }

        public override void InitialCards()
        {
            base.InitialCards();

#if DEBUG
            //int[] cardToGive = new[] { 51000312 };
            //foreach (var cardId in cardToGive)
            //{
            //    CardManager.AddCard(new ActiveCard(cardId, 1, 0));
            //}
#endif
        }

        private void CalculateEquipAndSkill(int[] equipids, int[] energyData)
        {
            var equipList = EquipBook.GetEquipsList(equipids);
            var addon = EquipBook.GetVirtualEquips(equipList);
            foreach (var equip in equipList)
            {
                EquipConfig equipConfig = ConfigData.GetEquipConfig(equip.TemplateId);
                for (int i = 0; i < 3; i++)
                {
                    energyData[i] += equipConfig.EnergyRate[i];
                }
            
                if (equipConfig.SpecialSkill > 0)
                {
                    HeroSkillList.Add(equipConfig.SpecialSkill); //添加装备附带的技能
                }
            }
            State.UpdateAttr(addon.Atk, addon.Hp);
            State.UpdateSkills(new int[0], new int[0]);
        }
    }
}
