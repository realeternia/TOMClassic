namespace ConfigDatas
{
	public class TournamentMatchConfig
	{
		public int Id;
		public int Tid;
		public int Offset;
		public int Date;
		public int LeftType;
		public int LeftValue;
		public int RightType;
		public int RightValue;
		public TournamentMatchConfig(){}
		public TournamentMatchConfig(int Id,int Tid,int Offset,int Date,int LeftType,int LeftValue,int RightType,int RightValue)
		{
			this.Id= Id;
			this.Tid= Tid;
			this.Offset= Offset;
			this.Date= Date;
			this.LeftType= LeftType;
			this.LeftValue= LeftValue;
			this.RightType= RightType;
			this.RightValue= RightValue;
		}
		public class Indexer
		{
		}
	}
}
