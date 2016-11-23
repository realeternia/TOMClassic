namespace ConfigDatas
{
    public interface ISpell : ITargetMeasurable
    {
        int Id { get; }
        int Level { get; }
        int Attr { get; }

        int Damage { get;  }
        int Cure { get;  }
        double Time { get; }
        double Help { get; }
        double Rate { get; }

        int Atk { get; }
    }
}
