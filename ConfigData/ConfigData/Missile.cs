namespace ConfigDatas
{
	public class MissileConfig
	{
		public int Id;
		public string TypeName;
		public string Name;
		public double Speed;
		public int Image;
		public MissileConfig(){}
		public MissileConfig(int Id,string TypeName,string Name,double Speed,int Image)
		{
			this.Id= Id;
			this.TypeName= TypeName;
			this.Name= Name;
			this.Speed= Speed;
			this.Image= Image;
		}
		public class Indexer
		{
		}
	}
}
