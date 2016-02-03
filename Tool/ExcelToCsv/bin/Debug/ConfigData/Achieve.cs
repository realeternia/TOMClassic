namespace ConfigDatas
{
	public class AchieveConfig
	{
		public int Id;
		public string Name;
		public string Descript;
		public int Type;
		public string CheckType;
		public RLIdValue Condition;
		public int Money;
		public string Item;
		public string Icon;
		public AchieveConfig(){}
		public AchieveConfig(int Id,string Name,string Descript,int Type,string CheckType,RLIdValue Condition,int Money,string Item,string Icon)
		{
			this.Id= Id;
			this.Name= Name;
			this.Descript= Descript;
			this.Type= Type;
			this.CheckType= CheckType;
			this.Condition= Condition;
			this.Money= Money;
			this.Item= Item;
			this.Icon= Icon;
		}
		public class Indexer
		{
		}
	}
}
