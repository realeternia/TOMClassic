namespace TaleofMonsters.Datas.Decks
{
    public interface IMemCardData
    {
        int CardId { get;  }
        byte Level { get; }
        ushort Exp { get;  }
    }
}