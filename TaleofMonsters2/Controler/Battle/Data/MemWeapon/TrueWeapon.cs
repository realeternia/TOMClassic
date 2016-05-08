using System.Drawing;
using TaleofMonsters.Controler.Battle.Data.MemFlow;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards.Weapons;
using TaleofMonsters.DataType.Decks;

namespace TaleofMonsters.Controler.Battle.Data.MemWeapon
{
    internal class TrueWeapon
    {
        private LiveMonster self;
        public Weapon Avatar { get; private set; }

        public int CardId
        {
            get { return Avatar.Id; }
        }
        public int Life { get; private set; }
        public int Level { get; private set; }

        public bool IsAttackWeapon
        {
            get { return Avatar.WeaponConfig.Type == (int)CardTypeSub.Weapon || Avatar.WeaponConfig.Type == (int)CardTypeSub.Scroll; }
        }

        public TrueWeapon()
        {
            Avatar = new Weapon(0);
        }
        
        public TrueWeapon(LiveMonster lm, int level, Weapon wpn)
        {
            self = lm;
            Avatar = wpn;
            Level = level;
            Life = wpn.Dura;
        }

        public void OnHit()
        {
            if (IsAttackWeapon)
            {
                CheckWeaponLife();
            }
        }
        
        public void OnHited()
        {
            if (!IsAttackWeapon)
            {
                CheckWeaponLife();
            }
        }

        private void CheckWeaponLife()
        {
            Life--;
            if (Life == 0)
            {
                self.BreakWeapon();
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("装备破损", self.Position, -2, "Cyan", 26, 0, 0, -2, 15), false);
            }
            else
            {
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("耐久-1", self.Position, -2, "Cyan", 26, 0, 0, -2, 15), false);
            }
        }

        public DeckCard Card
        {
            get
            {
                return new DeckCard(CardId, (byte)Level, 0);
            }
        }

        public Image GetImage(int width, int height)
        {
            return WeaponBook.GetWeaponImage(Avatar.Id, width, height);  
        }

        public TrueWeapon GetCopy()
        {
            TrueWeapon newWeapon = new TrueWeapon(self, Level, Avatar);
            newWeapon.Life = Life;
            return newWeapon;
        }
    }
}