namespace ConfigDatas
{
	public class HeroSkillCommonConfig
	{
		public int Id;
		public string Name;
		public string Des;
		public int Group;
		public int Level;
		public int ForeSkill;
		public int HeroLevel;
		public RLIdValue Addon;
		public string Icon;
		public HeroSkillCommonConfig(){}
		public HeroSkillCommonConfig(int Id,string Name,string Des,int Group,int Level,int ForeSkill,int HeroLevel,RLIdValue Addon,string Icon)
		{
			this.Id= Id;
			this.Name= Name;
			this.Des= Des;
			this.Group= Group;
			this.Level= Level;
			this.ForeSkill= ForeSkill;
			this.HeroLevel= HeroLevel;
			this.Addon= Addon;
			this.Icon= Icon;
		}
		public class Indexer
		{
		}
	}
}
