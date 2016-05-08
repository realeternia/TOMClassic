using System.Drawing;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.DataType;

namespace TaleofMonsters.Controler.Battle.Data.MemWeapon
{
    internal interface IBattleWeapon
    {
        int CardId { get; }
        int Level { get; }
        string Arrow { get; }
        int Range { get; }
        int Mov { get; }
        void OnHit();
        void OnHited();

        Image GetImage(int width, int height);

        void CheckWeaponEffect(LiveMonster src, int symbol);

        CardTypeSub Type { get; }

        IBattleWeapon GetCopy();

        string Des { get; }
    }
}