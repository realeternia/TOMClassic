namespace ConfigDatas
{
	public class MissileConfig
	{
		public int Id;
		public string TypeName;
		public string Name;
		public double Speed;
		public int Image;
		public int FrameCount;
		public int FrameTime;
		public MissileConfig(){}
		public MissileConfig(int Id,string TypeName,string Name,double Speed,int Image,int FrameCount,int FrameTime)
		{
			this.Id= Id;
			this.TypeName= TypeName;
			this.Name= Name;
			this.Speed= Speed;
			this.Image= Image;
			this.FrameCount= FrameCount;
			this.FrameTime= FrameTime;
		}
		public class Indexer
		{
		}
	}
}
