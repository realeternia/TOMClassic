namespace TaleofMonsters.DataType.Effects
{
    internal class Effect
    {
        public string Name { get; set; }
        public string SoundName { get; set; }
        public int SpeedDown { get; set; }
        public EffectFrame[] Frames { get; set; }

        public Effect(string name)
        {
            Name = name;
            SpeedDown = 1;
        }
    }

    internal class Effects
    {
        public static Effect None = new Effect("");
    }
}
