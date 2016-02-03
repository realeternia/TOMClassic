namespace ConfigDatas
{
	public class TileConfig
	{
		public int Id;
		public string Name;
		public string Cname;
		public int Type;
		public string Color;
		public TileConfig(){}
		public TileConfig(int Id,string Name,string Cname,int Type,string Color)
		{
			this.Id= Id;
			this.Name= Name;
			this.Cname= Cname;
			this.Type= Type;
			this.Color= Color;
		}
		public class Indexer
		{
		}
	}
}
