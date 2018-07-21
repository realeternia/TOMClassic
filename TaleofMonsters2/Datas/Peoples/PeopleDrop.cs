using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Datas.Others;

namespace TaleofMonsters.Datas.Peoples
{
    internal class PeopleDrop
    {
        public uint Gold { get; private set; }
        private PeopleConfig peopleConfig;

        public PeopleDrop(int id)
        {
            peopleConfig = ConfigData.GetPeopleConfig(id);
            uint goldExpect = GameResourceBook.InGoldFight(peopleConfig.Level, PeopleBook.IsPeople(id));
            Gold = (uint)MathTool.GetRandom((int)(goldExpect * 7 / 10), (int)(goldExpect * 13 / 10) + 1);
        }
    }
}
