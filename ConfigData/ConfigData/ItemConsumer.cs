namespace ConfigDatas
{
	public class ItemConsumerConfig
	{
		public int Id;
		public string Name;
		public int GainExp;
		public int GainAp;
		public int ResourceId;
		public int ResourceCount;
		public int PeopleId;
		public int[] RandomCardRate;
		public int FarmItemId;
		public int FarmTime;
		public int GainLp;
		public int GainMp;
		public int GainPp;
		public int DirectDamage;
		public int FightRandomCardType;
		public ItemConsumerConfig(){}
		public ItemConsumerConfig(int Id,string Name,int GainExp,int GainAp,int ResourceId,int ResourceCount,int PeopleId,int[] RandomCardRate,int FarmItemId,int FarmTime,int GainLp,int GainMp,int GainPp,int DirectDamage,int FightRandomCardType)
		{
			this.Id= Id;
			this.Name= Name;
			this.GainExp= GainExp;
			this.GainAp= GainAp;
			this.ResourceId= ResourceId;
			this.ResourceCount= ResourceCount;
			this.PeopleId= PeopleId;
			this.RandomCardRate= RandomCardRate;
			this.FarmItemId= FarmItemId;
			this.FarmTime= FarmTime;
			this.GainLp= GainLp;
			this.GainMp= GainMp;
			this.GainPp= GainPp;
			this.DirectDamage= DirectDamage;
			this.FightRandomCardType= FightRandomCardType;
		}
		public class Indexer
		{
		}
	}
}
