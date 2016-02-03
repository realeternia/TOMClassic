namespace ConfigDatas
{
	public class GameShopConfig
	{
		public int Id;
		public int ItemId;
		public int Type;
		public int Shelf;
		public int Price;
		public GameShopConfig(){}
		public GameShopConfig(int Id,int ItemId,int Type,int Shelf,int Price)
		{
			this.Id= Id;
			this.ItemId= ItemId;
			this.Type= Type;
			this.Shelf= Shelf;
			this.Price= Price;
		}
		public class Indexer
		{
		}
	}
}
