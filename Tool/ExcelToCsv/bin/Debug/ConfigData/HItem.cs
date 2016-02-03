namespace ConfigDatas
{
	public class HItemConfig
	{
		public int Id;
		public string Name;
		public int Type;
		public string Descript;
		public int Level;
		public int Rare;
		public int MaxPile;
		public int Value;
		public int SubType;
		public int[] UseEffect;
		public bool IsUsable;
		public bool IsThrowable;
		public bool IsRandom;
		public int CdGroup;
		public string Url;
		public HItemConfig(){}
		public HItemConfig(int Id,string Name,int Type,string Descript,int Level,int Rare,int MaxPile,int Value,int SubType,int[] UseEffect,bool IsUsable,bool IsThrowable,bool IsRandom,int CdGroup,string Url)
		{
			this.Id= Id;
			this.Name= Name;
			this.Type= Type;
			this.Descript= Descript;
			this.Level= Level;
			this.Rare= Rare;
			this.MaxPile= MaxPile;
			this.Value= Value;
			this.SubType= SubType;
			this.UseEffect= UseEffect;
			this.IsUsable= IsUsable;
			this.IsThrowable= IsThrowable;
			this.IsRandom= IsRandom;
			this.CdGroup= CdGroup;
			this.Url= Url;
		}
		public class Indexer
		{
		}
	}
}
