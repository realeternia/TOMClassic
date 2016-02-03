namespace ConfigDatas
{
	public class SceneWarpConfig
	{
		public int Id;
		public int FromMap;
		public int X;
		public int Y;
		public int ToMap;
		public SceneWarpConfig(){}
		public SceneWarpConfig(int Id,int FromMap,int X,int Y,int ToMap)
		{
			this.Id= Id;
			this.FromMap= FromMap;
			this.X= X;
			this.Y= Y;
			this.ToMap= ToMap;
		}
		public class Indexer
		{
		}
	}
}
