namespace ConfigDatas
{
	public class TaskConfig
	{
		public int Id;
		public string Name;
		public int Former;
		public int Exclude;
		public int Level;
		public RLXY Position;
		public int Main;
		public int StartNpc;
		public int EndNpc;
		public int Type;
		public int FightId;
		public int FightLand;
		public int[] NeedItemId;
		public int[] NeedItemCount;
		public int NeedLevel;
		public int[] ChooseAttr;
		public int WinId;
		public int WinLevel;
		public int WinCount;
		public int NeedResourceId;
		public int NeedResourceCount;
		public int ItemGive;
		public string Descript;
		public string Hint;
		public int Money;
		public string Resource;
		public int Exp;
		public int Card;
		public RLIdValueList Item;
		public string Icon;
		public TaskConfig(){}
		public TaskConfig(int Id,string Name,int Former,int Exclude,int Level,RLXY Position,int Main,int StartNpc,int EndNpc,int Type,int FightId,int FightLand,int[] NeedItemId,int[] NeedItemCount,int NeedLevel,int[] ChooseAttr,int WinId,int WinLevel,int WinCount,int NeedResourceId,int NeedResourceCount,int ItemGive,string Descript,string Hint,int Money,string Resource,int Exp,int Card,RLIdValueList Item,string Icon)
		{
			this.Id= Id;
			this.Name= Name;
			this.Former= Former;
			this.Exclude= Exclude;
			this.Level= Level;
			this.Position= Position;
			this.Main= Main;
			this.StartNpc= StartNpc;
			this.EndNpc= EndNpc;
			this.Type= Type;
			this.FightId= FightId;
			this.FightLand= FightLand;
			this.NeedItemId= NeedItemId;
			this.NeedItemCount= NeedItemCount;
			this.NeedLevel= NeedLevel;
			this.ChooseAttr= ChooseAttr;
			this.WinId= WinId;
			this.WinLevel= WinLevel;
			this.WinCount= WinCount;
			this.NeedResourceId= NeedResourceId;
			this.NeedResourceCount= NeedResourceCount;
			this.ItemGive= ItemGive;
			this.Descript= Descript;
			this.Hint= Hint;
			this.Money= Money;
			this.Resource= Resource;
			this.Exp= Exp;
			this.Card= Card;
			this.Item= Item;
			this.Icon= Icon;
		}
		public class Indexer
		{
public static readonly int Start = 12000001;
		}
	}
}
