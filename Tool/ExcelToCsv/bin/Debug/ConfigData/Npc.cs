namespace ConfigDatas
{
	public class NpcConfig
	{
		public int Id;
		public string Name;
		public int MapId;
		public int Lv;
		public int X;
		public int Y;
		public int Func;
		public string Say;
		public string Answer;
		public string Figue;
		public string Icon;
		public NpcConfig(){}
		public NpcConfig(int Id,string Name,int MapId,int Lv,int X,int Y,int Func,string Say,string Answer,string Figue,string Icon)
		{
			this.Id= Id;
			this.Name= Name;
			this.MapId= MapId;
			this.Lv= Lv;
			this.X= X;
			this.Y= Y;
			this.Func= Func;
			this.Say= Say;
			this.Answer= Answer;
			this.Figue= Figue;
			this.Icon= Icon;
		}
		public class Indexer
		{
		}
	}
}
