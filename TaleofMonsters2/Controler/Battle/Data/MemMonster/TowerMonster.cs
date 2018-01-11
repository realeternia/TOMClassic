using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards.Monsters;

namespace TaleofMonsters.Controler.Battle.Data.MemMonster
{
    internal class TowerMonster : LiveMonster
    {
        public bool IsKing { get; private set; }

        public override bool CanMove
        {
            get
            {
                if (OwnerPlayer.DeckCards.LeftCount <= 0) //如果手牌没了，可以御驾亲征
                    return true;
                return base.CanMove;
            }
        }

        public TowerMonster(int level, Monster mon, bool isKing, Point point, bool isLeft) 
            : base(level, mon, point, isLeft)
        {
            IsKing = isKing;
        }

        protected override void DrawImg(Graphics g)
        {
            Image img = null;
            if (Type == (int) CardTypeSub.NormalTower)
            {
                img = MonsterBook.GetMonsterImage(Avatar.Id, 100, 100);
                if (img != null)
                    g.DrawImage(img, 0, 0, 100, 100);
                return;
            }

            if (OwnerPlayer.Modifier.CoreId != 0)
            {
                EquipConfig equipConfig = ConfigData.GetEquipConfig(OwnerPlayer.Modifier.CoreId);
                img = PicLoader.Read("Equip.Big", string.Format("{0}.JPG", equipConfig.Url));
                if (img != null)
                {
                    g.DrawImage(img, 0, 0, 100, 100);
                    img.Dispose();
                }
            }
            else
            {
                img = MonsterBook.GetMonsterImage(Avatar.Id, 100, 100);
                if (img != null)
                    g.DrawImage(img, 0, 0, 100, 100);
            }

            if (OwnerPlayer.Modifier.Wall1Id != 0)
            {
                EquipConfig equipConfig = ConfigData.GetEquipConfig(OwnerPlayer.Modifier.Wall1Id);
                img = PicLoader.Read("Equip", string.Format("{0}.JPG", equipConfig.Url));
                if (img != null)
                {
                    g.DrawImage(img, new Rectangle(4, 70, 24, 24), 8, 8, 48, 48, GraphicsUnit.Pixel);
                    img.Dispose();
                    g.DrawRectangle(Pens.Lime, 4, 70, 24, 24);
                }
            }
            if (OwnerPlayer.Modifier.Wall2Id != 0)
            {
                EquipConfig equipConfig = ConfigData.GetEquipConfig(OwnerPlayer.Modifier.Wall2Id);
                img = PicLoader.Read("Equip", string.Format("{0}.JPG", equipConfig.Url));
                if (img != null)
                {
                    g.DrawImage(img, new Rectangle(30, 70, 24, 24), 8, 8, 48, 48, GraphicsUnit.Pixel);
                    img.Dispose();
                    g.DrawRectangle(Pens.Lime, 30, 70, 24, 24);
                }
            }
        }

        private bool pingPong;
        public override string Arrow
        {
            get
            {
                int wid = 0;
                if (OwnerPlayer.Modifier.Weapon1Id ==0 && OwnerPlayer.Modifier.Weapon2Id != 0)
                {
                    wid = OwnerPlayer.Modifier.Weapon2Id;
                }
                else if (OwnerPlayer.Modifier.Weapon2Id == 0 && OwnerPlayer.Modifier.Weapon1Id != 0)
                {
                    wid = OwnerPlayer.Modifier.Weapon1Id;
                }
                else if (OwnerPlayer.Modifier.Weapon2Id != 0 && OwnerPlayer.Modifier.Weapon1Id != 0)
                {
                    pingPong = !pingPong;
                    wid = pingPong ? OwnerPlayer.Modifier.Weapon1Id : OwnerPlayer.Modifier.Weapon2Id;
                }

                string arrow = Avatar.MonsterConfig.Arrow;
                if (wid > 0)
                    arrow = ConfigData.GetEquipConfig(wid).Arrow;

                return arrow;
            }
        }

        public override void OnDie()
        {
            base.OnDie();

            if(IsKing)
                OwnerPlayer.IsAlive = false;
        }
    }
}