namespace ConfigDatas
{
	public class WeaponConfig
	{
		public int Id;
		public string Name;
		public string Ename;
		public string EnameShort;
		public int Star;
		public int Type;
		public int Attr;
		public int Cost;
		public int AtkP;
		public int VitP;
		public int SkillMark;
		public int Sum;
		public int Modify;
		public int Dura;
		public int Def;
		public int  Mag;
		public int Spd;
		public int Hit;
		public int Dhit;
		public int Crt;
		public int Luk;
		public int Range;
		public int Mov;
		public int SkillId;
		public int Percent;
		public string Arrow;
		public int Res;
		public string Icon;
		public int IsSpecial;
		public int IsNew;
		public string Remark;
		public WeaponConfig(){}
		public WeaponConfig(int Id,string Name,string Ename,string EnameShort,int Star,int Type,int Attr,int Cost,int AtkP,int VitP,int SkillMark,int Sum,int Modify,int Dura,int Def,int  Mag,int Spd,int Hit,int Dhit,int Crt,int Luk,int Range,int Mov,int SkillId,int Percent,string Arrow,int Res,string Icon,int IsSpecial,int IsNew,string Remark)
		{
			this.Id= Id;
			this.Name= Name;
			this.Ename= Ename;
			this.EnameShort= EnameShort;
			this.Star= Star;
			this.Type= Type;
			this.Attr= Attr;
			this.Cost= Cost;
			this.AtkP= AtkP;
			this.VitP= VitP;
			this.SkillMark= SkillMark;
			this.Sum= Sum;
			this.Modify= Modify;
			this.Dura= Dura;
			this.Def= Def;
			this.Mag= Mag;
			this.Spd= Spd;
			this.Hit= Hit;
			this.Dhit= Dhit;
			this.Crt= Crt;
			this.Luk= Luk;
			this.Range= Range;
			this.Mov= Mov;
			this.SkillId= SkillId;
			this.Percent= Percent;
			this.Arrow= Arrow;
			this.Res= Res;
			this.Icon= Icon;
			this.IsSpecial= IsSpecial;
			this.IsNew= IsNew;
			this.Remark= Remark;
		}
		public class Indexer
		{
		}
	}
}
