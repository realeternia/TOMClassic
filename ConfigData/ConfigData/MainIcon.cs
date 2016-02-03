namespace ConfigDatas
{
	public class MainIconConfig
	{
		public int Id;
		public string Name;
		public string Des;
		public int Type;
		public int Record;
		public int Level;
		public int X;
		public int Y;
		public int Width;
		public int Height;
		public int Flow;
		public string Icon;
		public MainIconConfig(){}
		public MainIconConfig(int Id,string Name,string Des,int Type,int Record,int Level,int X,int Y,int Width,int Height,int Flow,string Icon)
		{
			this.Id= Id;
			this.Name= Name;
			this.Des= Des;
			this.Type= Type;
			this.Record= Record;
			this.Level= Level;
			this.X= X;
			this.Y= Y;
			this.Width= Width;
			this.Height= Height;
			this.Flow= Flow;
			this.Icon= Icon;
		}
		public class Indexer
		{
		}
	}
}
