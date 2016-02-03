namespace ConfigDatas
{
	public class MazeItemConfig
	{
		public int Id;
		public int MazeId;
		public int X;
		public int Y;
		public int[] Cond;
		public string Type;
		public int[] Info;
		public string Word;
		public MazeItemConfig(){}
		public MazeItemConfig(int Id,int MazeId,int X,int Y,int[] Cond,string Type,int[] Info,string Word)
		{
			this.Id= Id;
			this.MazeId= MazeId;
			this.X= X;
			this.Y= Y;
			this.Cond= Cond;
			this.Type= Type;
			this.Info= Info;
			this.Word= Word;
		}
		public class Indexer
		{
		}
	}
}
