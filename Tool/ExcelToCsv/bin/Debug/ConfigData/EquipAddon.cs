namespace ConfigDatas
{
	public class EquipAddonConfig
	{
		public int Id;
		public string Name;
		public string Format;
		public string Type;
		public int Rare;
		public EquipAddonConfig(){}
		public EquipAddonConfig(int Id,string Name,string Format,string Type,int Rare)
		{
			this.Id= Id;
			this.Name= Name;
			this.Format= Format;
			this.Type= Type;
			this.Rare= Rare;
		}
		public class Indexer
		{
		}
	}
}
