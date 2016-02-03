namespace ConfigDatas
{
	public class MazeConfig
	{
		public int Id;
		public int Level;
		public string Path;
		public string Map;
		public MazeConfig(){}
		public MazeConfig(int Id,int Level,string Path,string Map)
		{
			this.Id= Id;
			this.Level= Level;
			this.Path= Path;
			this.Map= Map;
		}
		public class Indexer
		{
		}
	}
}
