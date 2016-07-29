using System.Security.Principal;

namespace ConfigDatas
{
    public interface IMonsterAuro
    {
        IMonsterAuro AddRace(string race);
        IMonsterAuro AddAttr(string attr);
        IMonsterAuro SetRange(int range);
        IMonsterAuro SetMid(int mid);
        IMonsterAuro SetStar(int min, int max);
    }
}