namespace ConfigDatas
{
	public class DropCardConfig
	{
		public int Id;
		public int Pid;
		public RLIdValueList CardId;
		public DropCardConfig(){}
		public DropCardConfig(int Id,int Pid,RLIdValueList CardId)
		{
			this.Id= Id;
			this.Pid= Pid;
			this.CardId= CardId;
		}
		public class Indexer
		{
		}
	}
}
