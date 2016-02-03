namespace ConfigDatas
{
	public class TournamentConfig
	{
		public int Id;
		public string Name;
		public int Type;
		public int ApplyDate;
		public int BeginDate;
		public int EndDate;
		public int MinLevel;
		public int MaxLevel;
		public int PlayerCount;
		public int MatchCount;
		public int[] Awards;
		public RLIdValueList Resource;
		public string Map;
		public string Icon;
		public TournamentConfig(){}
		public TournamentConfig(int Id,string Name,int Type,int ApplyDate,int BeginDate,int EndDate,int MinLevel,int MaxLevel,int PlayerCount,int MatchCount,int[] Awards,RLIdValueList Resource,string Map,string Icon)
		{
			this.Id= Id;
			this.Name= Name;
			this.Type= Type;
			this.ApplyDate= ApplyDate;
			this.BeginDate= BeginDate;
			this.EndDate= EndDate;
			this.MinLevel= MinLevel;
			this.MaxLevel= MaxLevel;
			this.PlayerCount= PlayerCount;
			this.MatchCount= MatchCount;
			this.Awards= Awards;
			this.Resource= Resource;
			this.Map= Map;
			this.Icon= Icon;
		}
		public class Indexer
		{
		}
	}
}
