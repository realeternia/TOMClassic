namespace ConfigDatas
{
	public class SpellConfig
	{
		public int Id;
		public string Name;
		public string Ename;
		public string EnameShort;
		public int Star;
		public int Type;
		public int Attr;
		public int Cost;
		public int Damage;
		public int Cure;
		public double Time;
		public double Help;
		public double Rate;
		public int Modify;
		public int Range;
		public string Target;
		public int Mark;
		public SpellEffectDelegate Effect;
		public string GetDescript;
		public string UnitEffect;
		public int Res;
		public string Icon;
		public int IsSpecial;
		public int IsNew;
		public string Remark;
		public SpellConfig(){}
		public SpellConfig(int Id,string Name,string Ename,string EnameShort,int Star,int Type,int Attr,int Cost,int Damage,int Cure,double Time,double Help,double Rate,int Modify,int Range,string Target,int Mark,SpellEffectDelegate Effect,string GetDescript,string UnitEffect,int Res,string Icon,int IsSpecial,int IsNew,string Remark)
		{
			this.Id= Id;
			this.Name= Name;
			this.Ename= Ename;
			this.EnameShort= EnameShort;
			this.Star= Star;
			this.Type= Type;
			this.Attr= Attr;
			this.Cost= Cost;
			this.Damage= Damage;
			this.Cure= Cure;
			this.Time= Time;
			this.Help= Help;
			this.Rate= Rate;
			this.Modify= Modify;
			this.Range= Range;
			this.Target= Target;
			this.Mark= Mark;
			this.Effect= Effect;
			this.GetDescript= GetDescript;
			this.UnitEffect= UnitEffect;
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
