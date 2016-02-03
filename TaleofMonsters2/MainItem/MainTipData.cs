namespace TaleofMonsters.MainItem
{
    internal struct MainTipData
    {
        public string Word;
        public string Color;
        public long CreateTime;

        public override string ToString()
        {
            return string.Format("{0}:{1}", Color, Word);
        }
    }
}
