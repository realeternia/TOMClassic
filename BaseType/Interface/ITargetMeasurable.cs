using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigDatas
{
    public interface ITargetMeasurable
    {
        string Target { get; }
        string Shape { get; }

        int Range { get;  }
    }
}
