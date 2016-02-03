namespace ConfigDatas
{
	public class BuffConfig
	{
		public int Id;
		public string Name;
		public string Type;
		public BuffEffectDelegate OnAdd;
		public BuffEffectDelegate OnRemove;
		public BuffEffectDelegate OnRound;
		public int[] Effect;
		public bool EndOnHit;
		public FormatStringDelegate GetDescript;
		public int Mark;
		public string Icon;
		public BuffConfig(){}
		public BuffConfig(int Id,string Name,string Type,BuffEffectDelegate OnAdd,BuffEffectDelegate OnRemove,BuffEffectDelegate OnRound,int[] Effect,bool EndOnHit,FormatStringDelegate GetDescript,int Mark,string Icon)
		{
			this.Id= Id;
			this.Name= Name;
			this.Type= Type;
			this.OnAdd= OnAdd;
			this.OnRemove= OnRemove;
			this.OnRound= OnRound;
			this.Effect= Effect;
			this.EndOnHit= EndOnHit;
			this.GetDescript= GetDescript;
			this.Mark= Mark;
			this.Icon= Icon;
		}
		public class Indexer
		{
		}
	}
}
