namespace ConfigDatas
{
	public class HeroSkillAttrConfig
	{
		public int Id;
		public string Name;
		public string Des;
		public int HeroLevel;
		public int Atk;
		public int Def;
		public int Magic;
		public int Hit;
		public int Dhit;
		public int Spd;
		public int Hp;
		public string Icon;
		public HeroSkillAttrConfig(){}
		public HeroSkillAttrConfig(int Id,string Name,string Des,int HeroLevel,int Atk,int Def,int Magic,int Hit,int Dhit,int Spd,int Hp,string Icon)
		{
			this.Id= Id;
			this.Name= Name;
			this.Des= Des;
			this.HeroLevel= HeroLevel;
			this.Atk= Atk;
			this.Def= Def;
			this.Magic= Magic;
			this.Hit= Hit;
			this.Dhit= Dhit;
			this.Spd= Spd;
			this.Hp= Hp;
			this.Icon= Icon;
		}
		public class Indexer
		{
		}
	}
}
