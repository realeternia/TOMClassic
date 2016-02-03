namespace ConfigDatas
{
	public class ItemGiftConfig
	{
		public int Id;
		public RLIItemRateCountList Items;
		public ItemGiftConfig(){}
		public ItemGiftConfig(int Id,RLIItemRateCountList Items)
		{
			this.Id= Id;
			this.Items= Items;
		}
		public class Indexer
		{
		}
	}
}
