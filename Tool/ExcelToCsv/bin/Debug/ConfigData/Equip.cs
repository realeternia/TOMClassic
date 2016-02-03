namespace ConfigDatas
{
	public class EquipConfig
	{
		public int Id;
		public string Name;
		public int Quality;
		public int Level;
		public int Position;
		public int PackId;
		public int BaseId;
		public RLIdValueList Addon;
		public int Value;
		public int LvNeed;
		public int JobNeed;
		public string Url;
		public EquipConfig(){}
		public EquipConfig(int Id,string Name,int Quality,int Level,int Position,int PackId,int BaseId,RLIdValueList Addon,int Value,int LvNeed,int JobNeed,string Url)
		{
			this.Id= Id;
			this.Name= Name;
			this.Quality= Quality;
			this.Level= Level;
			this.Position= Position;
			this.PackId= PackId;
			this.BaseId= BaseId;
			this.Addon= Addon;
			this.Value= Value;
			this.LvNeed= LvNeed;
			this.JobNeed= JobNeed;
			this.Url= Url;
		}
		public class Indexer
		{
		}
	}
}
