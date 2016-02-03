namespace ConfigDatas
{
	public class LevelInfoConfig
	{
		public int Id;
		public int Level;
		public int Type;
		public string Des;
		public string Icon;
		public LevelInfoConfig(){}
		public LevelInfoConfig(int Id,int Level,int Type,string Des,string Icon)
		{
			this.Id= Id;
			this.Level= Level;
			this.Type= Type;
			this.Des= Des;
			this.Icon= Icon;
		}
		public class Indexer
		{
		}
	}
}
