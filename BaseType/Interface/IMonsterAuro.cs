namespace ConfigDatas
{
    public interface IMonsterAuro
    {
        IMonsterAuro AddRace(string race);
        IMonsterAuro AddAttr(string attr);
        IMonsterAuro SetMid(int mid);
        IMonsterAuro SetStar(int min, int max);
    }
}