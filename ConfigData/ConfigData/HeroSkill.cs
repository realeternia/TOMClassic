namespace ConfigDatas
{
	public class HeroSkillConfig
	{
		public int Id;
		public string Name;
		public string Des;
		public int Type;
		public int CardId;
		public string Icon;
		public HeroSkillConfig(){}
		public HeroSkillConfig(int Id,string Name,string Des,int Type,int CardId,string Icon)
		{
			this.Id= Id;
			this.Name= Name;
			this.Des= Des;
			this.Type= Type;
			this.CardId= CardId;
			this.Icon= Icon;
		}
		public class Indexer
		{
		}
	}
}
