namespace ConfigDatas
{
	public class NpcShopConfig
	{
		public int Id;
		public RLIdValueList SellTable;
		public NpcShopConfig(){}
		public NpcShopConfig(int Id,RLIdValueList SellTable)
		{
			this.Id= Id;
			this.SellTable= SellTable;
		}
		public class Indexer
		{
		}
	}
}
