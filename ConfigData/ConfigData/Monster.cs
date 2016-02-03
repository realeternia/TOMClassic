namespace ConfigDatas
{
	public class MonsterConfig
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
		public int DefP;
		public int MagP;
		public int HitP;
		public int DhitP;
		public int LukP;
		public int SpdP;
		public int VitP;
		public int Sum;
		public int Modify;
		public string Arrow;
		public string Cover;
		public RLVector3List Skills;
		public double[] AttrAtk;
		public double[] AttrDef;
		public int Res;
		public int Icon;
		public int IsSpecial;
		public int IsNew;
		public double VsMark;
		public string Remark;
		public MonsterConfig(){}
		public MonsterConfig(int Id,string Name,string Ename,string EnameShort,int Star,int Type,int Attr,int Cost,int AtkP,int DefP,int MagP,int HitP,int DhitP,int LukP,int SpdP,int VitP,int Sum,int Modify,string Arrow,string Cover,RLVector3List Skills,double[] AttrAtk,double[] AttrDef,int Res,int Icon,int IsSpecial,int IsNew,double VsMark,string Remark)
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
			this.DefP= DefP;
			this.MagP= MagP;
			this.HitP= HitP;
			this.DhitP= DhitP;
			this.LukP= LukP;
			this.SpdP= SpdP;
			this.VitP= VitP;
			this.Sum= Sum;
			this.Modify= Modify;
			this.Arrow= Arrow;
			this.Cover= Cover;
			this.Skills= Skills;
			this.AttrAtk= AttrAtk;
			this.AttrDef= AttrDef;
			this.Res= Res;
			this.Icon= Icon;
			this.IsSpecial= IsSpecial;
			this.IsNew= IsNew;
			this.VsMark= VsMark;
			this.Remark= Remark;
		}
		public class Indexer
		{
public static readonly int HeroCardId = 51019001;
		}
	}
}
