namespace JLM.NetSocket
{
    public class RankData
    {
        public string Name;
        public int Job;
        public int Level;
        public int Exp;

        public void Write(TBinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(Job);
            writer.Write(Level);
            writer.Write(Exp);
        }
        public void Read(TBinaryReader reader)
        {
            Name = reader.ReadString();
            Job = reader.ReadInt32();
            Level = reader.ReadInt32();
            Exp = reader.ReadInt32();
        }
    }
}