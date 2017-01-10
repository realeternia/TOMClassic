namespace TaleofMonsters.DataType.Peoples
{
    internal struct PeopleFightParm
    {
        public PeopleFightReason Reason;
        public PeopleFightRuleAddon RuleAddon;
        public int RuleLevel; //1-5
    }

    internal enum PeopleFightReason
    {
        Other,
        PeopleView,
        SceneQuest,
    }
    internal enum PeopleFightRuleAddon
    {
        None,
        TowerHp, //40%塔最大生命
        Energy, //增加初始LP，MP，PP
        AddUnit,
        Card //手牌++
    }
}