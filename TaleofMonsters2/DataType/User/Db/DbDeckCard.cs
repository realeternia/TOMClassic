using TaleofMonsters.Core;
using TaleofMonsters.DataType.Others;

namespace TaleofMonsters.DataType.User.Db
{
    public class DbDeckCard
    {
        [FieldIndex(Index = 1)] public int BaseId;
        [FieldIndex(Index = 2)] public byte Level;
        [FieldIndex(Index = 3)] public ushort Exp;

        public DbDeckCard()
        {
        }

        public DbDeckCard(int baseId, byte level, ushort exp)
        {
            BaseId = baseId;
            Level = level;
            Exp = exp;
        }
        public void AddExp(int addon)
        {
            if (Level >= ExpTree.MaxLevel)
            {
                return;
            }

            Exp = (ushort)(Exp + addon);
        }
    }
}
