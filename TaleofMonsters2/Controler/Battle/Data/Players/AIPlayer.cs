using System.Collections.Generic;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType.Decks;

namespace TaleofMonsters.Controler.Battle.Data.Players
{
    internal class AIPlayer : Player
    {
        public AIPlayer(int id, string deck, bool isLeft, int rlevel, bool isPlayerControl)
            : base(isPlayerControl, isLeft)
        {
            PeopleId = id;

            Level = rlevel;
            PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(id);
            Job = peopleConfig.Job;

            EnergyGenerator.SetRateNpc(peopleConfig);

            DeckCard[] cds = DeckBook.GetDeckByName(deck, Level);
            Cards = new ActiveCards(cds);
          //  PlayerAttr attr = new PlayerAttr();
          //  attr.ModifyMonsterData(HeroData);
            InitBase();
        }
        
        public override void InitialCards()
        {
            base.InitialCards();
        }
        public override List<int> GetInitialMonster()
        {
            var list = new List<int>();
            PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(PeopleId);
            if (peopleConfig.RightMon != null && peopleConfig.RightMon.Length>0)
            {
                list.AddRange(peopleConfig.RightMon);
            }
            BattleManager.Instance.RuleData.CheckInitialMonster(this, list, peopleConfig.PetMon);//会修改player.InitialMonster
            return list;
        }

    }
}
