namespace TaleofMonsters.Datas.Peoples
{
    internal struct PeopleFightParm
    {
        public PeopleFightReason Reason;
    }

    internal enum PeopleFightReason
    {
        Other,
        PeopleView,
        SceneQuest,
    }
}