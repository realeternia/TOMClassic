namespace ConfigDatas
{
	public class HeroSkillCardConfig
	{
		public int Id;
		public string Name;
		public string Des;
		public int CardType;
		public string CardTypeSub;
		public string CardAttr;
		public int MonSkillId;
		public int AddLevel;
		public HeroSkillCardConfig(){}
		public HeroSkillCardConfig(int Id,string Name,string Des,int CardType,string CardTypeSub,string CardAttr,int MonSkillId,int AddLevel)
		{
			this.Id= Id;
			this.Name= Name;
			this.Des= Des;
			this.CardType= CardType;
			this.CardTypeSub= CardTypeSub;
			this.CardAttr= CardAttr;
			this.MonSkillId= MonSkillId;
			this.AddLevel= AddLevel;
		}
		public class Indexer
		{
		}
	}
}
