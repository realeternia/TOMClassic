namespace ConfigDatas
{
	public class SpellTrapConfig
	{
		public int Id;
		public string Name;
		public SpellTrapAddCardDelegate EffectUse;
		public SpellTrapSummonDelegate EffectSummon;
		public string Comment;
		public SpellTrapConfig(){}
		public SpellTrapConfig(int Id,string Name,SpellTrapAddCardDelegate EffectUse,SpellTrapSummonDelegate EffectSummon,string Comment)
		{
			this.Id= Id;
			this.Name= Name;
			this.EffectUse= EffectUse;
			this.EffectSummon= EffectSummon;
			this.Comment= Comment;
		}
		public class Indexer
		{
		}
	}
}
