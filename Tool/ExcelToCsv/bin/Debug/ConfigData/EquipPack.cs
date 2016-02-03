namespace ConfigDatas
{
	public class EquipPackConfig
	{
		public int Id;
		public string Name;
		public int Count;
		public RLIdValueList Addon;
		public EquipPackConfig(){}
		public EquipPackConfig(int Id,string Name,int Count,RLIdValueList Addon)
		{
			this.Id= Id;
			this.Name= Name;
			this.Count= Count;
			this.Addon= Addon;
		}
		public class Indexer
		{
		}
	}
}
