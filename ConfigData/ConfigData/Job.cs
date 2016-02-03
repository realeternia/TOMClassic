namespace ConfigDatas
{
	public class JobConfig
	{
		public int Id;
		public string Name;
		public string Arrow;
		public string Des;
		public int[] EnergyRate;
		public int[] InitialCards;
		public int[] InitialItem;
		public int[] InitialEquip;
		public string ProtoImage;
		public JobConfig(){}
		public JobConfig(int Id,string Name,string Arrow,string Des,int[] EnergyRate,int[] InitialCards,int[] InitialItem,int[] InitialEquip,string ProtoImage)
		{
			this.Id= Id;
			this.Name= Name;
			this.Arrow= Arrow;
			this.Des= Des;
			this.EnergyRate= EnergyRate;
			this.InitialCards= InitialCards;
			this.InitialItem= InitialItem;
			this.InitialEquip= InitialEquip;
			this.ProtoImage= ProtoImage;
		}
		public class Indexer
		{
public static readonly int NewBie = 11000000;
		}
	}
}
