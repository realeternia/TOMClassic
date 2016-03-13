using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Cards.Monsters;
using TaleofMonsters.DataType.Cards.Spells;
using TaleofMonsters.DataType.Cards.Weapons;
using TaleofMonsters.DataType.Decks;

namespace TaleofMonsters.Controler.Battle.Data.Players
{
    internal class RandomPlayer : Player
    {
        public RandomPlayer(int id, bool isLeft, bool isplayerControl)
            : base(isplayerControl, isLeft)
        {
            PeopleId = id;

            PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(id);
            Level = peopleConfig.Level;

            EnergyGenerator.SetRateNpc(peopleConfig);

            DeckCard[] cd = new DeckCard[GameConstants.DeckCardCount];
            for (int i = 0; i < GameConstants.DeckCardCount; i++)
            {
                switch (MathTool.GetRandom(7))
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        cd[i] = new DeckCard( MonsterBook.GetRandMonsterId(), 1, 0);
                        break;
                    case 5:
                        cd[i] = new DeckCard( WeaponBook.GetRandWeaponId(), 1, 0);
                        break;
                    case 6:
                        cd[i] = new DeckCard(SpellBook.GetRandSpellId(), 1, 0);
                        break;
                }
            }
            Cards = new ActiveCards(cd);
            InitBase();
        }
    }
}
