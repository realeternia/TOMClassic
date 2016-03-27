using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigDatas
{
    public interface ISpell : ITargetMeasurable
    {
        int Id { get; }
        int Level { get; }

        int Damage { get;  }
        int Cure { get;  }
        double Time { get; }
        double Help { get; }
        double Rate { get; }
    }
}
